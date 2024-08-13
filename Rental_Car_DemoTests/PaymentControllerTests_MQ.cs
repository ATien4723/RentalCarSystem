using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using Rental_Car_Demo.Controllers;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Services;
using Rental_Car_Demo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Rental_Car_Demo.Services;

namespace Rental_Car_Demo.UnitTests
{
    [TestFixture]
    public class PaymentControllerTest_MQ
    {
        private PaymentController _controller;
        private Mock<IEmailService> _emailServiceMock;
        private DummySession _dummySession;
        private DefaultHttpContext _httpContext;
        private RentCarDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<RentCarDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            _context = new RentCarDbContext(options);

            _emailServiceMock = new Mock<IEmailService>();
            _dummySession = new DummySession();

            _httpContext = new DefaultHttpContext
            {
                Session = _dummySession
            };

            _controller = new PaymentController(_context)
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
            var user1 = new User
            {
                Email = "kiet123@gmail.com",
                Password = HashPassword("kiet123"),
                Name = "kiet ne",
                Phone = "0334567890",
                Role = false,
                Wallet = 2000,
                RememberMe = false,
            };

            var user2 = new User
            {
                Email = "hehe@gmail.com",
                Password = HashPassword("hehe123"),
                Name = "hehe",
                Phone = "0987654321",
                Role = true,
                Wallet = 0,
                RememberMe = false,
            };

            _context.Users.AddRange(user1, user2);
            _context.SaveChanges();

            var wallet = new Wallet
            {
                UserId = user1.UserId,
                Amount = "1000",
                Type = "Top-up",
                CreatedAt = new DateTime(2024, 7, 9)
            };
            var transaction1 = new Wallet
            {
                UserId = user1.UserId,
                Amount = "500",
                Type = "Top-Up",
                CreatedAt = new DateTime(2024, 7, 2)
            };
            var transaction2 = new Wallet
            {
                UserId = user1.UserId,
                Amount = "200",
                Type = "Withdraw",
                CreatedAt = new DateTime(2024, 7, 15)
            };

            _context.Wallets.Add(wallet);
            _context.Wallets.Add(transaction1);
            _context.Wallets.Add(transaction2);
            _context.SaveChanges();

