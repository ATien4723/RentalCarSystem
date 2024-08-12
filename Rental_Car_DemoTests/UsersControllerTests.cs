using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using Rental_Car_Demo.Context;
using Rental_Car_Demo.Controllers;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.UserRepository;
using Rental_Car_Demo.Services;
using Rental_Car_Demo.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rental_Car_Demo.UnitTests
{
    [TestFixture]
    public class UsersControllerTests
    {
        private UsersController _controller;
        private Mock<IEmailService> _emailServiceMock;
        private InMemoryUserDAO _userDao;
        private DummySession _dummySession;
        private DummyCookies _dummyCookies;
        private DefaultHttpContext _httpContext;
        private RentCarDbContext _context;
        private Mock<ITokenGenerator> _mockTokenGenerator;
        private Mock<ICustomerContext> _mockCustomerContext;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<RentCarDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            _context = new RentCarDbContext(options);

            _emailServiceMock = new Mock<IEmailService>();
            _userDao = new InMemoryUserDAO();
            _dummySession = new DummySession();
            _dummyCookies = new DummyCookies();
            _httpContext = new DefaultHttpContext
            {
                Session = _dummySession,
                Request = { Cookies = _dummyCookies }
            };



            _controller = new UsersController(_context, _mockCustomerContext.Object, _mockTokenGenerator.Object, _emailServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext
                }
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
                 Email = "kiet123@gmail.com",
                 Password = HashPassword("kiet123"),
                 Name = "kiet ne",
                 Phone = "0334567890",
                 Role = false,
                 Wallet = 0,
                 RememberMe= false,
             },
             new User
             {
                 Email = "hehe@gmail.com",
                 Password = HashPassword("hehe123"),
                 Name = "hehe",
                 Phone = "0987654321",
                 Role = true,
                 Wallet = 0,
                 RememberMe= true,
             }
         };

            _context.Users.AddRange(users);
            _context.SaveChanges();
        }

        [Test]
        public void Login_SetsDefaultCultureCookie()
        {

            var expectedCulture = "or-IN";

            var result = _controller.Login();

            Assert.AreEqual(expectedCulture, CultureInfo.CurrentCulture.Name);
        }
        [Test]
        public void Login_InitializesViewModel()
        {
            var result = _controller.Login() as ViewResult;

            var viewModel = result?.Model as RegisterAndLoginViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsNotNull(viewModel.Register);
            Assert.IsNotNull(viewModel.User);
        }

        [Test]
        public void Login_NoRememberMeCookie_SetsUserDefaults()
        {
            var result = _controller.Login() as ViewResult;

            var viewModel = result?.Model as RegisterAndLoginViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsNull(viewModel.User.Email);
            Assert.IsNull(viewModel.User.Password);
            Assert.IsFalse(viewModel.User.RememberMe);
        }
        [Test]
        [TestCase("UserEmail", "invalidFormat")]
        public void Login_InvalidCookieFormat_DoesNotSetUserDetails(string sessionkey, string sessionvalue)
        {

            _dummyCookies.Append(sessionkey, sessionvalue);


            var result = _controller.Login() as ViewResult;
            var viewModel = result.Model as RegisterAndLoginViewModel;


            Assert.IsNull(viewModel.User.Email);
            Assert.IsNull(viewModel.User.Password);
            Assert.IsFalse(viewModel.User.RememberMe);
        }

        [Test]
        [TestCase("UserEmail", "|mquan123")]
        [TestCase("UserEmail", "mquan19022k3@gmail.com|")]
        [TestCase("UserEmail", "|")]
        public void Login_SessionNotNull(string sessionkey, string sessionvalue)
        {

            _dummyCookies.Append(sessionkey, sessionvalue);


            var result = _controller.Login() as ViewResult;
            var viewModel = result.Model as RegisterAndLoginViewModel;


            Assert.IsNotNull(viewModel.User.Email);
            Assert.IsNotNull(viewModel.User.Password);
        }
        [Test]
        public void Login_CookieContainsExtraSeparator_SplitsIncorrectly()
        {

            _dummyCookies.Append("UserEmail", "test@example.com|password123|extra");


            var result = _controller.Login() as ViewResult;
            var viewModel = result.Model as RegisterAndLoginViewModel;


            Assert.IsNull(viewModel.User.Email);
            Assert.IsNull(viewModel.User.Password);
            Assert.IsFalse(viewModel.User.RememberMe);
        }

        [Test]
        public void Login_ExpiredCultureCookie_ResetsToDefaultCulture()
        {

            CultureInfo.CurrentCulture = new CultureInfo("en-US");


            var result = _controller.Login() as ViewResult;


            Assert.AreEqual("or-IN", CultureInfo.CurrentCulture.Name);
        }
        [Test]
        public void Login_ExpiredCookie_NoUserDetailsSet()
        {

            _dummyCookies.Append("UserEmail", "test@gmail.com|password123");

            _dummyCookies.Delete("UserEmail");


            var result = _controller.Login() as ViewResult;


            var viewModel = result.Model as RegisterAndLoginViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsNull(viewModel.User.Email);
            Assert.IsNull(viewModel.User.Password);
        }

        [Test]
        public void Login_ReturnsLoginView()
        {

            var result = _controller.Login() as ViewResult;


            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ViewName ?? "Login");
        }

        [Test]
        public void Login_WithRememberMeCookie_ReturnsViewWithUserDetails()
        {

            var cookieValue = "test@example.com|password123";
            var encodedCookieValue = System.Web.HttpUtility.UrlEncode(cookieValue);
            _httpContext.Request.Headers["Cookie"] = $"UserEmail={encodedCookieValue}";


            var result = _controller.Login() as ViewResult;

            Assert.IsNotNull(result);
            var viewModel = result.Model as RegisterAndLoginViewModel;
            Assert.IsNotNull(viewModel);
            Assert.AreEqual("test@example.com", viewModel.User.Email);
            Assert.AreEqual("password123", viewModel.User.Password);
        }
        [Test]
        public void Login_ValidCookie_DoesNotOverwriteValidUserDetails()
        {

            string rememberMeValue = "kiet123@gmail.com|kiet123";
            _dummyCookies.Append("UserEmail", rememberMeValue);


            var firstLoginResult = _controller.Login() as ViewResult;
            var firstViewModel = firstLoginResult.Model as RegisterAndLoginViewModel;


            string newRememberMeValue = "hehe@gmail.com|hehe123";
            _dummyCookies.Append("UserEmail", newRememberMeValue);


            var secondLoginResult = _controller.Login() as ViewResult;
            var secondViewModel = secondLoginResult.Model as RegisterAndLoginViewModel;


            Assert.AreEqual("hehe@gmail.com", secondViewModel.User.Email);
            Assert.AreEqual("hehe123", secondViewModel.User.Password);
            Assert.IsTrue(secondViewModel.User.RememberMe);
        }
        [Test]
        public void Login_EmptyCookie_DoesNotSetUserDetails()
        {

            _dummyCookies.Append("UserEmail", string.Empty);


            var result = _controller.Login() as ViewResult;
            var viewModel = result.Model as RegisterAndLoginViewModel;


            Assert.IsNull(viewModel.User.Email);
            Assert.IsNull(viewModel.User.Password);
            Assert.IsFalse(viewModel.User.RememberMe);
        }
        [Test]
        public void Login_ValidUser_LogsInSuccessfully()
        {

            _dummyCookies.Append("UserEmail", "kiet123@gmail.com|kiet123");


            var result = _controller.Login() as ViewResult;
            var viewModel = result.Model as RegisterAndLoginViewModel;


            Assert.AreEqual("kiet123@gmail.com", viewModel.User.Email);
            Assert.AreEqual("kiet123", viewModel.User.Password);
            Assert.IsTrue(viewModel.User.RememberMe);
        }
        [Test]
        public void Login_CultureAlreadySet_DoesNotChangeCulture()
        {
            // Arrange
            CultureInfo.CurrentCulture = new CultureInfo("or-IN");

            // Act
            var result = _controller.Login() as ViewResult;

            // Assert
            Assert.AreEqual("or-IN", CultureInfo.CurrentCulture.Name);
        }

        [Test]
        public void Login_NullCookieValue_DoesNotSetUserDetails()
        {
            // Arrange
            _dummyCookies.Append("UserEmail", null);

            // Act
            var result = _controller.Login() as ViewResult;
            var viewModel = result.Model as RegisterAndLoginViewModel;

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.IsNull(viewModel.User.Email);
            Assert.IsNull(viewModel.User.Password);
            Assert.IsFalse(viewModel.User.RememberMe);
        }

        [Test]
        public void Login_MultipleCookiesWithSameKey_UsesLatestValue()
        {
            // Arrange
            _dummyCookies.Append("UserEmail", "old@example.com|oldpassword");
            _dummyCookies.Append("UserEmail", "new@example.com|newpassword");

            // Act
            var result = _controller.Login() as ViewResult;
            var viewModel = result.Model as RegisterAndLoginViewModel;

            // Assert
            Assert.AreEqual("new@example.com", viewModel.User.Email);
            Assert.AreEqual("newpassword", viewModel.User.Password);
        }

        [Test]
        [TestCase("", "mquan123")]
        [TestCase("", "")]
        [TestCase("mquan19022k3@gmail.com", "")]
        [TestCase("mquan19022k3gmail.com", "")]
        [TestCase("mquan19022k3@", "")]
        [TestCase("mquan19022k3", "")]
        [TestCase("mquan19022k3.com", "")]
        public void Login_Post_ValidFormat_RedirectToLogin(string email, string password)
        {
            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = email, Password = password }
            };

            var result = _controller.Login(viewModel) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ViewName ?? "Login");

        }
        [Test]
        public void Login_Post_NoRememberMe()
        {
            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = "kiet123@gmail.com", Password = "kiet123", RememberMe = false }
            };

            var result = _controller.Login(viewModel) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(_httpContext.Request.Cookies.ContainsKey("UserEmail"));
        }

        [Test]
        public void Login_Post_IncorrectEmailandPassword_ReturnLoginPage()
        {
            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = "invalid@example.com", Password = "wrongpassword" }
            };


            var result = _controller.Login(viewModel) as ViewResult;


            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ViewName ?? "Login");
        }
        [Test]
        public void Login_Post_WrongPassword_ErrorNotNull()
        {

            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = "invalid@example.com", Password = "wrongpassword" }
            };


            var result = _controller.Login(viewModel) as ViewResult;


            Assert.IsNotNull(result);
            Assert.IsTrue(_controller.ViewBag.Error != null);
        }
        [Test]
        public void Login_Post_WrongPassword_ReturnErrorMessage()
        {

            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = "kiet123@example.com", Password = "wrongpassword" }
            };


            var result = _controller.Login(viewModel) as ViewResult;


            Assert.IsNotNull(result);
            Assert.AreEqual("Either email address or password is incorrect. Please try again", result.ViewData["Error"]);
        }
        [Test]
        public void Login_UserDoesNotExist_ErrorNotNull()
        {
            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = "notexist@gmail.com", Password = "123aad" }
            };

            var result = _controller.Login(viewModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(_controller.ViewBag.Error != null);
        }
        [Test]
        public void Login_UserDoesNotExist_ReturnErrorMessage()
        {
            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = "notexist@gmail.com", Password = "123aad" }
            };

            var result = _controller.Login(viewModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Either email address or password is incorrect. Please try again", _controller.ViewBag.Error);
        }
        [Test]
        public void Login_Post_ValidUser_RedirectsToLoginOwn()
        {

            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = "hehe@gmail.com", Password = "hehe123" }
            };


            var result = _controller.Login(viewModel) as RedirectToActionResult;


            Assert.IsNotNull(result);
            Assert.AreEqual("LoginOwn", result.ActionName);
        }
        [Test]
        [TestCase("hehe@gmail.com", "hehe123")]
        [TestCase("kiet123@gmail.com", "kiet123")]
        public void Login_Post_ValidUser_RedirectsToUserAction(string email, string password)
        {

            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = email, Password = password }
            };


            var result = _controller.Login(viewModel) as RedirectToActionResult;


            Assert.IsNotNull(result);
            Assert.AreEqual("Users", result.ControllerName);
        }
        [Test]
        [TestCase("hehe@gmail.com", "hehe123")]
        [TestCase("kiet123@gmail.com", "kiet123")]
        public void Login_Post_ValidUser_SaveSession(string email, string password)
        {

            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = email, Password = password }
            };


            var result = _controller.Login(viewModel) as RedirectToActionResult;


            Assert.IsNotNull(result);
            Assert.IsTrue(_dummySession.TryGetValue("User", out _));
        }
        [Test]
        [TestCase("heheafaf12@gmail.com", "hehe123")]
        [TestCase("kiet123aaff@gmail.com", "kiet123")]
        [TestCase("kiet123aaffgmail.com", "kiet123")]
        [TestCase("kiet123aaffgmail.com", "")]
        [TestCase("", "kiet123")]
        [TestCase("", "")]
        [TestCase("kiet123aaff@gmail", "")]
        public void Login_Post_WrongEmailAndPassword_DontSaveSession(string email, string password)
        {

            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = email, Password = password }
            };


            var result = _controller.Login(viewModel) as RedirectToActionResult;

            Assert.IsFalse(_dummySession.TryGetValue("User", out _));
        }
        [Test]
        public void Login_Post_ValidCustomer_RedirectsToLoginCus()
        {

            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = "kiet123@gmail.com", Password = "kiet123" }
            };


            var result = _controller.Login(viewModel) as RedirectToActionResult;


            Assert.IsNotNull(result);
            Assert.AreEqual("LoginCus", result.ActionName);
        }
        [Test]
        public void Login_Post_RememberMeUnchecked_DeletesCookie()
        {

            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = "kiet123@gmail.com", Password = "kiet123" }
            };


            var result = _controller.Login(viewModel) as RedirectToActionResult;


            Assert.IsNotNull(result);
            Assert.IsFalse(_httpContext.Request.Cookies.ContainsKey("UserEmail"));
        }
        [Test]
        public void Login_Post_ExistingSession_ReturnsViewResult()
        {
            // Arrange
            var user = new User { Email = "test@example.com", Password = "password" };
            _httpContext.Session.SetString("User", JsonConvert.SerializeObject(user));

            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = "test@example.com", Password = "password" }
            };

            // Act
            var result = _controller.Login(viewModel);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }
        [Test]
        public void Login_Post_RememberMeChecked_SetsCookie()
        {
            // Arrange
            var user = new User { Email = "test@example.com", Password = HashPassword("password"), Name = "Quan", Phone = "0815284412", Role = false };
            _context.Users.Add(user);
            _context.SaveChanges();

            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = "test@example.com", Password = "password", RememberMe = true }
            };

            // Act
            var result = _controller.Login(viewModel) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("LoginCus", result.ActionName);
            Assert.IsTrue(_httpContext.Response.Headers.ContainsKey("Set-Cookie"));
        }

        [Test]
        public void Login_Post_RememberMe_SetsEncodedCookie()
        {
            // Arrange
            var user = new User { Email = "test@example.com", Password = HashPassword("password"), Name = "Quan", Phone = "0815284412", Role = false };
            _context.Users.Add(user);
            _context.SaveChanges();

            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = "test@example.com", Password = "password", RememberMe = true }
            };

            // Act
            var result = _controller.Login(viewModel) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("LoginCus", result.ActionName);

            var setCookieHeader = _httpContext.Response.Headers["Set-Cookie"].ToString();
            Assert.IsTrue(setCookieHeader.Contains("UserEmail"));
        }
        [Test]
        public void Login_Post_NoRememberMe_DontSetsCookieWithCorrectOptions()
        {
            // Arrange
            var user = new User { Email = "test@example.com", Password = HashPassword("password"), Name = "Quan", Phone = "0815284412", Role = false };
            _context.Users.Add(user);
            _context.SaveChanges();

            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = "test@example.com", Password = "password", RememberMe = false }
            };

            // Act
            var result = _controller.Login(viewModel) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            var setCookieHeader = _httpContext.Response.Headers["Set-Cookie"].ToString();
            Assert.IsFalse(setCookieHeader.Contains("HttpOnly"));
            Assert.IsFalse(setCookieHeader.Contains("Secure"));
            Assert.IsFalse(setCookieHeader.Contains("SameSite=None"));
            Assert.IsFalse(setCookieHeader.Contains("Expires="));
        }
        public string HashPassword(string password)
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
        [TestCase("en-US")]
        [TestCase("fr-FR")]
        public void Login_Post_DifferentCultures_LoginWorks(string culture)
        {
            var cultureInfo = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = "hehe@gmail.com", Password = "hehe123" }
            };

            var result = _controller.Login(viewModel) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("LoginOwn", result.ActionName);
        }
        [Test]
        public void Login_Post_ValidUser_UsesHashedPassword()
        {
            var password = "testPassword";
            var hashedPassword = HashPassword(password);
            var user = new User { Email = "test@example.com", Password = hashedPassword, Name = "quan", Phone = "0815232213", Role = true };
            _context.Users.Add(user);
            _context.SaveChanges();

            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = "test@example.com", Password = password }
            };

            var result = _controller.Login(viewModel) as RedirectToActionResult;

            Assert.IsNotNull(result);
        }
        [Test]
        public void Login_Post_InvalidUser_ErrorMessageDoesNotContainHtml()
        {
            var viewModel = new RegisterAndLoginViewModel
            {
                User = new User { Email = "<script>alert('xss')</script>", Password = "password" }
            };

            var result = _controller.Login(viewModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(_controller.ViewBag.Error);
            Assert.IsFalse(_controller.ViewBag.Error.Contains("<script>"));
        }
        [Test]
        public void HashPassword_ValidInput_ReturnsNonEmptyString()
        {
            string result = _controller.HashPassword("password123");
            Assert.IsNotEmpty(result);
        }
        [Test]
        public void HashPassword_NullInput_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _controller.HashPassword(null));
        }
        [Test]
        [TestCase("")]
        [TestCase("!@#$%^&*()_+{}|:<>?")]
        [TestCase("パスワード")]
        [TestCase("\n")]
        public void HashPassword_SpecialPassword_ReturnsValidHash(string password)
        {
            string result = _controller.HashPassword(password);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(64, result.Length);
        }
        [Test]
        public void HashPassword_LongInput_ReturnsValidHash()
        {
            string longInput = new string('a', 10000);
            string result = _controller.HashPassword(longInput);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(64, result.Length);
        }
        [Test]
        public void HashPassword_ConsistentLength_ReturnsHashOf64Characters()
        {
            string[] inputs = { "a", "ab", "abc", "abcd", "abcde" };
            foreach (var input in inputs)
            {
                string result = _controller.HashPassword(input);
                Assert.AreEqual(64, result.Length);
            }
        }
        [Test]
        public void HashPassword_OnlyHexCharacters_InOutput()
        {
            string result = _controller.HashPassword("testPassword");
            Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(result, "^[a-fA-F0-9]+$"));
        }
        [Test]
        public void HashPassword_PerformanceTest()
        {
            string input = "performance test password";
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _controller.HashPassword(input);
            watch.Stop();
            Assert.Less(watch.ElapsedMilliseconds, 100, "Hashing took too long");
        }

        [Test]
        public void HashPassword_ValidPassword_ReturnsExpectedHash()
        {

            string password = "testpassword";
            string expectedHash = "9f735e0df9a1ddc702bf0a1a7b83033f9f7153a00c29de82cedadc9957289b05";



            string actualHash = _controller.HashPassword(password);


            Assert.AreEqual(expectedHash, actualHash);
        }

        [Test]
        public void HashPassword_DifferentPasswords_ReturnDifferentHashes()
        {

            string password1 = "password1";
            string password2 = "password2";


            string hash1 = _controller.HashPassword(password1);
            string hash2 = _controller.HashPassword(password2);


            Assert.AreNotEqual(hash1, hash2);
        }
        [Test]
        public void HashPassword_DifferentCase_ReturnsDifferentHashes()
        {
            string inputLower = "password";
            string inputUpper = "PASSWORD";

            string hashLower = _controller.HashPassword(inputLower);
            string hashUpper = _controller.HashPassword(inputUpper);

            Assert.AreNotEqual(hashLower, hashUpper);
        }
        [Test]
        public void HashPassword_ValidInput_NoExceptions()
        {
            Assert.DoesNotThrow(() => _controller.HashPassword("safeInput"));
        }

        [Test]
        public void HashPassword_SameInputTwice_ReturnsSameHash()
        {
            string input = "testPassword";
            string hash1 = _controller.HashPassword(input);
            string hash2 = _controller.HashPassword(input);
            Assert.AreEqual(hash1, hash2);
        }


        [Test]
        public void LoginCus_UserIsOwner_ReturnsErrorAuthorizationView()
        {

            var user = new User { Role = true };
            _dummySession.Set("User", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user)));


            var result = _controller.LoginCus() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ErrorAuthorization", result.ViewName);
        }

        [Test]
        public void LoginCus_UserIsCustomer_ReturnsLoginCusView()
        {

            var user = new User { Role = false };
            _dummySession.Set("User", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user)));


            var result = _controller.LoginCus() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void LoginOwn_UserIsCustomer_ReturnsErrorAuthorizationView()
        {

            var user = new User { Role = false };
            _dummySession.Set("User", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user)));

            var result = _controller.LoginOwn() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ErrorAuthorization", result.ViewName);
        }

        [Test]
        public void LoginOwn_UserIsOwner_ReturnsLoginOwnView()
        {

            var user = new User { Role = true };
            _dummySession.Set("User", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user)));


            var result = _controller.LoginOwn() as ViewResult;


            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Logout_RedirectsToGuest()
        {

            _dummySession.Set("User", new byte[0]);

            var result = _controller.Logout();

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("Guest", redirectResult.ActionName);
            Assert.AreEqual("Users", redirectResult.ControllerName);
        }
        [Test]
        public void Logout_RemoveSession()
        {

            _dummySession.Set("User", new byte[0]);


            var result = _controller.Logout();


            Assert.IsFalse(_dummySession.TryGetValue("User", out _));
        }
        [Test]
        public void Guest_ReturnsView()
        {

            var result = _controller.Guest();


            Assert.IsInstanceOf<ViewResult>(result);
        }
    }
    
    public class DummyCookies : IRequestCookieCollection, IResponseCookies
    {
        private readonly Dictionary<string, string> _cookiesStore = new Dictionary<string, string>();
        private readonly List<string> _deletedCookies = new List<string>();

        public string this[string key] => _cookiesStore.ContainsKey(key) ? _cookiesStore[key] : null;

        public int Count => _cookiesStore.Count;

        public ICollection<string> Keys => _cookiesStore.Keys;

        public bool ContainsKey(string key) => _cookiesStore.ContainsKey(key);

        public bool TryGetValue(string key, out string value) => _cookiesStore.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _cookiesStore.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _cookiesStore.GetEnumerator();

        public IReadOnlyList<string> DeletedCookies => _deletedCookies.AsReadOnly();

        public void Append(string key, string value, CookieOptions options)
        {
            _cookiesStore[key] = value;
        }

        public void Append(string key, string value)
        {
            _cookiesStore[key] = value;
        }

        public void Delete(string key)
        {
            if (_cookiesStore.ContainsKey(key))
            {
                _cookiesStore.Remove(key);
                _deletedCookies.Add(key);
            }
        }

        public void Delete(string key, CookieOptions options)
        {
            Delete(key);
        }
    }


    public class InMemoryUserDAO : UserDAO
    {
        private readonly List<User> _users = new List<User>();

        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public List<User> GetUserList()
        {
            return _users;
        }
    }

}

