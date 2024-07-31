using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Rental_Car_Demo.Controllers;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Validation;
using Rental_Car_Demo.ViewModel;
using System.Security.Cryptography;

namespace Rental_Car_Demo.UnitTests
{
    [TestFixture]
    public class UsersControllerTests
    {
        private UsersController _controller;
        private RentCarDbContext _context;
        private Mock<IEmailService> _mockEmailService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<RentCarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new RentCarDbContext(options);
            _controller = new UsersController(_context);
            SeedDatabase();
            
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _controller.Dispose();
        }

        private void SeedDatabase()
        {
            var users = new List<User>
            {
                new User
                {
                    Email = "nvutuankiet2003@gmail.com",
                    Password = HashPassword("kiet123"),
                    Name = "kiet ne",
                    Phone = "0334567890",
                    Role = false,
                    Wallet = 0
                },
                new User
                {
                    Email = "hehe@gmail.com",
                    Password = HashPassword("hehe123"),
                    Name = "hehe",
                    Phone = "0987654321",
                    Role = true,
                    Wallet = 0
                }
            };

            _context.Users.AddRange(users);
            _context.SaveChanges();
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        [Test]
        [TestCase("nvutuankiet2003@gmail.com", true)]
        [TestCase("hehe123@gmail.com", false)]
        public void IsEmailExist_ShouldReturnExpectedResult(string email, bool expectedResult)
        {
            // Act
            var result = _controller.IsEmailExist(email);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Register_EmailAlreadyExists_ShouldAddModelError()
        {
            // Arrange
            var model = new RegisterAndLoginViewModel
            {
                Register = new RegisterViewModel { Email = "nvutuankiet2003@gmail.com", Password = "kiet123", ConfirmPassword="kiet123", Name="kiet ne", Phone= "0334567890",Role= "carOwner", AgreeToTerms = true }
            };

            // Act
            var result = _controller.Register(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Email already existed. Please try another email.", result.ViewData.ModelState["Register.Email"].Errors[0].ErrorMessage);
        }

        [Test]
        public void Register_AgreeToTermsIsFalse_ShouldAddModelError()
        {
            // Arrange
            var model = new RegisterAndLoginViewModel
            {
                Register = new RegisterViewModel { Email = "hehehe@gmail.com", Password = "hehe", ConfirmPassword = "hehe123", Name = "hehe", Phone = "0987654321", Role = "rentCar", AgreeToTerms = false }
            };

            // Act
            var result = _controller.Register(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Please agree to this!", result.ViewData.ModelState["Register.AgreeToTerms"].Errors[0].ErrorMessage);
        }

        [Test]
        public void Register_ValidModel_ShouldRedirectToGuest()
        {
            // Arrange
            var model = new RegisterAndLoginViewModel
            {
                Register = new RegisterViewModel { Email = "hehe123@gmail.com", Password = "hehe123", ConfirmPassword = "hehe123", Name = "hehe123", Phone = "0987654321", Role = "rentCar", AgreeToTerms = false }
            };

            // Act
            var result = _controller.Register(model) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Guest", result.ActionName);
        }
    }
}
