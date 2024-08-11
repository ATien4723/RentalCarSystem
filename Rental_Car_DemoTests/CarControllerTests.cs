//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using Rental_Car_Demo.Controllers;
//using Rental_Car_Demo.Models;
//using Rental_Car_Demo.Repository.CarRepository;
//using Rental_Car_Demo.Services;
//using System.Text;
//using System.Security.Cryptography;
//using Microsoft.CodeAnalysis;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
//using Microsoft.AspNetCore.Http;

//namespace Rental_Car_Demo.Tests
//{
//    [TestFixture]
//    public class CarControllerTests
//    {
//        private CarController _controller;
//        private RentCarDbContext _context;
//        private Mock<IEmailService> _mockEmailService;
//        private Mock<ITempDataDictionary> _mockTempData;
//        private Mock<ICarRepository> _mockCarRepository;
//        private DummySession _dummySession;
//        private DefaultHttpContext _httpContext;


//        [SetUp]
//        public void SetUp()
//        {
//            var options = new DbContextOptionsBuilder<RentCarDbContext>()
//                .UseInMemoryDatabase(databaseName: "TestDatabase")
//                .Options;

//            _mockEmailService = new Mock<IEmailService>();
//            _mockCarRepository = new Mock<ICarRepository>();
//            _mockTempData = new Mock<ITempDataDictionary>();

//            _context = new RentCarDbContext(options);

//            // Initialize DummySession
//            _dummySession = new DummySession();

//            // Initialize HttpContext and assign DummySession
//            _httpContext = new DefaultHttpContext();
//            _httpContext.Session = _dummySession;

//            _controller = new CarController(_mockCarRepository.Object,_context, _mockEmailService.Object)
//            {
//                TempData = _mockTempData.Object
//            };

//            _controller.ControllerContext.HttpContext = _httpContext;

//            SeedDatabase();

//        }

//        private void SeedDatabase()
//        {
//            var cityData = new List<City>
//            {
//                new City { CityId = 1, CityProvince = "Thành phố Hà Nội"},
//                new City { CityId = 2, CityProvince = "Tỉnh Hà Giang"},
//                new City { CityId = 3, CityProvince = "Tỉnh Cao Bằng"},
//                new City { CityId = 4, CityProvince = "Tỉnh Bắc Kạn"},
//            };

//            var districtData = new List<District>
//            {
//                new District { CityId = 1, DistrictName = "Quận Ba Đình", DistrictId = 1},
//                new District { CityId = 2, DistrictName = "Huyện Đồng Văn", DistrictId = 2},
//                new District { CityId = 3, DistrictName = "Huyện Bảo Lâm", DistrictId = 3},
//                new District { CityId = 4, DistrictName = "Huyện Bạch Thông", DistrictId = 4},
//            };

//            var wardData = new List<Ward>
//            {
//                new Ward { WardId = 1, WardName = "Phường Phúc Xá", DistrictId = 1},
//                new Ward { WardId = 2, WardName = "Phường Phúc Tân", DistrictId = 2},
//                new Ward { WardId = 3, WardName = "Phường Phú Thượng", DistrictId = 3},
//                new Ward { WardId = 4, WardName = "Phường Thượng Thanh", DistrictId = 4},
//            };

//            var addressData = new List<Address>
//            {
//                new Address { AddressId = 1, CityId = 1, DistrictId = 1, WardId = 1, HouseNumberStreet = "Nha so 1"},
//                new Address { AddressId = 2, CityId = 2, DistrictId = 2, WardId = 2, HouseNumberStreet = "Nha so 2"},
//                new Address { AddressId = 3, CityId = 3, DistrictId = 3, WardId = 3, HouseNumberStreet = "Nha so 3"},
//                new Address { AddressId = 4, CityId = 4, DistrictId = 4, WardId = 4, HouseNumberStreet = "Nha so 4"},
//            };

//            var carModels = new List<CarModel>
//            {
//                new CarModel { ModelId = 1, ModelName = "Model X" },
//                new CarModel { ModelId = 2, ModelName = "Model Y" }
//            };


//            var carBrands = new List<CarBrand>
//            {
//                new CarBrand { BrandId = 1, BrandName = "Brand A" },
//                new CarBrand { BrandId = 2, BrandName = "Brand B" }
//            };

//            var TermOfUse = new List<TermOfUse>
//            {
//                new TermOfUse
//                {
//                    TermId = 1,
//                    NoFoodInCar = true,
//                    NoSmoking = true,
//                    NoPet = true,
                   
//                },

//                new TermOfUse
//                {
//                    TermId = 2,
//                    NoSmoking = true
//                }
//            };

//            var additionalFunction = new List<AdditionalFunction>
//            {
//                new AdditionalFunction
//                {
//                    FucntionId = 1,
//                    Gps = true,
//                    Camera = true,
                    
//                },
//                new AdditionalFunction
//                {
//                    FucntionId = 2,
//                    SunRoof = true,
//                    Usb = true

//                }
//            };

//            var carDocument = new List<CarDocument>
//            {
//                new CarDocument
//                {
//                    DocumentId = 1,
//                    Registration = "Registration1",
//                    Certificate = "Certificate1"
//                },

