using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Utils
{
    public static class TokenUtils
    {
        // SHA256 base64
        public static string HashToken(string token)
        {
            if(token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // Generate cryptographically secure random token
        public static string GenerateRandomToken(int size = 64)
        {
            var bytes = RandomNumberGenerator.GetBytes(size);
            return Convert.ToBase64String(bytes);
        }
    }
}
