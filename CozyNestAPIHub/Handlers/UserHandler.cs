using CozyNestAPIHub.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;

namespace CozyNestAPIHub.Handlers
{
    public class UserHandler
    {
        static UserHandler instance;
        static MySqlConnection connection;
        private static readonly string SecretKey = "your-256-bit-secret";  // Secret key for signing JWTs

        // In-memory cache for users
        private static readonly ConcurrentDictionary<int, User> _userCacheById = new();
        private static readonly ConcurrentDictionary<string, User> _userCacheByUsername = new();
        private static readonly ConcurrentDictionary<int, Role> _roleCacheById = new();
        private static readonly ConcurrentDictionary<string, Role> _roleCacheByName = new();

        // Constructor to initialize the connection
        private UserHandler(string name, string password)
        {
            connection = new MySqlConnection($"Server=localhost;Database=cozynest;User ID={name};Password={password};");
            connection.Open();
        }

        // Destructor to close the connection
        ~UserHandler() { connection.Close(); }

        // Initialize singleton instance of UserHandler
        public static void Initialize(string name, string password)
        {
            if (instance != null) { throw new InvalidOperationException("There's already a UserHandler instance running."); }
            instance = new UserHandler(name, password);
            
        }

        // Property to check if the connection is valid
        static bool IsConnectionValid => connection != null && instance != null;

        // Get a user by ID with caching
        public static async Task<User?> GetUserById(int id)
        {
            if (!IsConnectionValid) return null;

            // Check the cache first
            if (_userCacheById.TryGetValue(id, out User cachedUser))
            {
                return cachedUser;
            }

            // Query database if not in cache
            string query = "SELECT id, username, email, address, hashed_password, first_name, last_name, closed, join_date, role_id FROM users WHERE id = @userId;";
            await using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", id);
                await using (var reader = await command.ExecuteReaderAsync())
                {
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
            }
            return null;
        }

        // Get a user by username with caching
        public static async Task<User?> GetUserByUsername(string username)
        {
            if (!IsConnectionValid) return null;

            // Check cache first
            if (_userCacheByUsername.TryGetValue(username, out User cachedUser))
            {
                return cachedUser;
            }

            // Query database if not in cache
            string getUserQuery = "SELECT id, username, email, address, hashed_password, first_name, last_name, closed, join_date, role_id FROM users WHERE username = @username;";
            await using (var command = new MySqlCommand(getUserQuery, connection))
            {
                command.Parameters.AddWithValue("@username", username);

                await using (var reader = await command.ExecuteReaderAsync())
                {
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
                        _userCacheByUsername[user.Username] = user;

                        return user;
                    }
                }
            }
            return null;
        }

