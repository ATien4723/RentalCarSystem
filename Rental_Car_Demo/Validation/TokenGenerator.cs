using Rental_Car_Demo.Context;
using Rental_Car_Demo.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Rental_Car_Demo.Validation
{
    public class TokenGenerator
    {
        public RentCarDbContext context = new RentCarDbContext();

        private const int TokenExpirationHours = 1;
        public string GenerateToken(int length)
        {
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            byte[] randomBytes = new byte[length];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }

            StringBuilder token = new StringBuilder(length);
            foreach (byte b in randomBytes)
            {
                token.Append(allowedChars[b % allowedChars.Length]);
            }

            return token.ToString();
        }

        public  DateTime GetExpirationTime()
        {
            return DateTime.UtcNow.AddHours(TokenExpirationHours+8);
        }

        public  string SHA256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Chuyển đổi chuỗi đầu vào thành mảng byte
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);

                // Tính toán giá trị băm
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // Chuyển đổi mảng byte thành chuỗi hex
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private  void isValidToken(string token)
        {
            var storedToken = context.TokenInfors.FirstOrDefault(t => t.Token == token);
            
            
        }


    }
}
