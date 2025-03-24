global using static CozyNestAPIHub.GlobalMethods;
using System.Collections.Concurrent;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace CozyNestAPIHub
{
    public class GlobalMethods
    {
        public static async Task<T> GetItemFromContext<T>(HttpContext context, string itemName) where T : class
        {
            if (!context.Items.TryGetValue(itemName, out var item))
            {
                throw new InvalidOperationException($"Item '{itemName}' not found in the context of this request.");
            }
            return await Task.FromResult(item as T);
        }

        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = 4;
                argon2.MemorySize = 65536;
                argon2.Iterations = 3;

                byte[] hash = argon2.GetBytes(32);
                return Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash);
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            string[] parts = hashedPassword.Split('$');
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHash = Convert.FromBase64String(parts[1]);

            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = 4;
                argon2.MemorySize = 65536;
                argon2.Iterations = 3;

                byte[] computedHash = argon2.GetBytes(32);
                return storedHash.SequenceEqual(computedHash);
            }
        }

        public static string GenerateRandomPassword()
        {
            Random random = new Random();
            string resetPassword = "";
            for (int i = 0; i < 8; i++)
            {
                resetPassword = resetPassword + random.Next(1, 10).ToString();
            }
            return resetPassword;
        }
    }
}
