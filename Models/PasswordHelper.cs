using System.Security.Cryptography;
using System.Text;

namespace AuthenticationWebApplication.Models
{
    public static class PasswordHelper
    {
        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(storedSalt));
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var computedHashString = Convert.ToBase64String(computedHash);
            return computedHashString == storedHash;
        }
    }
}