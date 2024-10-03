using System.Security.Cryptography;

namespace HotelManagementFinalDemoApi.Helpers
{
    public class PasswordHash
    {
        public static string HashPassword(string Password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        public static string GenerateSecureToken(int size = 32)
        {
            var tokenBytes = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }

            return Convert.ToBase64String(tokenBytes);
        }
    }
}