//                new CarDocument
//                {
//                    DocumentId = 2,
//                    Registration = "Registration2",
//                    Certificate = "Certificate2"
//                },

//            };

//            var color = new List<CarColor>
//            {
//                new CarColor
//                {
//                    ColorId = 1,
//                    ColorName = "den"
//                },
//                new CarColor
//                {
//                    ColorId = 2,
//                    ColorName = "trang"
//                }
//            };

//            var users = new List<User>
//            {
//                new User
//                {
//                    Email = "nvutuankiet2003@gmail.com",
//                    Password = HashPassword("kiet123"),
//                    Name = "kiet ne",
//                    Phone = "0334567890",
//                    Role = false,
//                    Wallet = 0
//                },
//                new User
//                {
//                    Email = "hehe@gmail.com",
//                    Password = HashPassword("hehe123"),
//                    Name = "hehe",
//                    Phone = "0987654321",
//                    Role = true,
//                    Wallet = 0
//                }
//            };

//            var cars = new List<Car>
//            {
//                new Car
//                {
//                    CarId = 1,
//                    UserId = 1,
//                    Name = "Car 1",
//                    LicensePlate = "50F-567.89",
//                    BrandId = 1,
//                    ModelId = 1,
//                    Seats = 10,
//                    FrontImage = "front1.jpg",
//                    BackImage = "back1.jpg",
//                    LeftImage = "left1.jpg",
//                    RightImage = "right1.jpg",
//                    ProductionYear = 2024,
//                    TransmissionType = true,
//                    FuelType = true,
//                    Mileage = 50000,
//                    FuelConsumption = 100,
//                    BasePrice = 15000000,
//                    Deposit = 5000000,
//                    ColorId = 1,
//                    AddressId = 1,
//                    Description = "Description 1",
//                    DocumentId = 1,
//                    TermId = 1,
//                    FucntionId = 1,
//                    Status = 1,
//                    NoOfRide = 1
//                },

//                new Car
//                {
//                    CarId = 2,
//                    UserId = 2,
//                    Name = "Car 2",
//                    LicensePlate = "50F-567.90",
//                    BrandId = 2,
//                    ModelId = 2,
//                    Seats = 4,
//                    FrontImage = "front2.jpg",
//                    BackImage = "back2.jpg",
//                    LeftImage = "left2.jpg",
//                    RightImage = "right2.jpg",
//                    ProductionYear = 2024,
//                    TransmissionType = false,
//                    FuelType = false,
//                    Mileage = 30000,
//                    FuelConsumption = 200,
//                    BasePrice = 35000000,
//                    Deposit = 3000000,
//                    ColorId = 2,
//                    AddressId = 2,
//                    Description = "Description 2",
//                    DocumentId = 2,
//                    TermId = 2,
//                    FucntionId = 2,
//                    Status = 2,
//                    NoOfRide = 2
//                }
//            };

//            _context.Users.AddRange(users);
//            _context.Cities.AddRange(cityData);
//            _context.Districts.AddRange(districtData);
//            _context.Wards.AddRange(wardData);
//            _context.Addresses.AddRange(addressData);
//            _context.CarModels.AddRange(carModels);
//            _context.CarBrands.AddRange(carBrands);
//            _context.AdditionalFunctions.AddRange(additionalFunction);
//            _context.TermOfUses.AddRange(TermOfUse);
//            _context.CarDocuments.AddRange(carDocument);
//            _context.Cars.AddRange(cars);
//            _context.SaveChanges();
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _context.Database.EnsureDeleted();
//            _context.Dispose();
//            _controller.Dispose();
//        }

//        private string HashPassword(string password)
//        {
//            using (SHA256 sha256Hash = SHA256.Create())
//            {
//                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
//                StringBuilder builder = new StringBuilder();
//                for (int i = 0; i < bytes.Length; i++)
//                {
//                    builder.Append(bytes[i].ToString("x2"));
//                }
//                return builder.ToString();
//            }
//        }

//        [Test]
//        public void ChangeCarDetailsByOwner_ShouldReturnNotFound_WhenCarDoesNotExist()
//        {
//            // Arrange
//            var carId = 999; // An ID that does not exist

//            // Act
//            var result = _controller.ChangeCarDetailsByOwner(carId) as ViewResult;

//            // Assert
//            Assert.AreEqual("NotFound", result.ViewName, "The view name is incorrect");
//        }

//        [Test]
//        public void ChangeCarDetailsByOwner_ShouldReturnErrorAuthorization_WhenUserNotAuthorized()
//        {
//            // Arrange
//            var carId = 1;
//            var user = new User { UserId = 2, Email = "test@test.com", Password = "hashedpassword", Role = false };

//            var userString = JsonConvert.SerializeObject(user);
//            _dummySession.SetString("User", userString);


//            // Act
//            var result = _controller.ChangeCarDetailsByOwner(carId) as ViewResult;

//            // Assert
//            Assert.AreEqual("ErrorAuthorization", result.ViewName);
//        }