            var car1 = new Car
            {
                CarId = 1,
                Status = 2,
                BackImage = "back.img",
                Description = "Description",
                FrontImage = "frt.img",
                LeftImage = "left.img",
                LicensePlate = "lcs",
                Name = "Name",
                RightImage = "right.img"
            };
            _context.Cars.Add(car1);
            var booking1 = new Booking
            {
                CarId = car1.CarId,
                BookingNo = 1,
                Status = 5
            };
            _context.Bookings.Add(booking1);
            _context.SaveChanges();

        }
        [Test]
        public void MyWallet_UserNotLoggedIn_ReturnsErrorAuthorizationView()
        {
            _dummySession.Clear();
            var user = _context.Users.First();
            var result = _controller.MyWallet(user.UserId, null, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ErrorAuthorization", result.ViewName);
        }
        [Test]
        public void MyWallet_LoggedInUserIdDoesNotMatchParameter_ReturnsErrorAuthorizationView()
        {
            var user = _context.Users.First();
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _dummySession.SetString("User", userJson);

            var result = _controller.MyWallet(user.UserId + 1, null, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ErrorAuthorization", result.ViewName);
        }

        [Test]
        public void MyWallet_ValidUser_ReturnsWalletView()
        {

            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _dummySession.SetString("User", userJson);


            var result = _controller.MyWallet(user.UserId, null, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
            var model = result.Model as UserWalletViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(user.UserId, model.User.UserId);
        }
        [Test]
        public void MyWallet_ValidUser_ReturnCorrectTransactionAmount()
        {

            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _dummySession.SetString("User", userJson);

            var result = _controller.MyWallet(user.UserId, null, null) as ViewResult;

            var model = result.Model as UserWalletViewModel;
            Assert.AreEqual(3, model.Wallets.Count);
        }
        [Test]
        public void MyWallet_ValidUser_ReturnCorrectAmount()
        {

            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _dummySession.SetString("User", userJson);


            var result = _controller.MyWallet(user.UserId, null, null) as ViewResult;

            var model = result.Model as UserWalletViewModel;
            Assert.AreEqual("200", model.Wallets.First().Amount);
        }
        [Test]
        public void MyWallet_UserWithNoWalletEntries_ReturnsEmptyWalletList()
        {
            var user = _context.Users.First(x => x.Email == "hehe@gmail.com");
            _dummySession.SetString("User", JsonConvert.SerializeObject(user));

            var result = _controller.MyWallet(user.UserId, null, null) as ViewResult;

            Assert.IsNotNull(result);
            var model = result.Model as UserWalletViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Wallets.Count);
        }
        [Test]
        public void MyWallet_NullFromAndToDate_ReturnsAllTransactions()
        {
            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _dummySession.SetString("User", userJson);

            _context.Wallets.Add(new Wallet { UserId = user.UserId, Amount = "500", Type = "Top-up", CreatedAt = DateTime.Now.AddDays(-1) });
            _context.Wallets.Add(new Wallet { UserId = user.UserId, Amount = "200", Type = "Withdraw", CreatedAt = DateTime.Now.AddDays(-2) });
            _context.SaveChanges();

            var result = _controller.MyWallet(user.UserId, null, null) as ViewResult;

            Assert.IsNotNull(result);
            var model = result.Model as UserWalletViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(5, model.Wallets.Count);
        }
        [Test]
        [TestCase("999")]
        [TestCase("-1")]
        public void MyWallet_InvalidUserId_ReturnsErrorAuthorization(int userId)
        {

            var result = _controller.MyWallet(userId, null, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ErrorAuthorization", result.ViewName);
        }
        [Test]
        public void MyWallet_OrderByDescendingCreatedAt_ReturnsCorrectOrder()
        {
            var user = _context.Users.First(u => u.Email == "hehe@gmail.com");
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _dummySession.SetString("User", userJson);

            _context.Wallets.Add(new Wallet { UserId = user.UserId, Amount = "500", Type = "Top-up", CreatedAt = DateTime.Now.AddDays(-2) });
            _context.Wallets.Add(new Wallet { UserId = user.UserId, Amount = "200", Type = "Withdraw", CreatedAt = DateTime.Now.AddDays(-1) });
            _context.SaveChanges();

            var result = _controller.MyWallet(user.UserId, null, null) as ViewResult;

            Assert.IsNotNull(result);
            var model = result.Model as UserWalletViewModel;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.Wallets[0].CreatedAt > model.Wallets[1].CreatedAt);
        }
        [Test]
        public void Withdraw_Success_SaveTransaction()
        {

            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");
            decimal withdrawalAmount = 1500;


            var result = _controller.Withdraw(user.UserId, withdrawalAmount);


            Assert.IsNotNull(result);
            Assert.AreEqual("-1,500.00", _context.Wallets.Last().Amount);
        }
        [Test]
        public void Withdraw_Success_RemainingBalance()
        {

            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");
            decimal withdrawalAmount = 1500;


            var result = _controller.Withdraw(user.UserId, withdrawalAmount);

            Assert.AreEqual(500, user.Wallet);
        }
        [Test]
        public void Withdraw_Success_JsonSuccess()
        {
            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");
            decimal withdrawalAmount = 1500;


            var result = _controller.Withdraw(user.UserId, withdrawalAmount) as JsonResult;
            var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(result.Value));

            Assert.IsTrue(Convert.ToBoolean(jsonResult["success"]));
        }
        [Test]
        [TestCase("1000")]
        [TestCase("1800")]
        public void Withdraw_ValidAmount_TransactionAddedToDatabase(decimal amount)
        {

            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");
            int initialTransactionCount = _context.Wallets.Count();


            var result = _controller.Withdraw(user.UserId, amount);


            int newTransactionCount = _context.Wallets.Count();
            Assert.AreEqual(initialTransactionCount + 1, newTransactionCount);
            var lastTransaction = _context.Wallets.OrderByDescending(w => w.CreatedAt).FirstOrDefault();
            Assert.IsNotNull(lastTransaction);
            Assert.AreEqual(user.UserId, lastTransaction.UserId);
            Assert.AreEqual($"-{amount.ToString("N2")}", lastTransaction.Amount);
            Assert.AreEqual("Withdraw", lastTransaction.Type);
        }
        [Test]
        [TestCase("3000")]
        [TestCase("-100")]
        [TestCase("0")]
        public void Withdraw_SpecialCase_RemainBalance(decimal amount)
        {

            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");


            var result = _controller.Withdraw(user.UserId, amount);


            Assert.AreEqual(2000, user.Wallet);
        }
        [Test]
        public void Withdraw_SimultaneousWithdrawals_CorrectlyUpdatesBalance()
        {
            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");

            _controller.Withdraw(user.UserId, 500);
            _controller.Withdraw(user.UserId, 300);

            var updatedUser = _context.Users.First(u => u.UserId == user.UserId);

            Assert.AreEqual(1200, updatedUser.Wallet);
            Assert.AreEqual(3, _context.Wallets.Count(w => w.UserId == user.UserId && w.Type == "Withdraw"));
        }
        [Test]
        public void Withdraw_MaximumBalance_SuccessfulWithdrawal()
        {
            var user = _context.Users.First();
            var withdrawAmount = 2000;

            var result = _controller.Withdraw(user.UserId, 2000) as JsonResult;

            Assert.AreEqual(0, user.Wallet);
        }
        [Test]
        [TestCase("2500")]
        [TestCase("2001")]
        public void Withdraw_InsufficientBalance_ReturnsError(decimal amount)
        {
            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");
            var initialBalance = user.Wallet;

            var result = _controller.Withdraw(user.UserId, amount) as JsonResult;

            Assert.IsNotNull(result);
            var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(result.Value));

            Assert.IsNotNull(jsonResult);
            Assert.IsFalse(Convert.ToBoolean(jsonResult["success"]));
            Assert.AreEqual("Insufficient balance", jsonResult["error"].ToString());

            _context.Entry(user).Reload();
            Assert.AreEqual(initialBalance, user.Wallet);
        }
        [Test]
        public void Withdraw_NegativeAmount_ReturnsSuccessFalse()
        {
            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");

            var result = _controller.Withdraw(user.UserId, -100) as JsonResult;
            var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(result.Value));