        // Modify an existing user's details and update cache
        public static async Task<User?> ModifyUser(User user)
        {
            if (!IsConnectionValid) return null;

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

            await using (var command = new MySqlCommand(updateQuery, connection))
            {
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
                    // Update the cache
                    _userCacheById[user.Id] = user;
                    _userCacheByUsername[user.Username] = user;
                    return user;
                }
            }
            return null;
        }

        // Create a new user and cache it
        public static async Task<User?> CreateUser(User user)
        {
            if (!IsConnectionValid) return null;
            if (await UserExists(user.Username, user.Email)) return null;

            string insertQuery = @"INSERT INTO users (username, email, address, hashed_password, first_name, last_name, join_date, role_id) 
                                   VALUES (@username, @email, @address, @hashedpassword, @firstname, @lastname, @joindate, @roleid);";

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
                var createdUser = await GetUserByUsername(user.Username);

                if (createdUser != null)
                {
                    _userCacheById[createdUser.Id] = createdUser;
                    _userCacheByUsername[createdUser.Username] = createdUser;
                }
                return createdUser;
            }
        }

        // Check if a user exists by username or email
        public static async Task<bool> UserExists(string username, string email)
        {
            if (!IsConnectionValid) return false;

            string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username OR email = @email;";
            await using (var command = new MySqlCommand(checkQuery, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@email", email);
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
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

        // Token Management - Create a new Token (Access and Refresh Tokens)
        public static async Task<Token?> CreateToken(int userId, string username)
        {
            if (!IsConnectionValid) { return null; }

            string accessToken = GenerateJwtToken(userId, username);  // Generate JWT for access token
            string refreshToken = GenerateToken();  // Generate refresh token
            DateTime accessExpiry = DateTime.UtcNow.AddMinutes(30);  // 30 minutes for access token
            DateTime refreshExpiry = DateTime.UtcNow.AddDays(7);    // 7 days for refresh token

            string insertTokenQuery = @"INSERT INTO tokens (user_id, access_token, refresh_token, access_expiry, refresh_expiry, is_active) 
                                        VALUES (@userId, @accessToken, @refreshToken, @accessExpiry, @refreshExpiry, @isActive);";

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

        // Validate a user's access token (manually decoding and validating the JWT)
        public static bool ValidateAccessToken(string accessToken)
        {
            try
            {
                var parts = accessToken.Split('.');
                if (parts.Length != 3) return false;  // Ensure we have 3 parts: header, payload, and signature

                var header = Base64UrlDecode(parts[0]);
                var payload = Base64UrlDecode(parts[1]);
                var signature = Base64UrlDecode(parts[2]);

                // Decode the payload to check the expiration and other claims
                var payloadJson = Encoding.UTF8.GetString(payload);
                var payloadData = JsonConvert.DeserializeObject<Dictionary<string, object>>(payloadJson);
                var exp = Convert.ToInt64(payloadData["exp"]);
                if (exp < DateTimeOffset.UtcNow.ToUnixTimeSeconds()) return false;  // Token has expired

                // Recompute the signature using the same key and compare it to the one in the token
                var keyBytes = Encoding.UTF8.GetBytes(SecretKey);
                var hmac = new HMACSHA256(keyBytes);
                var signatureInput = $"{parts[0]}.{parts[1]}";
                var computedSignature = hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureInput));

                if (!signature.SequenceEqual(computedSignature)) return false;  // Signature mismatch

                // If we pass all checks, the token is valid
                return true;
            }
            catch (Exception)
            {
                return false;  // Token is invalid
            }
        }

        // Helper function to Base64Url decode
        private static byte[] Base64UrlDecode(string input)
        {
            var base64 = input.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)  // Add padding if necessary
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        // Validate a user's refresh token
        public static async Task<bool> ValidateRefreshToken(string refreshToken)
        {
            if (!IsConnectionValid) { return false; }

            string checkQuery = "SELECT is_active, refresh_expiry FROM tokens WHERE refresh_token = @refreshToken;";

            await using (var command = new MySqlCommand(checkQuery, connection))
            {
                command.Parameters.AddWithValue("@refreshToken", refreshToken);

                await using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        bool isActive = reader.GetBoolean("is_active");
                        DateTime refreshExpiry = reader.GetDateTime("refresh_expiry");

                        // Check if the token is active and has not expired
                        return isActive && refreshExpiry > DateTime.UtcNow;
                    }
                }
            }

            return false;  // Token is invalid
        }

        // Revoke a user's token (both access and refresh tokens)
        public static async Task<bool> RevokeToken(string accessToken, string refreshToken)
        {
            if (!IsConnectionValid) { return false; }

            string revokeQuery = "UPDATE tokens SET is_active = false WHERE access_token = @accessToken OR refresh_token = @refreshToken;";

            await using (var command = new MySqlCommand(revokeQuery, connection))
            {
                command.Parameters.AddWithValue("@accessToken", accessToken);
                command.Parameters.AddWithValue("@refreshToken", refreshToken);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;  // Return true if tokens were successfully revoked
            }
        }

        public static async Task<int?> GetUserIdByAccessToken(string accessToken) 
        { 
            if (!IsConnectionValid) { return null; }
            string query = "SELECT user_id, is_active FROM tokens WHERE access_token = @accesstoken;";
            await using (var command = new MySqlCommand(query, connection)) 
            {
                command.Parameters.AddWithValue("@accesstoken", accessToken);
                await using (var reader = await command.ExecuteReaderAsync()) 
                {
                    if (await reader.ReadAsync())
                    {
                        if (reader.GetBoolean("is_active"))
                        {
                            return reader.GetInt32("user_id");
                        }
                    }
                }
            }
            return null;
        }

        public static async Task<int?> GetUserIdByRefreshToken(string refreshToken)
        {
            if (!IsConnectionValid) { return null; }
            string query = "SELECT user_id, is_active FROM tokens WHERE refresh_token = @refreshtoken;";
            await using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@refreshtoken", refreshToken);
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        if (reader.GetBoolean("is_active"))
                        {
                            return reader.GetInt32("user_id");
                        }
                    }
                }
            }
            return null;
        }

        public static Role? GetRoleById(int id) 
        { 
            if (!IsConnectionValid) { return null; }
            if (_roleCacheById.TryGetValue(id, out Role role))
            {
                return role;
            }
            return null;
        }
        public static Role? GetRoleByName(string name)
        {
            if (!IsConnectionValid) { return null; }
            if (_roleCacheByName.TryGetValue(name, out Role role))
            {
                return role;
            }
            return null;
        }
        public static async Task<bool> SetRole(User user, string roleName) 
        {
            if (!IsConnectionValid) { return false; }
            if (await UserExists(user.Username, user.Email) == false) {  return false; }
            if (string.IsNullOrEmpty(roleName)) { return false; }
            Role? role = GetRoleByName(roleName);
            if (role == null) { return false; }
            user.RoleId = role.Id;
            return await ModifyUser(user) != null;
        }
        public static async Task<bool> BuildRoles() 
        {
            if (!IsConnectionValid) { return false; }
            string query = "SELECT id, name FROM roles;";
            await using (var command = new MySqlCommand(query, connection)) 
            { 
                await using (var reader = await command.ExecuteReaderAsync()) 
                {
                    if (await reader.ReadAsync())
                    {
                        Role role = new Role
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                        };
                        _roleCacheById[role.Id] = role;
                        _roleCacheByName[role.Name] = role;
                        return true;
                    }
                }
            }
            return false;
        }
        public static List<Role> GetRoles() 
        {
            return _roleCacheByName.Values.ToList();
        }
    }
}
