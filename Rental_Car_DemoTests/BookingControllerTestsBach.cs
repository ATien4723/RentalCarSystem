using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Rental_Car_Demo.Controllers;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.BookingRepository;
using System.Collections;


namespace Rental_Car_Demo.UnitTests
{
    [TestFixture]
    public class BookingControllerTestsBach
    {
        private BookingController _bookingController;
        private RentCarDbContext _dbContext;
        private DummySession _session;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<RentCarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new RentCarDbContext(options);
            _session = new DummySession();
            _bookingController = new BookingController(null,_dbContext)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        Session = _session
                    }
                }
            };

            SeedDatabase();
        }

        [Test, MaxTime(2000)]
        public void ViewBookingListGet_CheckNotNullForView()
        {
            var user = _dbContext.Users.Find(2);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _bookingController.ViewBookingList() as ViewResult;
            var viewBagCars = _bookingController.ViewData["Bookings"] as IEnumerable<Booking>;

            // Assert
            Assert.IsNotNull(viewBagCars);
            Assert.IsNotNull(result);
        }

        [Test, MaxTime(2000)]
        public void ViewBookingListGet_ReturnsExpectedTotalBookings()
        {
            var user = _dbContext.Users.Find(2);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _bookingController.ViewBookingList() as ViewResult;
            var viewBagCars = _bookingController.ViewData["Bookings"] as IEnumerable<Booking>;
            int totalBookings = viewBagCars?.Count() ?? 0;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(viewBagCars);
            Assert.That(totalBookings, Is.EqualTo(3));
        }

        [Test, MaxTime(2000)]
        public void ViewBookingListGet_ReturnsExpectedOngoingBookings()
        {
            var user = _dbContext.Users.Find(2);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _bookingController.ViewBookingList() as ViewResult;
            var numsBookingOngoing = _bookingController.ViewData["Count"];

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(numsBookingOngoing);
            Assert.That(numsBookingOngoing, Is.EqualTo(2));
        }

        [Test, MaxTime(2000)]
        public void ViewBookingListGet_ReturnsViewError()
        {
            var user = _dbContext.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _bookingController.ViewBookingList() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.ViewName, Is.EqualTo("ErrorAuthorization"));
        }

        [Test, MaxTime(2000)]
        public void ViewBookingListGet_DoNotLogin_RedirectToLogin()
        {
            // Act
            var result = _bookingController.ViewBookingList() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.ActionName, Is.EqualTo("Login"));
            Assert.That(result.ControllerName, Is.EqualTo("Users"));
        }

        [Test, MaxTime(2000)]
        [TestCase("latest")]
        [TestCase("newest")]
        [TestCase("highest")]
        [TestCase("lowest")]
        public void ViewBookingListPost_ReturnSortOrderBooking(string sortOrder)
        {
            var user = _dbContext.Users.Find(2);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _bookingController.ViewBookingList(sortOrder) as ViewResult;
            var bookings = (result.ViewData["Bookings"] as IEnumerable)?.Cast<dynamic>().ToList();
            var actualSortOrder = result.ViewData["SortOrder"];

            // Assert
            Assert.IsNotNull(bookings);
            Assert.IsNotNull(actualSortOrder);
            Assert.That(actualSortOrder.ToString(), Is.EqualTo(sortOrder));
        }

        [Test, MaxTime(2000)]
        [TestCase("latest")]
        [TestCase("newest")]
        [TestCase("highest")]
        [TestCase("lowest")]
        public void ViewBookingListPost_ReturnTotalBooking(string sortOrder)
        {
            var user = _dbContext.Users.Find(2);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _bookingController.ViewBookingList(sortOrder) as ViewResult;
            var bookings = (result.ViewData["Bookings"] as IEnumerable)?.Cast<dynamic>().ToList();

            // Assert
            Assert.IsNotNull(bookings);
            Assert.That(bookings.Count(), Is.EqualTo(3));
        }

        [Test, MaxTime(2000)]
        [TestCase("latest")]
        [TestCase("newest")]
        [TestCase("highest")]
        [TestCase("lowest")]
        public void ViewBookingListPost_ReturnOngoingBooking(string sortOrder)
        {
            var user = _dbContext.Users.Find(2);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _bookingController.ViewBookingList(sortOrder) as ViewResult;
            var numsBookingOngoing = _bookingController.ViewData["Count"];
            // Assert
            Assert.IsNotNull(result);
            Assert.That(numsBookingOngoing, Is.EqualTo(2));
        }

        [Test, MaxTime(2000)]
        [TestCase("latest")]
        [TestCase("newest")]
        [TestCase("highest")]
        [TestCase("lowest")]
        public void ViewBookingListPost_LoginNewAccount_ZeroViewBagBookings(string sortOrder)
        {
            var user = _dbContext.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _bookingController.ViewBookingList(sortOrder) as ViewResult;
            var numsBooking = _bookingController.ViewData["Bookings"] as IEnumerable<Booking>; ;

            // Assert
            Assert.IsNotNull (result);
            Assert.IsEmpty(numsBooking);

        }

        [Test, MaxTime(2000)]
        [TestCase("latest")]
        [TestCase("newest")]
        [TestCase("highest")]
        [TestCase("lowest")]
        public void ViewBookingListPost_LoginNewAccount_ZeroViewBagCount(string sortOrder)
        {
            var user = _dbContext.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _bookingController.ViewBookingList(sortOrder) as ViewResult;
            var numsBookingOngoing = _bookingController.ViewData["Count"];

            // Assert
            Assert.That(numsBookingOngoing, Is.EqualTo(0));

        }

        [Test, MaxTime(2000)]
        [TestCase("2024-01-01", "2024-03-01", 60)] // Expected result is 60 days
        [TestCase("2024-01-01", "2023-03-01", -1)] // Expected result is -1 (end date before start date)
        [TestCase("2024-01-01", "2024-01-01", -1)] // Expected result is -1 (same start and end date)
        [TestCase("2024-02-28", "2024-03-01", 2)] // Leap year consideration, expected 2 days
        [TestCase("2024-12-31", "2025-01-01", 1)] // End of year, expected 1 day
        [TestCase("2020-01-01", "2024-01-01", 1461)] // Long date range over several years
        [TestCase("2024-01-01", "2024-01-02", 1)] // One day difference
        [TestCase("2024-01-01", "2024-01-10", 9)] // Short date range
        [TestCase("2024-01-01", "2024-01-01T18:00:00", 1)] // End date is today
        public void GetDayBetween_ReturnValueOfDay(string startDateString, string endDateString, int expected)
        {
            // Arrange
            DateTime startDate = DateTime.Parse(startDateString);
            DateTime endDate = DateTime.Parse(endDateString);

            // Act
            var result = BookingHelper.GetDaysBetween(startDate, endDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, MaxTime(2000)]
        [TestCase(100, "2024-01-01", "2023-03-01", -1)] // End date before start date
        [TestCase(100, "2024-01-01", "2024-01-01", -1)] // Same start and end date
        [TestCase(100, "2024-02-28", "2024-03-01", 250)] // Leap year
        [TestCase(100, "2024-12-31", "2025-01-01", 150)] // Across year boundary
        [TestCase(100, "2020-01-01", "2024-01-01", 146150)] // Long date range
        [TestCase(100, "2024-01-01", "2024-01-02", 150)] // Single day rental
        [TestCase(0, "2024-01-01", "2024-01-10", -1)] // Zero base price
        [TestCase(-100, "2024-01-01", "2024-01-10", -1)] // Negative base price
        [TestCase(100, "2024-01-01", "2024-01-01T18:00:00", 100)] // End date is today
        public void GetTotalPrice_ReturnsExpectedResult(decimal baseprice, string startDateString, string endDateString, decimal expected)
        {
            // Arrange
            DateTime startDate = DateTime.Parse(startDateString);
            DateTime endDate = DateTime.Parse(endDateString);

            // Act
            var result = BookingHelper.GetTotalPrice(baseprice, startDate, endDate);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, MaxTime(2000)]
        [TestCase(100, "2024-01-01", "2023-03-01", -1)] // End date before start date
        [TestCase(100, "2024-01-01", "2024-01-01", -1)] // Same start and end date
        [TestCase(100, "2024-02-28", "2024-03-01", 250)] // Leap year
        [TestCase(100, "2024-12-31", "2025-01-01", 150)] // Across year boundary
        [TestCase(100, "2020-01-01", "2024-01-01", 146150)] // Long date range
        [TestCase(100, "2024-01-01", "2024-01-02", 150)] // Single day rental
        [TestCase(0, "2024-01-01", "2024-01-10", -1)] // Zero deposit
        [TestCase(-100, "2024-01-01", "2024-01-10", -1)] // Negative deposit
        [TestCase(100, "2024-01-01", "2024-01-01T18:00:00", 100)] // End date is today
        public void GetTotalDeposit_ReturnsExpectedResult(decimal deposit, string startDateString, string endDateString, decimal expected)
        {
            // Arrange
            DateTime startDate = DateTime.Parse(startDateString);
            DateTime endDate = DateTime.Parse(endDateString);

            // Act
            var result = BookingHelper.GetTotalDeposit(deposit, startDate, endDate);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, MaxTime(2000)]
        [TestCase(100, "2024-01-01T15:00:00", "2024-03-01T18:00:00", 6050)] // Valid date range
        [TestCase(100, "2024-01-01", "2023-03-01", -1)] // End date in the past
        [TestCase(100, "2024-01-01", "2024-01-01", -1)] // Same start and end date
        [TestCase(100, "2024-02-28T18:00:00", "2024-03-01T17:00:00", 200)] // Leap year
        [TestCase(100, "2020-01-01", "2024-01-01", 146150)] // Long date range ending in the past
        [TestCase(100, "2024-01-01", "2024-01-02", 150)] // Single day rental
        [TestCase(0, "2024-01-01", "2024-01-10", -1)] // Zero base price
        [TestCase(-100, "2024-01-01", "2024-01-10", -1)] // Negative base price
        [TestCase(100, "2024-01-01", "2023-01-01", -1)] // End date before start date
        [TestCase(100, "2024-01-01", "2023-12-31", -1)] // End date just before start date
        [TestCase(100, "2024-02-28", "2024-03-01", 250)] // Leap day rental
        [TestCase(100, "2024-01-01", "2024-01-01T18:00:00", 100)] // End date is the same day but with time
        public void GetTotalPriceFromToday_ReturnsExpectedResult(decimal baseprice, string startDateString, string endDateString, decimal expected)
        {
            // Arrange
            DateTime startDate = DateTime.Parse(startDateString);
            DateTime endDate = DateTime.Parse(endDateString);

            // Act
            var result = BookingHelper.GetTotalPriceFromToday(baseprice, startDate, endDate);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }


        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _bookingController.Dispose();
            _dbContext.Dispose();
        }
        private void SeedDatabase()
        {
            var bookingData = new List<Booking>
            {
                new Booking{ BookingNo = 1, UserId = 2, CarId = 1, StartDate =  new DateTime(2024, 06, 22), EndDate =  new DateTime(2024, 06, 23), PaymentMethod = 1, Status = 3 },
                new Booking { BookingNo = 2, UserId = 2, CarId = 2, StartDate =  new DateTime(2024, 07, 22), EndDate =  new DateTime(2024, 08, 02), PaymentMethod = 2, Status = 3 },
                new Booking { BookingNo = 3, UserId = 2, CarId = 2, StartDate =  new DateTime(2024, 07, 22), EndDate =  new DateTime(2024, 08, 02), PaymentMethod = 2, Status = 5 }

            };
            var userData = new List<User>
            {
                new User { UserId = 1, Name = "User A",Role = true, Email = "duyquan7b@gmail.com", Password ="quandeptrai123",Phone="0987654321" },
                new User { UserId = 2, Name = "User B", Role = false,Email = "duyquan7b1@gmail.com", Password ="quandeptrai123", Phone="0987654321" },
                new User { UserId = 3, Name = "User C", Role = false,Email = "duyquan7b2@gmail.com", Password ="quandeptrai123", Phone="0987654321" }
            };
            var carData = new List<Car>
            {
                new Car {CarId = 1,UserId = 1, Name = "Acura ILX 2000", LicensePlate = "56F-513.11", BrandId = 1, ModelId = 1, Seats = 4,
                ColorId = 1,FrontImage = "Image1.jpg",BackImage ="Image2.jpg",LeftImage ="Image1.jpg", RightImage ="Image1.jpg",
                ProductionYear = 2000,TransmissionType = true, FuelType = true, Mileage = 200, FuelConsumption = 10,
                BasePrice = 1000000, Deposit = 500000,AddressId = 1, Description = "NOG NOG 1", DocumentId = 1,
                TermId = 1, FucntionId = 1, Status = 1, NoOfRide = 1
                },
                new Car {CarId = 2,UserId = 1, Name = "Acura MDX 2003", LicensePlate = "56F-513.22", BrandId = 1, ModelId = 2, Seats = 10,
                ColorId = 2,FrontImage = "Image2.jpg",BackImage ="Image2.jpg",LeftImage ="Image2.jpg", RightImage ="Image2.jpg",
                ProductionYear = 2003,TransmissionType = false, FuelType = false, Mileage = 100, FuelConsumption = 8,
                BasePrice = 1500000, Deposit = 500000,AddressId = 2, Description = "NOG NOG 2", DocumentId = 1,
                TermId = 2, FucntionId = 2, Status = 1, NoOfRide = 2
                },
                new Car {CarId = 3,UserId = 1, Name = "Acura MDX 2003", LicensePlate = "56F-513.33", BrandId = 1, ModelId = 2, Seats = 10,
                ColorId = 2,FrontImage = "Image2.jpg",BackImage ="Image2.jpg",LeftImage ="Image2.jpg", RightImage ="Image2.jpg",
                ProductionYear = 2003,TransmissionType = false, FuelType = false, Mileage = 100, FuelConsumption = 8,
                BasePrice = 1200000, Deposit = 500000,AddressId = 2, Description = "NOG NOG 3", DocumentId = 1,
                TermId = 2, FucntionId = 2, Status = 1, NoOfRide = 2
                },
            };
            _dbContext.Users.AddRange(userData);
            _dbContext.Cars.AddRange(carData);
            _dbContext.Bookings.AddRange(bookingData);
            _dbContext.SaveChanges();
        }

        public class DummySession : ISession
        {
            private readonly Dictionary<string, byte[]> _sessionStore = new Dictionary<string, byte[]>();

            public DummySession()
            {
            }

            // Phương thức này để lấy giá trị string từ session
            public string? GetString(string key)
            {
                if (_sessionStore.TryGetValue(key, out var value))
                {
                    return System.Text.Encoding.UTF8.GetString(value);
                }
                return null;
            }

            public void Clear() => _sessionStore.Clear();
            public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public void Dispose() { }
            public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public void Remove(string key) => _sessionStore.Remove(key);
            public void Set(string key, byte[] value) => _sessionStore[key] = value;
            public bool TryGetValue(string key, out byte[] value) => _sessionStore.TryGetValue(key, out value);
            public IEnumerable<string> Keys => _sessionStore.Keys;

            public bool IsAvailable => throw new NotImplementedException();

            public string Id => throw new NotImplementedException();
        }
    }
}
