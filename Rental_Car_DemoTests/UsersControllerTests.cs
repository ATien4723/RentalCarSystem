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

namespace Rental_Car_Demo.Tests
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
        public void Register_ReturnsViewResult()
        {
            // Act
            var result = _controller.Register();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void ResetPass_ReturnsViewResult()
        {
            // Act
            var result = _controller.ResetPassword();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
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
        [TestCase("kietdzne@gmail.com", "kiet123", "kiet123", "kiettttttttttttttttttttttttttttttttttttttttttttttt", "0326713614", "carOwner", true, Description = "1. Name exceed 50 char")]
        [TestCase("kietdzne@gmail.com", "kiet123", "kiet123", "", "0326713614", "carOwner", true, Description = "2. Name is empty")]

        [TestCase("kietdzne@gmail.com", "", "", "hehe", "0904182483", "rentCar", true, Description = "3. Password is empty")]
        [TestCase("kietdzne@gmail.com", "hehe12", "hehe12", "hehe", "0904182483", "rentCar", true, Description = "4. Password not have at least 7 char")]
        [TestCase("kietdzne@gmail.com", "hehehehe", "hehehehe", "hehe", "0904182483", "rentCar", true, Description = "5. Password not have number")]
        [TestCase("kietdzne@gmail.com", "hehe", "hehe", "hehe", "0904182483", "rentCar", true, Description = "6. Password not have number and not have at least 7 char")]
        [TestCase("kietdzne@gmail.com", "11111111", "11111111", "hehe", "0904182483", "rentCar", true, Description = "7. Password not have letter")]
        [TestCase("kietdzne@gmail.com", "1111", "1111", "hehe", "0904182483", "rentCar", true, Description = "8. Password not have letter and not have at least 7 char")]
        [TestCase("kietdzne@gmail.com", "@@@@@@@@", "@@@@@@@@", "hehe", "0904182483", "rentCar", true, Description = "9. Password not have letter and number")]
        [TestCase("kietdzne@gmail.com", "@@@@", "@@@@", "hehe", "0904182483", "rentCar", true, Description = "10. Password not have letter and number and not have at least 7 char")]

        [TestCase("kietdzne@gmail.com", "hehehe123", "", "hehe", "0904182483", "rentCar", true, Description = "11. Confirm password is empty")]
        [TestCase("kietdzne@gmail.com", "hehehe123", "abcxyz123", "hehe", "0904182483", "rentCar", true, Description = "12. Confirm password and password not match")]

        [TestCase("", "kiet123", "kiet123", "kiet", "0326713614", "carOwner", true, Description = "13. Email is empty")]
        [TestCase("plainaddress", "kiet123", "kiet123", "kiet", "0326713614", "carOwner", true, Description = "14. Email without domain")]
        [TestCase("@missingusername.com", "kiet123", "kiet123", "kiet", "0326713614", "carOwner", true, Description = "15. Email missing username")]
        [TestCase("missingatsign.com", "kiet123", "kiet123", "kiet", "0326713614", "carOwner", true, Description = "16. Email missing @ sign")]
        [TestCase("missingdomain@.com", "kiet123", "kiet123", "kiet", "0326713614", "carOwner", true, Description = "17. Email missing domain name")]
        [TestCase("missingdot@domaincom", "kiet123", "kiet123", "kiet", "0326713614", "carOwner", true, Description = "18. Email missing dot in domain")]
        [TestCase("missingtld@domain.", "kiet123", "kiet123", "kiet", "0326713614", "carOwner", true, Description = "19. Email missing top-level domain")]
        [TestCase("missingsymbol@domain..com", "kiet123", "kiet123", "kiet", "0326713614", "carOwner", true, Description = "20. Email with double dot in domain")]
        [TestCase("contains spaces@domain.com", "kiet123", "kiet123", "kiet", "0326713614", "carOwner", true, Description = "21. Email with spaces")]
        [TestCase("specialchars!#$%&'*+/=?^_`{|}~@gmail.com", "kiet123", "kiet123", "kiet", "0326713614", "carOwner", true, Description = "22. Email with invalid special characters")]

        [TestCase("kietdzne@gmail.com", "kiet123", "kiet123", "kiet", "", "carOwner", true, Description = "23. Phone is empty")]
        [TestCase("kietdzne@gmail.com", "kiet123", "kiet123", "kiet", "001203019", "carOwner", true, Description = "24. Phone start with 00")]
        [TestCase("kietdzne@gmail.com", "kiet123", "kiet123", "kiet", "011203019", "carOwner", true, Description = "25. Phone start with 01")]
        [TestCase("kietdzne@gmail.com", "kiet123", "kiet123", "kiet", "021203019", "carOwner", true, Description = "26. Phone start with 02")]
        [TestCase("kietdzne@gmail.com", "kiet123", "kiet123", "kiet", "041203019", "carOwner", true, Description = "27. Phone start with 04")]
        [TestCase("kietdzne@gmail.com", "kiet123", "kiet123", "kiet", "061203019", "carOwner", true, Description = "28. Phone start with 06")]
        [TestCase("kietdzne@gmail.com", "kiet123", "kiet123", "kiet", "03120301", "carOwner", true, Description = "29. Phone is less than 10 number")]
        [TestCase("kietdzne@gmail.com", "kiet123", "kiet123", "kiet", "0312030111", "carOwner", true, Description = "30. Phone is more than 10 number")]
        public void Register_InvalidModel_ModelError_CannotSubmitForm(string email, string password, string confirmPassword,string name, string phone, string role, bool agreeTerm)
        {
            // Arrange
            var model = new RegisterAndLoginViewModel
            {
                Register = new RegisterViewModel { Email = email, Password = password, ConfirmPassword = confirmPassword, Name = name, Phone = phone, Role = role, AgreeToTerms = agreeTerm }
            };

            // Act
            var result = _controller.Register(model) as ViewResult;

            // Assert
            Assert.IsNull(result);
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
        [TestCase("nvutuankiet2003@gmail.com", "token01", 1, true, Description = "1. Email existing")]
        [TestCase("abc@gmail.com", "token02", -1, true, Description = "2. Email not existing")]

        [TestCase("", "token01", 1, false, Description = "3. Email is empty")]
        [TestCase("plainaddress", "token01", -1, false, Description = "4. Email without domain")]
        [TestCase("@missingusername.com", "token01", -1, false, Description = "5. Email missing username")]
        [TestCase("missingatsign.com", "token01", -1, false, Description = "6. Email missing @ sign")]
        [TestCase("missingdomain@.com", "token01", -1, false, Description = "7. Email missing domain name")]
        [TestCase("missingdot@domaincom", "token01", -1, false, Description = "8. Email missing dot in domain")]
        [TestCase("missingtld@domain.", "token01", -1, false, Description = "9. Email missing top-level domain")]
        [TestCase("missingsymbol@domain..com", "token01", -1, false, Description = "10. Email with double dot in domain")]
        [TestCase("contains spaces@domain.com", "token01", -1, false, Description = "11. Email with spaces")]
        [TestCase("specialchars!#$%&'*+/=?^_`{|}~@gmail.com", "token01", -1, false, Description = "12. Email with invalid special characters")]
        public void ResetPassword_Post_Check_Email(string email, string token, int user, bool emailValid)
        {
            // Arrange
            var model = new ResetPasswordViewModel
            {
                Email = email
            };

            _mockTokenGenerator.Setup(tg => tg.GenerateToken(It.IsAny<int>())).Returns(token);
            _mockTokenGenerator.Setup(tg => tg.GetExpirationTime()).Returns(DateTime.Now.AddHours(1));
            _mockCustomerContext.Setup(cc => cc.getCustomerIdByEmail(model.Email)).Returns(user);

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
            _mockTempData.Setup(td => td["FailMessage"]).Returns<string>(key => key == "FailMessage" ? "Sorry, Your email does not exist in our database!" : null);

            

            if (!emailValid)
            {
                _controller.ModelState.AddModelError("Email", "Invalid Email");
                // Act
                var result = _controller.ResetPassword(model) as ViewResult;

                Assert.IsNotNull(result);
                Assert.AreEqual(model, result.Model);
            }
            else if (user == 1)
            {
                // Act
                var result = _controller.ResetPassword(model) as ViewResult;
                Assert.IsNotNull(result);
                Assert.AreEqual("We will send link to reset your password in the email!", _controller.TempData["SuccessMessage"]);
            }
            else if (user == -1)
            {
                // Act
                var result = _controller.ResetPassword(model) as ViewResult;
                Assert.IsNotNull(result);
                Assert.AreEqual("Sorry, Your email does not exist in our database!", _controller.TempData["FailMessage"]);
            }
        }

        [Test]
        [TestCase("tokenne", "tokenne", false, false, 1, null, 1)]
        [TestCase("tokenne", "wrongtoken", false, false, 1, "Fail", 1)]
        [TestCase("expiredToken", "expiredToken", true, false, 1, "Fail", 1)]
        [TestCase("lockedToken", "lockedToken", false, true, 1, "Fail", 1)]
        [TestCase("lockedAndExToken", "lockedAndExToken", true, true, 2, "Fail", 1)]
        [TestCase("tokenne", "tokenne", false, false, 2, "Fail", 1)]
        [TestCase("tokenne", "wrongtoken", false, false, 2, "Fail", 1)]
        [TestCase("expiredToken", "expiredToken", true, false, 2, "Fail", 1)]
        [TestCase("lockedToken", "lockedToken", false, true, 2, "Fail", 1)]

        public void ResetPassword2_TokenValidationTests(string tokenInDatabase, string tokenValue, bool isExpired, bool isLocked, int customerId, string? expectedViewName, int expectedCustomerId)
        {
            // Arrange
            var userIdOftoken = 1;

            var token = new TokenInfor
            {
                Token = tokenInDatabase,
                UserId = userIdOftoken,
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

            if (expectedViewName == null)
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
                Password = "newPassword123",
                ConfirmPassword = "newPassword123"

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
        [TestCase("", "pass01", "Password", Description = "1. Password is empty")]
        [TestCase("less7", "less7", "Password", Description = "2. Password not have at least 7 char")]
        [TestCase("nothavenum", "nothavenum", "Password", Description = "3. Password not have number")]
        [TestCase("noth", "noth", "Password", Description = "4. Password not have number and not have at least 7 char")]
        [TestCase("1234567", "1234567", "Password", Description = "5. Password not have letter")]
        [TestCase("1234", "1234", "Password", Description = "6. Password not have letter and less than 7 char")]
        [TestCase("@@@@@@@@", "@@@@@@@@", "Password", Description = "7. Password not have number and letter")]
        [TestCase("@@@", "@@@", "Password", Description = "8. Password not have number and letter and less than 7 char")]

        [TestCase("hehehehe123", "", "ConfirmPassword", Description = "9. ConfirmPassword empty")]
        [TestCase("hehehehe123", "abcxyz123", "ConfirmPassword", Description = "9. ConfirmPassword not match")]

        public void ResetPassword2_InvalidModel_ReturnsModel(string pass, string confirmPass, string modelError)
        {
            // Arrange
            _controller.ModelState.AddModelError(modelError, "Invalid");
            var model = new ResetPassword2ViewModel
            {
                CustomerId = 1,
                Password = pass,
                ConfirmPassword = confirmPass
            };

            // Act
            var result = _controller.ResetPassword2(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "The result should not be null");
            Assert.AreEqual(model, result.Model, "The model should be returned with errors");
        }
    }
}
