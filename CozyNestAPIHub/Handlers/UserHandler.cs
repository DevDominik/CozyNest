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
        private static readonly string SecretKey = "your-256-bit-secret";  // Secret key for signing JWTs
        private static readonly SemaphoreSlim _dbLock = new(1, 1); // Prevents race conditions in database operations
        private static readonly ConcurrentDictionary<int, User> _userCacheById = new();
        private static readonly ConcurrentDictionary<string, User> _userCacheByUsername = new();
        private static readonly ConcurrentDictionary<int, Role> _roleCacheById = new();
        private static readonly ConcurrentDictionary<string, Role> _roleCacheByName = new();

        public static void Initialize(string username, string password)
        {
            if (!string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("UserHandler is already initialized.");

            _connectionString = $"Server=localhost;Database=cozynest;User ID={username};Password={password};";
        }
        private static MySqlConnection CreateConnection() => new(_connectionString);
        public static async Task<List<User>> GetUsers()
        {
            var users = new List<User>();
            await _dbLock.WaitAsync();
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();
                string query = "SELECT id, username, email, address, hashed_password, first_name, last_name, closed, join_date, role_id FROM users;";
                using var command = new MySqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    User user = new User
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

                    // Store in cache
                    _userCacheById[user.Id] = user;
                    _userCacheByUsername[user.Username] = user;

                    users.Add(user);
                }
            }
            finally
            { _dbLock.Release(); }
            return users;

        }
        public static async Task<User?> GetUserById(int id)
        {
            if (_userCacheById.TryGetValue(id, out User cachedUser))
            {
                return cachedUser;
            }
            await _dbLock.WaitAsync();
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();
                string query = "SELECT id, username, email, address, hashed_password, first_name, last_name, closed, join_date, role_id FROM users WHERE id = @userId;";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", id);
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    User user = new User
                    {
                        Id = id,
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

                    // Store in cache
                    _userCacheById[id] = user;
                    _userCacheByUsername[user.Username] = user;

                    return user;
                }
            }
            finally 
            { _dbLock.Release(); }
            return null;
        }

        public static async Task<User?> GetUserByUsername(string username)
        {
            if (_userCacheByUsername.TryGetValue(username, out User cachedUser))
            {
                return cachedUser;
            }
            await _dbLock.WaitAsync();
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();
                string query = "SELECT id, username, email, address, hashed_password, first_name, last_name, closed, join_date, role_id FROM users WHERE username = @username;";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    User user = new User
                    {
                        Id = reader.GetInt32("id"),
                        Username = username,
                        Email = reader.GetString("email"),
                        Address = reader.GetString("address"),
                        HashedPassword = reader.GetString("hashed_password"),
                        FirstName = reader.GetString("first_name"),
                        LastName = reader.GetString("last_name"),
                        Closed = reader.GetBoolean("closed"),
                        JoinDate = reader.GetDateTime("join_date"),
                        RoleId = reader.GetInt32("role_id")
                    };

                    // Store in cache
                    _userCacheById[user.Id] = user;
                    _userCacheByUsername[username] = user;

                    return user;
                }
            }
            finally
            { _dbLock.Release(); }
            return null;
        }

        // Modify an existing user's details and update cache
        public static async Task<User?> ModifyUser(User user)
        {
            await _dbLock.WaitAsync();
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
                _dbLock.Release();
            }
            return null;
        }

        // Create a new user and cache it
        public static async Task<User?> CreateUser(User user)
        {
            await _dbLock.WaitAsync();
            try
            {
                if (await UserExists(user.Username, user.Email)) return null;

                string insertQuery = @"INSERT INTO users (username, email, address, hashed_password, first_name, last_name, join_date, role_id) 
                               VALUES (@username, @email, @address, @hashedpassword, @firstname, @lastname, @joindate, @roleid);";

                using var connection = CreateConnection();
                await connection.OpenAsync();

                await using (var command = new MySqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@username", user.Username);
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@address", user.Address);
                    command.Parameters.AddWithValue("@hashedpassword", user.HashedPassword);
                    command.Parameters.AddWithValue("@firstname", user.FirstName);
                    command.Parameters.AddWithValue("@lastname", user.LastName);
                    command.Parameters.AddWithValue("@joindate", DateTime.Now);
                    command.Parameters.AddWithValue("@roleid", user.RoleId);

                    await command.ExecuteNonQueryAsync();
                }

                var createdUser = await GetUserByUsername(user.Username);
                if (createdUser != null)
                {
                    _userCacheById[createdUser.Id] = createdUser;
                    _userCacheByUsername[createdUser.Username] = createdUser;
                }

                return createdUser;
            }
            finally
            {
                _dbLock.Release();
            }
        }

        // Check if a user exists by username or email
        public static async Task<bool> UserExists(string username, string email)
        {
            await _dbLock.WaitAsync();
            try
            {
                string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username OR email = @email;";

                using var connection = CreateConnection();
                await connection.OpenAsync();

                await using (var command = new MySqlCommand(checkQuery, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@email", email);
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result) > 0;
                }
            }
            finally
            {
                _dbLock.Release();
            }
        }

        // Helper function to generate a refresh token
        private static string GenerateToken()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] tokenData = new byte[32];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData);
            }
        }

        // Helper function to generate a JWT token (manually using HMACSHA256)
        private static string GenerateJwtToken(int userId, string username)
        {
            // JWT Header (Base64Url encode)
            var header = new
            {
                alg = "HS256",
                typ = "JWT"
            };

            var headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header));
            var base64UrlHeader = Base64UrlEncode(headerBytes);

            // JWT Payload (Base64Url encode)
            var payload = new
            {
                sub = username,
                jti = Guid.NewGuid().ToString(),
                userId = userId,
                exp = DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds()  // 30 minutes expiration
            };

            var payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));
            var base64UrlPayload = Base64UrlEncode(payloadBytes);

            // JWT Signature (HMACSHA256)
            var signatureInput = $"{base64UrlHeader}.{base64UrlPayload}";
            var keyBytes = Encoding.UTF8.GetBytes(SecretKey);
            using (var hmac = new HMACSHA256(keyBytes))
            {
                var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureInput));
                var base64UrlSignature = Base64UrlEncode(signatureBytes);

                // Final JWT token
                return $"{base64UrlHeader}.{base64UrlPayload}.{base64UrlSignature}";
            }
        }

        // Helper function to Base64Url encode
        private static string Base64UrlEncode(byte[] input)
        {
            var base64 = Convert.ToBase64String(input);
            base64 = base64.Split('=')[0];  // Remove trailing '=' characters
            base64 = base64.Replace('+', '-');  // Replace URL-unsafe characters
            base64 = base64.Replace('/', '_');  // Replace URL-unsafe characters
            return base64;
        }

        // Create a new token for a user
        public static async Task<Token?> CreateToken(int userId, string username)
        {
            await _dbLock.WaitAsync();
            try
            {
                string accessToken = GenerateJwtToken(userId, username);  // Generate JWT for access token
                string refreshToken = GenerateToken();  // Generate refresh token
                DateTime accessExpiry = DateTime.UtcNow.AddMinutes(30);  // 30 minutes for access token
                DateTime refreshExpiry = DateTime.UtcNow.AddDays(7);    // 7 days for refresh token

                string insertTokenQuery = @"INSERT INTO tokens (user_id, access_token, refresh_token, access_expiry, refresh_expiry, is_active) 
                                    VALUES (@userId, @accessToken, @refreshToken, @accessExpiry, @refreshExpiry, @isActive);";

                using var connection = CreateConnection();
                await connection.OpenAsync();

                await using (var command = new MySqlCommand(insertTokenQuery, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
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
                        UserId = userId,
                        IsActive = true
                    };
                }
            }
            finally
            {
                _dbLock.Release();
            }
        }

        // Validate access token
        public static async Task<bool> ValidateAccessToken(string accessToken)
        {
            await _dbLock.WaitAsync();
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

                return false;  // Token is invalid
            }
            finally
            {
                _dbLock.Release();
            }
        }

        // Validate refresh token
        public static async Task<bool> ValidateRefreshToken(string refreshToken)
        {
            await _dbLock.WaitAsync();
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

                return false;  // Token is invalid
            }
            finally
            {
                _dbLock.Release();
            }
        }

        // Revoke tokens (set them as inactive)
        public static async Task<bool> RevokeToken(string accessToken, string refreshToken)
        {
            await _dbLock.WaitAsync();
            try
            {
                string revokeQuery = "UPDATE tokens SET is_active = false WHERE access_token = @accessToken OR refresh_token = @refreshToken;";

                using var connection = CreateConnection();
                await connection.OpenAsync();

                await using (var command = new MySqlCommand(revokeQuery, connection))
                {
                    command.Parameters.AddWithValue("@accessToken", accessToken);
                    command.Parameters.AddWithValue("@refreshToken", refreshToken);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;  // Return true if tokens were successfully revoked
                }
            }
            finally
            {
                _dbLock.Release();
            }
        }

        // Get user ID by access token
        public static async Task<int?> GetUserIdByAccessToken(string accessToken)
        {
            if (!await ValidateAccessToken(accessToken)) return null;
            await _dbLock.WaitAsync();
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
                            return reader.GetInt32("user_id");
                        }
                    }
                }

                return null;
            }
            finally
            {
                _dbLock.Release();
            }
        }

        // Get user ID by refresh token
        public static async Task<int?> GetUserIdByRefreshToken(string refreshToken)
        {
            if (!await ValidateRefreshToken(refreshToken)) return null;
            await _dbLock.WaitAsync();
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
                            return reader.GetInt32("user_id");
                        }
                    }
                }

                return null;
            }
            finally
            {
                _dbLock.Release();
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
            await _dbLock.WaitAsync();
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
                _dbLock.Release();
            }
        }
        public static List<Role> GetRoles() 
        {
            return _roleCacheByName.Values.ToList();
        }
    }
}
