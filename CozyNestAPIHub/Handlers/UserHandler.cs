using CozyNestAPIHub.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;

namespace CozyNestAPIHub.Handlers
{
    public class UserHandler
    {
        private static string _connectionString;
        private static string secretKey; 
        private static readonly SemaphoreSlim _userReadLock = new(1, 1);
        private static readonly SemaphoreSlim _userWriteLock = new(1, 1);
        private static readonly SemaphoreSlim _roleReadLock = new(1, 1);
        private static readonly SemaphoreSlim _tokenReadLock = new(1, 1);
        private static readonly SemaphoreSlim _tokenWriteLock = new(1, 1);
        private static readonly ConcurrentDictionary<int, User> _userCacheById = new();
        private static readonly ConcurrentDictionary<string, User> _userCacheByUsername = new();
        private static readonly ConcurrentDictionary<int, Role> _roleCacheById = new();
        private static readonly ConcurrentDictionary<string, Role> _roleCacheByName = new();
        public static void SetSecretKey(string secret)
        {
            secretKey = secret;
        }
        public static void Initialize(string username, string password)
        {
            if (!string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("UserHandler is already initialized.");

            _connectionString = $"Server=localhost;Database=cozynest;User ID={username};Password={password};";
        }
        private static MySqlConnection CreateConnection() => new(_connectionString);
        private static User MapUser(DbDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt32("id"),
                Username = reader.GetString("username"),
                Email = reader.GetString("email"),
                Address = reader.GetString("address"),
                HashedPassword = reader.GetString("hashed_password"),
                FirstName = reader.GetString("first_name"),
                LastName = reader.GetString("last_name"),
                Closed = reader.GetBoolean("closed"),
                JoinDate = reader.GetDateTime("join_date"),
                RoleId = reader.GetInt32("role_id")
            };
        }
        public static async Task<List<User>> GetUsers()
        {
            var users = new List<User>();
            await _userReadLock.WaitAsync();
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();
                string query = "SELECT id, username, email, address, hashed_password, first_name, last_name, closed, join_date, role_id FROM users;";
                using var command = new MySqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var user = MapUser(reader);
                    _userCacheById[user.Id] = user;
                    _userCacheByUsername[user.Username] = user;
                    users.Add(user);
                }
            }
            finally
            {
                _userReadLock.Release();
            }
            return users;
        }

        public static async Task<User?> GetUserById(int id)
        {
            if (_userCacheById.TryGetValue(id, out User cachedUser))
            {
                return cachedUser;
            }

            await _userReadLock.WaitAsync();
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();
                string query = "SELECT * FROM users WHERE id = @userId;";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", id);
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var user = MapUser(reader);
                    _userCacheById[id] = user;
                    _userCacheByUsername[user.Username] = user;
                    return user;
                }
            }
            finally
            {
                _userReadLock.Release();
            }
            return null;
        }

        public static async Task<User?> GetUserByUsername(string username)
        {
            if (_userCacheByUsername.TryGetValue(username, out User cachedUser))
            {
                return cachedUser;
            }

            await _userReadLock.WaitAsync();
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();
                string query = "SELECT * FROM users WHERE username = @username;";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var user = MapUser(reader);
                    _userCacheById[user.Id] = user;
                    _userCacheByUsername[username] = user;
                    return user;
                }
            }
            finally
            {
                _userReadLock.Release();
            }
            return null;
        }

        public static async Task<User?> ModifyUser(User user)
        {
            await _userWriteLock.WaitAsync();
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();
                string updateQuery = @"UPDATE users SET 
            username = @username,
            email = @email,
            address = @address,
            hashed_password = @hashedpassword,
            first_name = @firstname,
            last_name = @lastname,
            closed = @closed,
            role_id = @roleid
        WHERE id = @userId;";

                using var command = new MySqlCommand(updateQuery, connection);
                command.Parameters.AddWithValue("@userId", user.Id);
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@address", user.Address);
                command.Parameters.AddWithValue("@hashedpassword", user.HashedPassword);
                command.Parameters.AddWithValue("@firstname", user.FirstName);
                command.Parameters.AddWithValue("@lastname", user.LastName);
                command.Parameters.AddWithValue("@closed", user.Closed);
                command.Parameters.AddWithValue("@roleid", user.RoleId);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    _userCacheById[user.Id] = user;
                    _userCacheByUsername[user.Username] = user;
                    return user;
                }
            }
            finally
            {
                _userWriteLock.Release();
            }
            return null;
        }

        public static async Task<User?> CreateUser(User user)
        {
            await _userWriteLock.WaitAsync();
            try
            {
                string insertQuery = @"INSERT INTO users (username, email, address, hashed_password, first_name, last_name, join_date, role_id) 
                                       VALUES (@username, @email, @address, @hashedpassword, @firstname, @lastname, @joindate, @roleid);
                                       SELECT LAST_INSERT_ID();";

                using var connection = CreateConnection();
                await connection.OpenAsync();

                using var command = new MySqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@address", user.Address);
                command.Parameters.AddWithValue("@hashedpassword", user.HashedPassword);
                command.Parameters.AddWithValue("@firstname", user.FirstName);
                command.Parameters.AddWithValue("@lastname", user.LastName);
                command.Parameters.AddWithValue("@joindate", DateTime.UtcNow);
                command.Parameters.AddWithValue("@roleid", user.RoleId);
                object result = await command.ExecuteScalarAsync();
                if (result != null && int.TryParse(result.ToString(), out int newUserId))
                {
                    user.Id = newUserId;
                    return user;
                }
            }
            finally
            {
                _userWriteLock.Release();
            }
            return null;
        }

        public static async Task<bool> UserExists(string username, string email)
        {
            await _userReadLock.WaitAsync();
            try
            {
                string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username OR email = @email;";

                using var connection = CreateConnection();
                await connection.OpenAsync();

                await using (var command = new MySqlCommand(checkQuery, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@email", email);
                    int result = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return result > 0;
                }
            }
            finally
            {
                _userReadLock.Release();
            }
        }

        private static string GenerateToken()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] tokenData = new byte[32];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData);
            }
        }

        private static string GenerateJwtToken(int userId, string username)
        {
            var header = new
            {
                alg = "HS256",
                typ = "JWT"
            };

            var headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header));
            var base64UrlHeader = Base64UrlEncode(headerBytes);

            var payload = new
            {
                sub = username,
                jti = Guid.NewGuid().ToString(),
                userId = userId,
                exp = DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds()
            };

            var payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));
            var base64UrlPayload = Base64UrlEncode(payloadBytes);

            var signatureInput = $"{base64UrlHeader}.{base64UrlPayload}";
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            using (var hmac = new HMACSHA256(keyBytes))
            {
                var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureInput));
                var base64UrlSignature = Base64UrlEncode(signatureBytes);

                return $"{base64UrlHeader}.{base64UrlPayload}.{base64UrlSignature}";
            }
        }

        private static string Base64UrlEncode(byte[] input)
        {
            var base64 = Convert.ToBase64String(input);
            base64 = base64.Split('=')[0];
            base64 = base64.Replace('+', '-');
            base64 = base64.Replace('/', '_');
            return base64;
        }
        public static async Task<bool> TokenExists(string token)
        {
            await _tokenReadLock.WaitAsync();
            try
            {
                string checkQuery = "SELECT COUNT(*) FROM tokens WHERE access_token = @token OR refresh_token = @token;";
                using var connection = CreateConnection();
                await connection.OpenAsync();
                await using (var command = new MySqlCommand(checkQuery, connection))
                {
                    command.Parameters.AddWithValue("@token", token);
                    int result = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return result > 0;
                }
            }
            finally
            {
                _tokenReadLock.Release();
            }
        }
        public static async Task<bool> TokenExists(string accessToken, string refreshToken)
        {
            await _tokenReadLock.WaitAsync();
            try
            {
                string checkQuery = "SELECT COUNT(*) FROM tokens WHERE access_token = @accessToken OR refresh_token = @refreshToken;";
                using var connection = CreateConnection();
                await connection.OpenAsync();
                await using (var command = new MySqlCommand(checkQuery, connection))
                {
                    command.Parameters.AddWithValue("@accessToken", accessToken);
                    command.Parameters.AddWithValue("@refreshToken", refreshToken);
                    int result = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return result > 0;
                }
            }
            finally
            {
                _tokenReadLock.Release();
            }
        }
        public static async Task<Token?> CreateToken(User user)
        {
            await _tokenWriteLock.WaitAsync();
            try
            {
                bool tokenExists = false;
                string accessToken = ""; 
                string refreshToken = "";
                do
                {
                    accessToken = GenerateJwtToken(user.Id, user.Username);
                    refreshToken = GenerateToken();
                    tokenExists = await TokenExists(accessToken, refreshToken);
                } while (tokenExists);
                DateTime accessExpiry = DateTime.UtcNow.AddMinutes(30);
                DateTime refreshExpiry = DateTime.UtcNow.AddDays(7);

                string insertTokenQuery = @"INSERT INTO tokens (user_id, access_token, refresh_token, access_expiry, refresh_expiry, is_active) 
                                    VALUES (@userId, @accessToken, @refreshToken, @accessExpiry, @refreshExpiry, @isActive);";

                using var connection = CreateConnection();
                await connection.OpenAsync();

                await using (var command = new MySqlCommand(insertTokenQuery, connection))
                {
                    command.Parameters.AddWithValue("@userId", user.Id);
                    command.Parameters.AddWithValue("@accessToken", accessToken);
                    command.Parameters.AddWithValue("@refreshToken", refreshToken);
                    command.Parameters.AddWithValue("@accessExpiry", accessExpiry);
                    command.Parameters.AddWithValue("@refreshExpiry", refreshExpiry);
                    command.Parameters.AddWithValue("@isActive", true);

                    await command.ExecuteNonQueryAsync();

                    return new Token
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        AccessExpiry = accessExpiry,
                        RefreshExpiry = refreshExpiry,
                        UserId = user.Id,
                        IsActive = true
                    };
                }
            }
            finally
            {
                _tokenWriteLock.Release();
            }
        }

        public static async Task<bool> ValidateAccessToken(string accessToken)
        {
            await _tokenReadLock.WaitAsync();
            try
            {
                string checkQuery = "SELECT is_active, access_expiry FROM tokens WHERE access_token = @accessToken;";

                using var connection = CreateConnection();
                await connection.OpenAsync();

                await using (var command = new MySqlCommand(checkQuery, connection))
                {
                    command.Parameters.AddWithValue("@accessToken", accessToken);

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            bool isActive = reader.GetBoolean("is_active");
                            DateTime accessExpiry = reader.GetDateTime("access_expiry");

                            return isActive && accessExpiry > DateTime.UtcNow;
                        }
                    }
                }

                return false;
            }
            finally
            {
                _tokenReadLock.Release();
            }
        }

        public static async Task<bool> ValidateRefreshToken(string refreshToken)
        {
            await _tokenReadLock.WaitAsync();
            try
            {
                string checkQuery = "SELECT is_active, refresh_expiry FROM tokens WHERE refresh_token = @refreshToken;";

                using var connection = CreateConnection();
                await connection.OpenAsync();

                await using (var command = new MySqlCommand(checkQuery, connection))
                {
                    command.Parameters.AddWithValue("@refreshToken", refreshToken);

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            bool isActive = reader.GetBoolean("is_active");
                            DateTime refreshExpiry = reader.GetDateTime("refresh_expiry");

                            return isActive && refreshExpiry > DateTime.UtcNow;
                        }
                    }
                }

                return false;
            }
            finally
            {
                _tokenReadLock.Release();
            }
        }
        public static async Task<bool> RevokeToken(string token)
        {
            await _tokenWriteLock.WaitAsync();
            try
            {
                string revokeQuery = "UPDATE tokens SET is_active = false WHERE access_token = @token OR refresh_token = @token;";

                using var connection = CreateConnection();
                await connection.OpenAsync();

                await using (var command = new MySqlCommand(revokeQuery, connection))
                {
                    command.Parameters.AddWithValue("@token", token);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            finally
            {
                _tokenWriteLock.Release();
            }
        }
        public static async Task<bool> RevokeAllTokensForUser(User user)
        {
            await _tokenWriteLock.WaitAsync();
            try
            {
                string revokeQuery = "UPDATE tokens SET is_active = false WHERE user_id = @userId;";

                using var connection = CreateConnection();
                await connection.OpenAsync();

                await using (var command = new MySqlCommand(revokeQuery, connection))
                {
                    command.Parameters.AddWithValue("@userId", user.Id);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            finally
            {
                _tokenWriteLock.Release();
            }
        }

        public static async Task<User?> GetUserByAccessToken(string accessToken)
        {
            if (!await ValidateAccessToken(accessToken)) return null;
            await _tokenReadLock.WaitAsync();
            try
            {
                string query = "SELECT user_id FROM tokens WHERE access_token = @accesstoken;";

                using var connection = CreateConnection();
                await connection.OpenAsync();

                await using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@accesstoken", accessToken);
                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return await GetUserById(reader.GetInt32("user_id"));
                        }
                    }
                }

                return null;
            }
            finally
            {
                _tokenReadLock.Release();
            }
        }

        public static async Task<User?> GetUserByRefreshToken(string refreshToken)
        {
            if (!await ValidateRefreshToken(refreshToken)) return null;

            await _tokenReadLock.WaitAsync();
            try
            {
                string query = "SELECT user_id FROM tokens WHERE refresh_token = @refreshtoken;";

                using var connection = CreateConnection();
                await connection.OpenAsync();

                await using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@refreshtoken", refreshToken);

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return await GetUserById(reader.GetInt32("user_id"));
                        }
                    }
                }

                return null;
            }
            finally
            {
                _tokenReadLock.Release();
            }
        }

        public static Role? GetRoleById(int id) 
        { 
            if (_roleCacheById.TryGetValue(id, out Role role))
            {
                return role;
            }
            return null;
        }
        public static Role? GetRoleByName(string name)
        {
            if (_roleCacheByName.TryGetValue(name, out Role role))
            {
                return role;
            }
            return null;
        }
        public static async Task<bool> SetRole(User user, string roleName)
        {
            if (!_roleCacheByName.TryGetValue(roleName, out var role))
                return false;

            user.RoleId = role.Id;
            return await ModifyUser(user) != null;
        }
        public static async Task<bool> BuildRoles()
        {
            await _roleReadLock.WaitAsync();
            
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();

                string query = "SELECT id, name FROM roles;";
                using var command = new MySqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var role = new Role
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                    };
                    _roleCacheById[role.Id] = role;
                    _roleCacheByName[role.Name] = role;
                }
                return true;
            }
            finally
            {
                _roleReadLock.Release();
            }
        }
        public static List<Role> GetRoles() 
        {
            return _roleCacheByName.Values.ToList();
        }
    }
}