            Assert.IsFalse(Convert.ToBoolean(jsonResult["success"]));
            Assert.AreEqual("Insufficient balance", jsonResult["error"].ToString());
        }
        [Test]
        public void Withdraw_UserDoesNotExist_ReturnsSuccessFalse()
        {
            var result = _controller.Withdraw(999, 100) as JsonResult;
            var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(result.Value));

            Assert.IsFalse(Convert.ToBoolean(jsonResult["success"]));
            Assert.AreEqual("User not found", jsonResult["error"].ToString());
        }

        [Test]
        public void TopUp_ValidAmount_Success()
        {

            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");


            var result = _controller.TopUp(user.UserId, 500);


            Assert.AreEqual(2500, user.Wallet);
        }
        [Test]
        public void TopUp_ValidAmount_TransactionAddedToDatabase()
        {

            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");
            decimal topUpAmount = 500;
            int initialTransactionCount = _context.Wallets.Count();


            var result = _controller.TopUp(user.UserId, topUpAmount);


            int newTransactionCount = _context.Wallets.Count();
            Assert.AreEqual(initialTransactionCount + 1, newTransactionCount);
            var lastTransaction = _context.Wallets.OrderByDescending(w => w.CreatedAt).FirstOrDefault();
            Assert.IsNotNull(lastTransaction);
            Assert.AreEqual(user.UserId, lastTransaction.UserId);
            Assert.AreEqual($"{topUpAmount.ToString("N2")}", lastTransaction.Amount);
            Assert.AreEqual("Top-Up", lastTransaction.Type);
        }

        [Test]
        public void TopUp_UserWalletNull_Success()
        {
            _context.Users.Add(new User
            {
                Email = "mquan19022k3@gmail.com",
                Password = HashPassword("mquan123"),
                Name = "quan",
                Phone = "0334567891",
                Role = false,
                Wallet = null
            });
            _context.SaveChanges();
            var user = _context.Users.First(u => u.Email == "mquan19022k3@gmail.com");
            var result = _controller.TopUp(user.UserId, 5000);
            Assert.AreEqual(5000, user.Wallet);
        }
        [Test]
        public void TopUp_UserWalletNull_SaveTransaction()
        {
            _context.Users.Add(new User
            {
                Email = "mquan19022k3@gmail.com",
                Password = HashPassword("mquan123"),
                Name = "quan",
                Phone = "0334567891",
                Role = false,
                Wallet = null
            });
            _context.SaveChanges();
            decimal amount = 5000;
            int initialTransactionCount = _context.Wallets.Count();

            var user = _context.Users.First(u => u.Email == "mquan19022k3@gmail.com");
            var result = _controller.TopUp(user.UserId, amount);
            int newTransactionCount = _context.Wallets.Count();

            Assert.AreEqual(initialTransactionCount + 1, newTransactionCount);
            var lastTransaction = _context.Wallets.OrderByDescending(w => w.CreatedAt).FirstOrDefault();
            Assert.IsNotNull(lastTransaction);
            Assert.AreEqual(user.UserId, lastTransaction.UserId);
            Assert.AreEqual($"{amount.ToString("N2")}", lastTransaction.Amount);
            Assert.AreEqual("Top-Up", lastTransaction.Type);
        }
        [Test]
        public void TopUp_UserNotFound_ReturnError()
        {
            var userId = 9999;
            var result = _controller.TopUp(userId, 1000) as JsonResult;

            var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(result.Value));

            Assert.IsFalse(Convert.ToBoolean(jsonResult["success"]));
            Assert.AreEqual("User not found", jsonResult["error"].ToString());
        }
        [Test]
        public void TopUp_Success_JsonSuccess()
        {
            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");

            var result = _controller.TopUp(user.UserId, 1000) as JsonResult;
            var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(result.Value));

            Assert.IsTrue(Convert.ToBoolean(jsonResult["success"]));
        }
        [Test]
        public void TopUp_NegativeAmount_ReturnError()
        {
            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");

            var result = _controller.TopUp(user.UserId, -1000) as JsonResult;
            var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(result.Value));

            Assert.IsFalse(Convert.ToBoolean(jsonResult["success"]));
            Assert.AreEqual("Insufficient balance", jsonResult["error"]);
        }
        [Test]
        [TestCase("2024, 7, 2", "2024, 7, 5")]
        [TestCase("2024, 7, 1", "2024, 7, 2")]
        [TestCase("2024, 6, 26", "2024, 7, 5")]
        public void SearchTransactions_ValidDateRange_ReturnsView(DateTime startdate, DateTime enddate)
        {

            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");


            DateTime fromDate = startdate;
            DateTime toDate = enddate;


            var result = _controller.SearchTransactions(user.UserId, fromDate, toDate) as ViewResult;


            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual("MyWallet", result.ViewName);
        }
        [Test]
        public void SearchTransactions_ValidDateRange_ReturnsCorrectTransaction()
        {

            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");


            DateTime fromDate = new DateTime(2024, 7, 1);
            DateTime toDate = new DateTime(2024, 7, 31);


            var result = _controller.SearchTransactions(user.UserId, fromDate, toDate) as ViewResult;

            var model = result.Model as UserWalletViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual("200", model.Wallets[0].Amount);
            Assert.AreEqual("1000", model.Wallets[1].Amount);
            Assert.AreEqual("Withdraw", model.Wallets[0].Type);
            Assert.AreEqual("Top-up", model.Wallets[1].Type);
        }
        [Test]
        [TestCase("2024, 7, 2", "2024, 7, 15")]
        [TestCase("2024, 6, 25", "2024, 7, 20")]
        [TestCase("2024, 7, 2", "2024, 7, 20")]
        [TestCase("2024, 6, 25", "2024, 7, 15")]
        public void SearchTransactions_ValidDateRange_ReturnsNumberOfTransaction(DateTime fromDate, DateTime toDate)
        {

            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");

            var result = _controller.SearchTransactions(user.UserId, fromDate, toDate) as ViewResult;

            var model = result.Model as UserWalletViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(3, model.Wallets.Count);
        }
        [Test]
        [TestCase("2024, 7, 15", "2024, 7, 17")]
        [TestCase("2024, 7, 13", "2024, 7, 15")]
        public void SearchTransactions_LimitDateRange_ReturnCorrectTransaction(DateTime startdate, DateTime enddate)
        {
            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");

            DateTime fromDate = startdate;
            DateTime toDate = enddate;

            var result = _controller.SearchTransactions(user.UserId, fromDate, toDate) as ViewResult;

            var model = result.Model as UserWalletViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Wallets.Count);
            Assert.AreEqual("200", model.Wallets[0].Amount);
            Assert.AreEqual("Withdraw", model.Wallets[0].Type);
        }
        [Test]
        [TestCase("2025, 1, 1", "2025, 1, 31")]
        [TestCase("2024, 8, 6", "2024, 8,1")]
        [TestCase(null, null)]
        public void SearchTransactions_InvalidDateRange_ReturnsView(DateTime? start, DateTime? end)
        {
            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");

            var result = _controller.SearchTransactions(user.UserId, start, end) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual("MyWallet", result.ViewName);
        }
        [Test]
        [TestCase("2025, 1, 1", "2025, 1, 31")]
        [TestCase("2024, 8, 6", "2024, 8,1")]
        public void SearchTransactions_InvalidDateRange_ReturnsNoTransaction(DateTime? start, DateTime? end)
        {
            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");


            var result = _controller.SearchTransactions(user.UserId, start, end) as ViewResult;

            var model = result.Model as UserWalletViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Wallets.Count);
        }
        [Test]
        public void SearchTransactions_BothNullDateRange_ReturnsTransaction()
        {
            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");


            var result = _controller.SearchTransactions(user.UserId, null, null) as ViewResult;

            var model = result.Model as UserWalletViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(3, model.Wallets.Count);
        }
        [Test]
        [TestCase(null, "2024, 7, 5")]
        [TestCase(null, "2024, 7, 2")]
        public void SearchTransactions_StartDateNull_ReturnsTransaction(DateTime? startdate, DateTime? enddate)
        {
            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");


            var result = _controller.SearchTransactions(user.UserId, startdate, enddate) as ViewResult;

            var model = result.Model as UserWalletViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual("500", model.Wallets[0].Amount);
            Assert.AreEqual("Top-Up", model.Wallets[0].Type);
        }
        [Test]
        [TestCase(null, "2024, 7, 18")]
        [TestCase(null, "2024, 7, 15")]
        public void SearchTransactions_StartDateNull_ReturnsNumberOfTransaction(DateTime? startdate, DateTime? enddate)
        {
            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");


            var result = _controller.SearchTransactions(user.UserId, startdate, enddate) as ViewResult;

            var model = result.Model as UserWalletViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(3, model.Wallets.Count);
        }
        [Test]
        [TestCase("2024, 7, 10", null)]
        [TestCase("2024, 7, 15", null)]
        public void SearchTransactions_ToDateNull_ReturnsTransaction(DateTime? startdate, DateTime? enddate)
        {
            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");


            var result = _controller.SearchTransactions(user.UserId, startdate, enddate) as ViewResult;

            var model = result.Model as UserWalletViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual("200", model.Wallets[0].Amount);
            Assert.AreEqual("Withdraw", model.Wallets[0].Type);
        }
        [Test]
        [TestCase("2024, 6, 30", null)]
        [TestCase("2024, 7, 2", null)]
        public void SearchTransactions_ToDateNull_ReturnsNumberOfTransaction(DateTime? startdate, DateTime? enddate)
        {
            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");


            var result = _controller.SearchTransactions(user.UserId, startdate, enddate) as ViewResult;

            var model = result.Model as UserWalletViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(3, model.Wallets.Count);
        }
        [Test]
        [TestCase("2024, 6, 15", "2024, 6, 20")]
        [TestCase("2024, 7, 16", "2024, 7, 20")]
        public void SearchTransactions_OutOfDateRange_ReturnsNoTransaction(DateTime? startdate, DateTime? enddate)
        {
            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");


            var result = _controller.SearchTransactions(user.UserId, startdate, enddate) as ViewResult;

            var model = result.Model as UserWalletViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Wallets.Count);
        }
        [Test]
        [TestCase("2024, 6, 15", "2024, 6, 20")]
        [TestCase("2024, 7, 16", "2024, 7, 20")]
        public void SearchTransactions_OutOfDateRange_ReturnsView(DateTime? startdate, DateTime? enddate)
        {
            var user = _context.Users.First(u => u.Email == "kiet123@gmail.com");


            var result = _controller.SearchTransactions(user.UserId, startdate, enddate) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("MyWallet", result.ViewName);
        }
        [Test]
        public void SearchTransaction_UserDontHaveTransaction_ReturnNoTransaction()
        {
            var user = _context.Users.First(u => u.Email == "hehe@gmail.com");
            DateTime fromDate = new DateTime(2024, 7, 1);
            DateTime toDate = new DateTime(2024, 7, 31);


            var result = _controller.SearchTransactions(user.UserId, fromDate, toDate) as ViewResult;
            var model = result.Model as UserWalletViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Wallets.Count);
        }
        [Test]
        public void SearchTransaction_UserDontHaveTransaction_ReturnView()
        {
            var user = _context.Users.First(u => u.Email == "hehe@gmail.com");
            DateTime fromDate = new DateTime(2024, 7, 1);
            DateTime toDate = new DateTime(2024, 7, 31);


            var result = _controller.SearchTransactions(user.UserId, fromDate, toDate) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("MyWallet", result.ViewName);
        }

        [Test]
        public void SearchTransaction_UserDoesNotExist_ReturnNotFound()
        {
            DateTime fromDate = new DateTime(2024, 7, 1);
            DateTime toDate = new DateTime(2024, 7, 31);


            var result = _controller.SearchTransactions(9999, fromDate, toDate);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        public void ConfirmPayment_ValidCarId_RedirectAction()
        {
            var car = _context.Cars.First(x => x.CarId == 1);

            var result = _controller.ConfirmPayment(car.CarId) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ChangeCarDetailsByOwner", result.ActionName);
            Assert.AreEqual("Car", result.ControllerName);
        }
        [Test]
        public void ConfirmPayment_ValidCarId_RouteValue()
        {
            var car = _context.Cars.First(x => x.CarId == 1);

            var result = _controller.ConfirmPayment(car.CarId) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RouteValues["CarId"]);

        }
        [Test]
        public void ConfirmPayment_ValidCarId_UpdateStatus()
        {
            var car = _context.Cars.First(x => x.CarId == 1);

            var result = _controller.ConfirmPayment(car.CarId) as RedirectToActionResult;

            var updatedCar = _context.Cars.FirstOrDefault(c => c.CarId == car.CarId);
            Assert.IsNotNull(updatedCar);
            Assert.AreEqual(1, updatedCar.Status);
        }
        [Test]
        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3")]
        [TestCase("4")]
        public void ConfirmPayment_NotPaid_ReturnView(int status)
        {

            var car = new Car
            {
                CarId = 2,
                Status = 2,
                BackImage = "back.img",
                Description = "Description",
                FrontImage = "frt.img",
                LeftImage = "left.img",
                LicensePlate = "lcs",
                Name = "Name",
                RightImage = "right.img"
            };
            _context.Cars.Add(car);
            var booking = new Booking
            {
                CarId = car.CarId,
                BookingNo = 2,
                Status = status
            };
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var paymentController = new PaymentController(_context);
            paymentController.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );


            var result = paymentController.ConfirmPayment(car.CarId) as RedirectToActionResult;


            Assert.IsNotNull(result);
            Assert.AreEqual("ChangeCarDetailsByOwner", result.ActionName);
            Assert.AreEqual("Car", result.ControllerName);
        }
        [Test]
        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3")]
        [TestCase("4")]
        public void ConfirmPayment_NotPaid_ReturnError(int status)
        {

            var car = new Car
            {
                CarId = 2,
                Status = 2,
                BackImage = "back.img",
                Description = "Description",
                FrontImage = "frt.img",
                LeftImage = "left.img",
                LicensePlate = "lcs",
                Name = "Name",
                RightImage = "right.img"
            };
            _context.Cars.Add(car);
            var booking = new Booking
            {
                CarId = car.CarId,
                BookingNo = 2,
                Status = status
            };
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var paymentController = new PaymentController(_context);
            paymentController.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );


            var result = paymentController.ConfirmPayment(car.CarId) as RedirectToActionResult;

            Assert.IsTrue(paymentController.TempData.ContainsKey("ErrorMessage"));
            Assert.AreEqual("Customer has not paid the car rental.", paymentController.TempData["ErrorMessage"]);
        }
        [Test]
        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3")]
        [TestCase("4")]
        public void ConfirmPayment_NotPaid_NotChangeBookingStatus(int status)
        {

            var car = new Car
            {
                CarId = 2,
                Status = 2,
                BackImage = "back.img",
                Description = "Description",
                FrontImage = "frt.img",
                LeftImage = "left.img",
                LicensePlate = "lcs",
                Name = "Name",
                RightImage = "right.img"
            };
            _context.Cars.Add(car);
            var booking = new Booking
            {
                CarId = car.CarId,
                BookingNo = 2,
                Status = status
            };
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var paymentController = new PaymentController(_context);
            paymentController.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );


            var result = paymentController.ConfirmPayment(car.CarId) as RedirectToActionResult;
            var curBooking = _context.Bookings.FirstOrDefault(b => b.CarId == car.CarId);
            Assert.AreEqual(status, curBooking.Status);
        }
        [Test]
        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3")]
        [TestCase("4")]
        public void ConfirmPayment_NotPaid_NotChangeCarStatus(int status)
        {

            var car = new Car
            {
                CarId = 2,
                Status = 2,
                BackImage = "back.img",
                Description = "Description",
                FrontImage = "frt.img",
                LeftImage = "left.img",
                LicensePlate = "lcs",
                Name = "Name",
                RightImage = "right.img"
            };
            _context.Cars.Add(car);
            var booking = new Booking
            {
                CarId = car.CarId,
                BookingNo = 2,
                Status = status
            };
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var paymentController = new PaymentController(_context);
            paymentController.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );


            var result = paymentController.ConfirmPayment(car.CarId) as RedirectToActionResult;

            var curCar = _context.Cars.FirstOrDefault(c => c.CarId == car.CarId);
            Assert.AreEqual(2, curCar.Status);
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
    }
    public class JsonResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
    }

}

