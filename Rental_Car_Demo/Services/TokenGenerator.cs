using Rental_Car_Demo.Context;
using Rental_Car_Demo.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Rental_Car_Demo.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        public RentCarDbContext context = new RentCarDbContext();

        public const int TokenExpirationHours = 1;
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

        public DateTime GetExpirationTime()
        {
            return DateTime.UtcNow.AddHours(TokenExpirationHours + 8);
        }

    }
}
