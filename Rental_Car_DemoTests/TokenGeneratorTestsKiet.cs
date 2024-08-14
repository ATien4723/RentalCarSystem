using System;
using NUnit.Framework;
using Rental_Car_Demo.Services;

namespace Rental_Car_Demo.Tests.Services
{
    [TestFixture]
    public class TokenGeneratorTestsKiet
    {
        private TokenGenerator _tokenGenerator;

        [SetUp]
        public void SetUp()
        {
            _tokenGenerator = new TokenGenerator();
        }

        [Test]
        public void GenerateToken_ShouldReturnTokenOfGivenLength()
        {
            // Arrange
            int length = 16;

            // Act
            string token = _tokenGenerator.GenerateToken(length);

            // Assert
            Assert.IsNotNull(token);
            Assert.AreEqual(length, token.Length);
        }

        [Test]
        public void GenerateToken_ShouldReturnUniqueTokens()
        {
            // Arrange
            int length = 16;

            // Act
            string token1 = _tokenGenerator.GenerateToken(length);
            string token2 = _tokenGenerator.GenerateToken(length);

            // Assert
            Assert.AreNotEqual(token1, token2);
        }

        [Test]
        public void GetExpirationTime_ShouldReturnCorrectExpirationTime()
        {
            // Arrange
            DateTime beforeExpiration = DateTime.UtcNow.AddHours(TokenGenerator.TokenExpirationHours + 8 - 1);
            DateTime afterExpiration = DateTime.UtcNow.AddHours(TokenGenerator.TokenExpirationHours + 8 + 1);

            // Act
            DateTime expirationTime = _tokenGenerator.GetExpirationTime();

            // Assert
            Assert.GreaterOrEqual(expirationTime, beforeExpiration);
            Assert.LessOrEqual(expirationTime, afterExpiration);
        }
    }
}