//        [Test]
//        public void ChangeCarDetailsByOwner_ShouldReturnView_WhenAuthorizedAndCarExists_WithRentals()
//        {
//            // Arrange
//            var carId = 1;


//            var user = new User { UserId = 1, Email = "test@test.com", Password = "hashedpassword", Role = false };

//            var userString = JsonConvert.SerializeObject(user);
//            _dummySession.SetString("User", userString);

//            var bookings = new List<Booking>
//        {
//            new Booking { CarId = carId, UserId = user.UserId, Status = 3 }
//        };

//            var feedbacks = new List<Feedback>
//        {
//            new Feedback { FeedbackId = 1, BookingNo = 1, Ratings = 4, Content = "Good", Date = DateTime.Now }
//        };
//            _context.AddRange(feedbacks);
//            _context.AddRange(bookings);
//            _context.SaveChanges();


//            // Act
//            var result = _controller.ChangeCarDetailsByOwner(carId) as ViewResult;

//            // Assert
//            Assert.IsNotNull(result);
//        }


//        [Test]
//        [TestCase(false, false, true, "No loud music", 20000000, 5000000)]
//        [TestCase(false, false, false, "abc", 20000000, 5000000)]
//        [TestCase(false, true, true, "xyz", 20000000, 5000000)]
//        [TestCase(true, true, true, "aaa", 20000000, 5000000)]
//        [TestCase(true, false, true, "No loud music", 20000000, 5000000)]
//        [TestCase(true, false, false, "No loud music", 20000000, 5000000)]
//        [TestCase(true, true, false, null, 20000000, 5000000)]
//        [TestCase(false, true, false, null, -20000000, 5000000)]

//        public void ChangeCarTermsByOwner_ShouldUpdateCarAndTerms_WhenDataIsValid(bool smoking, bool food, bool pet, string? specify, int basePrice, int deposit)
//        {
//            // Arrange
//            var car = _context.Cars.FirstOrDefault(c => c.CarId == 1);

//            car.BasePrice = basePrice;
//            car.Deposit = deposit;

//            // Act
//            var result = _controller.ChangeCarTermsByOwner(car, smoking, food, pet, specify) as RedirectToActionResult;

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual("ChangeCarDetailsByOwner", result.ActionName);
//            Assert.AreEqual(1, result.RouteValues["CarId"]);

//            var updatedCar = _context.Cars.FirstOrDefault(c => c.CarId == 1);
//            Assert.IsNotNull(updatedCar);
//            Assert.AreEqual(basePrice, updatedCar.BasePrice);
//            Assert.AreEqual(deposit, updatedCar.Deposit);

//            var updatedTerms = _context.TermOfUses.FirstOrDefault(t => t.TermId == updatedCar.TermId);
//            Assert.IsNotNull(updatedTerms);
//            Assert.AreEqual(updatedTerms.NoSmoking, smoking);
//            Assert.AreEqual(updatedTerms.NoFoodInCar, food);
//            Assert.AreEqual(updatedTerms.NoPet, pet);
//            Assert.AreEqual(specify, updatedTerms.Specify);
//        }



//        [Test]
//        [TestCase(1, 2)]
//        [TestCase(2, 1)]
//        public void ChangeCarStatus_ShouldUpdateCarStatus_AndRedirectToChangeCarDetailsByOwner(int carId,int status)
//        {

//            var car = new Car
//            {
//                CarId = carId,
//                Status = status 
//            };

//            // Act
//            var result = _controller.ChangeCarStatus(car) as RedirectToActionResult;

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual("ChangeCarDetailsByOwner", result.ActionName);
//            Assert.AreEqual(car.CarId, result.RouteValues["CarId"]);

//            var updatedCar = _context.Cars.FirstOrDefault(c => c.CarId == car.CarId);
//            Assert.IsNotNull(updatedCar);
//            Assert.AreEqual(status, updatedCar.Status);

//        }

//        [Test]
//        public void ConfirmDeposit_ShouldUpdateBookingStatus_AndRedirectToChangeCarDetailsByOwner()
//        {
//            // Arrange
//            var car = new Car
//            {
//                CarId = 1
//            };

//            // Ensure the booking exists in the database
//            var existingBooking = new Booking
//            {
//                BookingInfoId = 1,
//                CarId = car.CarId,
//                Status = 1, // Status before confirmation
//                UserId = 1,
//                BookingNo = 1,
//                StartDate = DateTime.Now,
//                EndDate = DateTime.Now
//            };

//            _context.Bookings.Add(existingBooking);
//            _context.SaveChanges();

//            // Act
//            var result = _controller.ConfirmDeposit(car) as RedirectToActionResult;

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual("ChangeCarDetailsByOwner", result.ActionName);
//            Assert.AreEqual(car.CarId, result.RouteValues["CarId"]);

//            var updatedBooking = _context.Bookings.FirstOrDefault(b => b.BookingInfoId == existingBooking.BookingInfoId);
//            Assert.IsNotNull(updatedBooking);
//            Assert.AreEqual(2, updatedBooking.Status); // Status after confirmation
//        }


//        }

//}
