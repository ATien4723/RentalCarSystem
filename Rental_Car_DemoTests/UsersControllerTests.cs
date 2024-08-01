using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Rental_Car_Demo.Controllers;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Services;
using Rental_Car_Demo.ViewModel;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Rental_Car_Demo.Context;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Http;

namespace Rental_Car_Demo.UserControllerTests
{
    [TestFixture]
    public class UsersControllerTests
    {
        private UsersController _controller;
        private RentCarDbContext _context;
        private Mock<IEmailService> _mockEmailService;
        private Mock<ITempDataDictionary> _mockTempData;
        private Mock<ITokenGenerator> _mockTokenGenerator;
        private Mock<ICustomerContext> _mockCustomerContext;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<RentCarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockEmailService = new Mock<IEmailService>();
            _mockTokenGenerator = new Mock<ITokenGenerator>();
            _mockCustomerContext = new Mock<ICustomerContext>();
            _mockTempData = new Mock<ITempDataDictionary>();

            _context = new RentCarDbContext(options);



            _controller = new UsersController(_context, _mockCustomerContext.Object, _mockTokenGenerator.Object, _mockEmailService.Object)
            {
                TempData = _mockTempData.Object
            };
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
                Register = new RegisterViewModel { Email = "hehe123@gmail.com", Password = "hehe123", ConfirmPassword = "hehe123", Name = "hehe123", Phone = "0987654321", Role = "rentCar", AgreeToTerms = true }
            };

            // Act
            var result = _controller.Register(model) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Guest", result.ActionName);
            Assert.AreEqual("Users", result.ControllerName);
        }



        [Test]
        public void ResetPassword_ExistingEmail_SendsResetLink()
        {
            // Arrange
            var model = new ResetPasswordViewModel
            {
                Email = "nvutuankiet2003@gmail.com"
            };

            _mockTokenGenerator.Setup(tg => tg.GenerateToken(It.IsAny<int>())).Returns("dummyToken");
            _mockTokenGenerator.Setup(tg => tg.GetExpirationTime()).Returns(DateTime.Now.AddHours(1));
            _mockCustomerContext.Setup(cc => cc.getCustomerIdByEmail(model.Email)).Returns(1);

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns("https://example.com/resetpassword");

            _controller.Url = mockUrlHelper.Object;

            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Scheme = "https";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext
            };

            _mockTempData.Setup(td => td["SuccessMessage"]).Returns<string>(key => key == "SuccessMessage" ? "We will send link to reset your password in the email!" : null);

            // Act
            var result = _controller.ResetPassword(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("We will send link to reset your password in the email!", _controller.TempData["SuccessMessage"]);
        }

        [Test]
        public void ResetPassword_NonExistingEmail_ShowsErrorMessage()
        {
            // Arrange
            var model = new ResetPasswordViewModel
            {
                Email = "nonexistentemail@example.com"
            };

            _mockCustomerContext.Setup(cc => cc.getCustomerIdByEmail(model.Email)).Returns(-1);

            _mockTempData.Setup(td => td.ContainsKey(It.IsAny<string>())).Returns<string>(key => key == "FailMessage");
            _mockTempData.Setup(td => td["FailMessage"]).Returns<string>(key => key == "FailMessage" ? "Sorry, Your email does not exist in our database!" : null);

            // Act
            var result = _controller.ResetPassword(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "The result should not be null");
            Assert.IsTrue(_controller.TempData.ContainsKey("FailMessage"), "TempData should contain FailMessage");
            Assert.AreEqual("Sorry, Your email does not exist in our database!", _controller.TempData["FailMessage"], "The FailMessage in TempData is incorrect");

        }

        [Test]
        [TestCase("validToken", false, false, "ResetPassword2ViewModel", 1)]
        [TestCase("expiredToken", true, false, "Fail", 1)]
        [TestCase("lockedToken", false, true, "Fail", 1)]
        public void ResetPassword2_TokenValidationTests(string tokenValue, bool isExpired, bool isLocked, string expectedViewName, int expectedCustomerId)
        {
            // Arrange
            var customerId = 1;

            var token = new TokenInfor
            {
                Token = tokenValue,
                UserId = customerId,
                ExpirationTime = isExpired ? DateTime.Now.AddHours(-1) : DateTime.Now.AddHours(1),
                IsLocked = isLocked
            };

            _context.TokenInfors.Add(token);
            _context.SaveChanges();

            // Act
            var result = _controller.ResetPassword2(customerId, tokenValue) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "The result should not be null");
            Assert.AreEqual(expectedViewName, result.ViewName, "The view name is incorrect");

            if (expectedViewName == "ResetPassword2ViewModel")
            {
                Assert.IsInstanceOf<ResetPassword2ViewModel>(result.Model, "The model should be of type ResetPassword2ViewModel");
                Assert.AreEqual(expectedCustomerId, ((ResetPassword2ViewModel)result.Model).CustomerId, "The CustomerId in the model should match");
            }
        }


        [Test]
        public void ResetPassword2_ValidModel_ResetsPasswordAndReturnsLoginView()
        {
            // Arrange
            var model = new ResetPassword2ViewModel
            {
                CustomerId = 1,
                Password = "newPassword123"
            };

            // Act
            var result = _controller.ResetPassword2(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "The result should not be null");
            Assert.AreEqual("Login", result.ViewName, "The view name should be 'Login'");

            var user = _context.Users.FirstOrDefault(u => u.UserId == model.CustomerId);

            Assert.IsNotNull(user, "User should not be null");
            Assert.AreEqual(HashPassword(model.Password), user.Password, "The user's password should be updated");

            _mockTempData.Setup(td => td["SuccessMessage"]).Returns<string>(key => key == "SuccessMessage" ? "Your password has been reset" : null);
            Assert.AreEqual("Your password has been reset", _controller.TempData["SuccessMessage"], "The success message should be set");
        }

        [Test]
        public void ResetPassword2_InvalidModel_ReturnsModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("Password", "Password is required");
            var model = new ResetPassword2ViewModel
            {
                CustomerId = 1,
                Password = "" // Invalid model data
            };

            // Act
            var result = _controller.ResetPassword2(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "The result should not be null");
            Assert.AreEqual(model, result.Model, "The model should be returned with errors");
        }
    }
}
