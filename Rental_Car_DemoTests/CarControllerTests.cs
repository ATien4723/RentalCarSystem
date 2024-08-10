using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using Rental_Car_Demo.Controllers;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.CarRepository;
using Rental_Car_Demo.Services;
using System.Text;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Rental_Car_Demo.Tests
{
    [TestFixture]
    public class CarControllerTests
    {
        private CarController _controller;
        private RentCarDbContext _context;
        private Mock<IEmailService> _mockEmailService;
        private Mock<ITempDataDictionary> _mockTempData;
        private Mock<ICarRepository> _mockCarRepository;
        private DummySession _dummySession;
        private DefaultHttpContext _httpContext;


        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<RentCarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockEmailService = new Mock<IEmailService>();
            _mockCarRepository = new Mock<ICarRepository>();
            _mockTempData = new Mock<ITempDataDictionary>();

            _context = new RentCarDbContext(options);

            // Initialize DummySession
            _dummySession = new DummySession();

            // Initialize HttpContext and assign DummySession
            _httpContext = new DefaultHttpContext();
            _httpContext.Session = _dummySession;

            _controller = new CarController(_mockCarRepository.Object, _context, _mockEmailService.Object)
            {
                TempData = _mockTempData.Object
            };

            _controller.ControllerContext.HttpContext = _httpContext;

            SeedDatabase();

        }

        private void SeedDatabase()
        {
            var cityData = new List<City>
            {
                new City { CityId = 1, CityProvince = "Thành phố Hà Nội"},
                new City { CityId = 2, CityProvince = "Tỉnh Hà Giang"},
                new City { CityId = 3, CityProvince = "Tỉnh Cao Bằng"},
                new City { CityId = 4, CityProvince = "Tỉnh Bắc Kạn"},
            };

            var districtData = new List<District>
            {
                new District { CityId = 1, DistrictName = "Quận Ba Đình", DistrictId = 1},
                new District { CityId = 2, DistrictName = "Huyện Đồng Văn", DistrictId = 2},
                new District { CityId = 3, DistrictName = "Huyện Bảo Lâm", DistrictId = 3},
                new District { CityId = 4, DistrictName = "Huyện Bạch Thông", DistrictId = 4},
            };

            var wardData = new List<Ward>
            {
                new Ward { WardId = 1, WardName = "Phường Phúc Xá", DistrictId = 1},
                new Ward { WardId = 2, WardName = "Phường Phúc Tân", DistrictId = 2},
                new Ward { WardId = 3, WardName = "Phường Phú Thượng", DistrictId = 3},
                new Ward { WardId = 4, WardName = "Phường Thượng Thanh", DistrictId = 4},
            };

            var addressData = new List<Address>
            {
                new Address { AddressId = 1, CityId = 1, DistrictId = 1, WardId = 1, HouseNumberStreet = "Nha so 1"},
                new Address { AddressId = 2, CityId = 2, DistrictId = 2, WardId = 2, HouseNumberStreet = "Nha so 2"},
                new Address { AddressId = 3, CityId = 3, DistrictId = 3, WardId = 3, HouseNumberStreet = "Nha so 3"},
                new Address { AddressId = 4, CityId = 4, DistrictId = 4, WardId = 4, HouseNumberStreet = "Nha so 4"},
            };

            var carModels = new List<CarModel>
            {
                new CarModel { ModelId = 1, ModelName = "Model X" },
                new CarModel { ModelId = 2, ModelName = "Model Y" }
            };


            var carBrands = new List<CarBrand>
            {
                new CarBrand { BrandId = 1, BrandName = "Brand A" },
                new CarBrand { BrandId = 2, BrandName = "Brand B" }
            };

            var TermOfUse = new List<TermOfUse>
            {
                new TermOfUse
                {
                    TermId = 1,
                    NoFoodInCar = true,
                    NoSmoking = true,
                    NoPet = true,

                },

                new TermOfUse
                {
                    TermId = 2,
                    NoSmoking = true
                }
            };

            var additionalFunction = new List<AdditionalFunction>
            {
                new AdditionalFunction
                {
                    FucntionId = 1,
                    Gps = true,
                    Camera = true,

                },
                new AdditionalFunction
                {
                    FucntionId = 2,
                    SunRoof = true,
                    Usb = true

                }
            };

            var carDocument = new List<CarDocument>
            {
                new CarDocument
                {
                    DocumentId = 1,
                    Registration = "Registration1",
                    Certificate = "Certificate1"
                },

                new CarDocument
                {
                    DocumentId = 2,
                    Registration = "Registration2",
                    Certificate = "Certificate2"
                },

            };

            var color = new List<CarColor>
            {
                new CarColor
                {
                    ColorId = 1,
                    ColorName = "den"
                },
                new CarColor
                {
                    ColorId = 2,
                    ColorName = "trang"
                }
            };

            var feedbacks = new List<Feedback>
            {
                new Feedback { FeedbackId = 1, BookingNo = 1, Ratings = 3, Content = "Good", Date = DateTime.Now },
                new Feedback { FeedbackId = 2, BookingNo = 2, Ratings = 5, Content = "Good", Date = DateTime.Now },
                new Feedback { FeedbackId = 3, BookingNo = 4, Ratings = -1, Content = "Good", Date = DateTime.Now }

            };



            var bookings = new List<Booking>
            {
                new Booking { BookingNo = 1, UserId = 2, CarId = 1, StartDate = DateTime.Now, EndDate = DateTime.Now },
                new Booking { BookingNo = 2, UserId = 3, CarId = 1, StartDate = DateTime.Now, EndDate = DateTime.Now },
                new Booking { BookingNo = 3, UserId = 1, CarId = 3, StartDate = DateTime.Now, EndDate = DateTime.Now },
                new Booking { BookingNo = 4, UserId = 2, CarId = 3, StartDate = DateTime.Now, EndDate = DateTime.Now }
            };

            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Email = "nvutuankiet2003@gmail.com",
                    Password = HashPassword("kiet123"),
                    Name = "kiet ne",
                    Phone = "0334567890",
                    Role = false,
                    Wallet = 0
                },
                new User
                {
                    UserId = 2,
                    Email = "hehe@gmail.com",
                    Password = HashPassword("hehe123"),
                    Name = "hehe",
                    Phone = "0987654321",
                    Role = true,
                    Wallet = 0
                },
                new User
                {
                    UserId = 3,
                    Email = "kakaka@gmail.com",
                    Password = HashPassword("kaka123"),
                    Name = "hehe",
                    Phone = "0987654321",
                    Role = true,
                    Wallet = 0
                }
            };

            var cars = new List<Car>
            {
                new Car
                {
                    CarId = 1,
                    UserId = 1,
                    Name = "Car 1",
                    LicensePlate = "50F-567.89",
                    BrandId = 1,
                    ModelId = 1,
                    Seats = 10,
                    FrontImage = "front1.jpg",
                    BackImage = "back1.jpg",
                    LeftImage = "left1.jpg",
                    RightImage = "right1.jpg",
                    ProductionYear = 2024,
                    TransmissionType = true,
                    FuelType = true,
                    Mileage = 50000,
                    FuelConsumption = 100,
                    BasePrice = 15000000,
                    Deposit = 5000000,
                    ColorId = 1,
                    AddressId = 1,
                    Description = "Description 1",
                    DocumentId = 1,
                    TermId = 1,
                    FucntionId = 1,
                    Status = 1,
                    NoOfRide = 0
                },

                new Car
                {
                    CarId = 2,
                    UserId = 2,
                    Name = "Car 2",
                    LicensePlate = "50F-567.90",
                    BrandId = 2,
                    ModelId = 2,
                    Seats = 4,
                    FrontImage = "front2.jpg",
                    BackImage = "back2.jpg",
                    LeftImage = "left2.jpg",
                    RightImage = "right2.jpg",
                    ProductionYear = 2024,
                    TransmissionType = false,
                    FuelType = false,
                    Mileage = 30000,
                    FuelConsumption = 200,
                    BasePrice = 35000000,
                    Deposit = 3000000,
                    ColorId = 2,
                    AddressId = 2,
                    Description = "Description 2",
                    DocumentId = 2,
                    TermId = 2,
                    FucntionId = 2,
                    Status = 2,
                    NoOfRide = 0
                },
                new Car
                {
                    CarId = 3,
                    UserId = 3,
                    Name = "Car 3",
                    LicensePlate = "50F-567.92",
                    BrandId = 2,
                    ModelId = 2,
                    Seats = 4,
                    FrontImage = "front3.jpg",
                    BackImage = "back3.jpg",
                    LeftImage = "left3.jpg",
                    RightImage = "right3.jpg",
                    ProductionYear = 2024,
                    TransmissionType = false,
                    FuelType = false,
                    Mileage = 30000,
                    FuelConsumption = 200,
                    BasePrice = 35000000,
                    Deposit = 3000000,
                    ColorId = 2,
                    AddressId = 2,
                    Description = "Description 3",
                    DocumentId = 2,
                    TermId = 2,
                    FucntionId = 2,
                    Status = 2,
                    NoOfRide = 0
                }

            };

            _context.Users.AddRange(users);
            _context.Cities.AddRange(cityData);
            _context.Districts.AddRange(districtData);
            _context.Wards.AddRange(wardData);
            _context.Addresses.AddRange(addressData);
            _context.CarModels.AddRange(carModels);
            _context.CarBrands.AddRange(carBrands);
            _context.AdditionalFunctions.AddRange(additionalFunction);
            _context.TermOfUses.AddRange(TermOfUse);
            _context.CarDocuments.AddRange(carDocument);
            _context.Cars.AddRange(cars);
            _context.Feedbacks.AddRange(feedbacks);
            _context.Bookings.AddRange(bookings);

            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _controller.Dispose();
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
        public void ChangeCarDetailsByOwner_ShouldReturnNotFound_WhenCarDoesNotExist()
        {
            // Arrange
            var carId = 999; // An ID that does not exist

            // Act
            var result = _controller.ChangeCarDetailsByOwner(carId) as ViewResult;

            // Assert
            Assert.AreEqual("NotFound", result.ViewName, "The view name is incorrect");
        }

        [Test]
        public void ChangeCarDetailsByOwner_ShouldReturnErrorAuthorization_WhenUserNotAuthorized()
        {
            // Arrange
            var carId = 1;
            var user = new User { UserId = 1, Email = "test@test.com", Password = "hashedpassword", Role = false };

            var userString = JsonConvert.SerializeObject(user);
            _dummySession.SetString("User", userString);


            // Act
            var result = _controller.ChangeCarDetailsByOwner(carId) as ViewResult;

            // Assert
            Assert.AreEqual("ErrorAuthorization", result.ViewName);
        }

        [Test]
        public void ChangeCarDetailsByOwner_ShouldReturnErrorAuthorization_WhenUserNotOwnCar()
        {
            // Arrange
            var carId = 1;
            var user = new User { UserId = 2, Email = "test@test.com", Password = "hashedpassword", Role = true };

            var userString = JsonConvert.SerializeObject(user);
            _dummySession.SetString("User", userString);


            // Act
            var result = _controller.ChangeCarDetailsByOwner(carId) as ViewResult;

            // Assert
            Assert.AreEqual("ErrorAuthorization", result.ViewName);
        }

        [Test]
        public void ChangeCarDetailsByOwner_ShouldReturnView_WhenAuthorizedAndCarExists_WithRentals()
        {
            // Arrange
            var carId = 3;


            var user = new User { UserId = 3, Email = "test@test.com", Password = "hashedpassword", Role = true };

            var userString = JsonConvert.SerializeObject(user);
            _dummySession.SetString("User", userString);

            // Act
            var result = _controller.ChangeCarDetailsByOwner(carId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void ChangeCarDetailsByOwner_ShouldReturnView()
        {
            // Arrange
            var carId = 2;


            var user = new User { UserId = 2, Email = "test@test.com", Password = "hashedpassword", Role = true };

            var userString = JsonConvert.SerializeObject(user);
            _dummySession.SetString("User", userString);

            // Act
            var result = _controller.ChangeCarDetailsByOwner(carId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        [TestCase(3, 3)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]

        public void ChangeCarDetailsByOwner_checkViewbag(int carId, int userId)
        {

            var user = new User { UserId = userId, Name = "kiet ne", Email = "test@test.com", Password = "hashedpassword", Role = true };
            var userString = JsonConvert.SerializeObject(user);
            _dummySession.SetString("User", userString);

            var result = _controller.ChangeCarDetailsByOwner(carId) as ViewResult;

            Assert.NotNull(result);

            var carOwner = _controller.ViewBag.CarOwner as User;
            Assert.NotNull(carOwner);
            Assert.AreEqual(user.UserId, carOwner.UserId);
        }

        [Test]
        [TestCase(false, false, true, "No loud music", 20000000, 5000000)]
        [TestCase(false, false, false, "abc", 20000000, 5000000)]
        [TestCase(false, true, true, "xyz", 20000000, 5000000)]
        [TestCase(true, true, true, "aaa", 20000000, 5000000)]
        [TestCase(true, false, true, "No loud music", 20000000, 5000000)]
        [TestCase(true, false, false, "No loud music", 20000000, 5000000)]
        [TestCase(true, true, false, null, 20000000, 5000000)]
        [TestCase(false, true, false, null, 20000000, 5000000)]

        public void ChangeCarTermsByOwner_ShouldUpdateCarAndTerms_WhenDataIsValid(bool smoking, bool food, bool pet, string? specify, int basePrice, int deposit)
        {
            // Arrange
            var car = _context.Cars.FirstOrDefault(c => c.CarId == 1);

            car.BasePrice = basePrice;
            car.Deposit = deposit;

            // Act
            var result = _controller.ChangeCarTermsByOwner(car, smoking, food, pet, specify) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("ChangeCarDetailsByOwner", result.ActionName);
            Assert.AreEqual(1, result.RouteValues["CarId"]);

            var updatedCar = _context.Cars.FirstOrDefault(c => c.CarId == 1);
            Assert.IsNotNull(updatedCar);
            Assert.AreEqual(basePrice, updatedCar.BasePrice);
            Assert.AreEqual(deposit, updatedCar.Deposit);

            var updatedTerms = _context.TermOfUses.FirstOrDefault(t => t.TermId == updatedCar.TermId);
            Assert.IsNotNull(updatedTerms);
            Assert.AreEqual(updatedTerms.NoSmoking, smoking);
            Assert.AreEqual(updatedTerms.NoFoodInCar, food);
            Assert.AreEqual(updatedTerms.NoPet, pet);
            Assert.AreEqual(specify, updatedTerms.Specify);
        }


        [Test]
        [TestCase(false, false, true, "No loud music", -20000000, 5000000)]
        [TestCase(false, false, true, "No loud music", -20000000, -5000000)]
        [TestCase(false, false, true, "No loud music", 20000000, -5000000)]
        public void ChangeCarTermsByOwner_ShouldUpdateCarAndTerms_WhenDataIsInValid(bool smoking, bool food, bool pet,
            string? specify, int basePrice, int deposit)
        {
            // Arrange
            var car = _context.Cars.FirstOrDefault(c => c.CarId == 1);

            car.BasePrice = basePrice;
            car.Deposit = deposit;

            // Act
            var result = _controller.ChangeCarTermsByOwner(car, smoking, food, pet, specify) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.ViewName);
        }

        [Test]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        public void ChangeCarStatus_ShouldUpdateCarStatus_AndRedirectToChangeCarDetailsByOwner(int carId, int status)
        {

            var car = new Car
            {
                CarId = carId,
                Status = status
            };

            // Act
            var result = _controller.ChangeCarStatus(car) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("ChangeCarDetailsByOwner", result.ActionName);
            Assert.AreEqual(car.CarId, result.RouteValues["CarId"]);

            var updatedCar = _context.Cars.FirstOrDefault(c => c.CarId == car.CarId);
            Assert.IsNotNull(updatedCar);
            Assert.AreEqual(status, updatedCar.Status);

        }

        [Test]
        public void ConfirmDeposit_ShouldUpdateBookingStatus_AndRedirectToChangeCarDetailsByOwner()
        {
            // Arrange
            var car = new Car
            {
                CarId = 1
            };

            // Ensure the booking exists in the database
            var existingBooking = new Booking
            {
                BookingInfoId = 1,
                CarId = car.CarId,
                Status = 1, // Status before confirmation
                UserId = 1,
                BookingNo = 5,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };

            _context.Bookings.Add(existingBooking);
            _context.SaveChanges();

            // Act
            var result = _controller.ConfirmDeposit(car) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("ChangeCarDetailsByOwner", result.ActionName);
            Assert.AreEqual(car.CarId, result.RouteValues["CarId"]);

            var updatedBooking = _context.Bookings.FirstOrDefault(b => b.BookingInfoId == existingBooking.BookingInfoId);
            Assert.IsNotNull(updatedBooking);
            Assert.AreEqual(2, updatedBooking.Status); // Status after confirmation
        }




        [Test]
        
        public async Task ChangeCarDetailsByOwner_ValidRequest_UpdatesCarDetails([Values("front.jpg")] string frontFileName, [Values("back.jpg")] string backFileName, [Values("left.jpg")] string leftFileName, [Values("right.jpg")] string rightFileName,
            [Values(true, false)] bool Bluetooth,
            [Values(true, false)] bool GPS,
            [Values(true, false)] bool Camera,
            [Values(true, false)] bool Sunroof,
            [Values(true, false)] bool Childlock,
            [Values(true, false)] bool Childseat,
            [Values(true)] bool DVD, [Values(true)] bool USB,
            [Values(2)] int city, [Values(2)] int district, [Values(2)] int ward, [Values("456 Main St")] string street, [Values(true)] bool TransmissionType, [Values(true)] bool FuelType, [Values(15000)] double Mileage, [Values(15000)] double FuelConsumption, [Values(15000)] decimal BasePrice, [Values(15000)] decimal Deposit, [Values("NOG NOG")] string description)
        {

            var directoryPath = @"D:\FSOFTASSignment\Project_RentalCar\rental-car\Rental_Car_DemoTests\bin\Debug\net8.0\wwwroot\img\";
            //if (!Directory.Exists(directoryPath))
            //{
            //    Directory.CreateDirectory(directoryPath);
            //}

            // Arrange
            IFormFile CreateMockFormFile(string fileName)
            {
                var mockFile = new Mock<IFormFile>();
                var content = "Fake file content";
                var ms = new MemoryStream();
                var writer = new StreamWriter(ms);
                writer.Write(content);
                writer.Flush();
                ms.Position = 0;

                mockFile.Setup(f => f.FileName).Returns(fileName);
                mockFile.Setup(f => f.Length).Returns(ms.Length);
                mockFile.Setup(f => f.OpenReadStream()).Returns(ms);
                mockFile.Setup(f => f.ContentDisposition).Returns($"inline; filename={fileName}");
                return mockFile.Object;
            }

            var front = CreateMockFormFile(frontFileName);
            var back = CreateMockFormFile(backFileName);
            var left = CreateMockFormFile(leftFileName);
            var right = CreateMockFormFile(rightFileName);


            var updatedCar = new Car
            {
                CarId = 1,
                Mileage = Mileage,
                FuelConsumption = FuelConsumption,
                BasePrice = BasePrice,
                Deposit = Deposit,
                Description = description
            }; 


            // Act
            var result = await _controller.ChangeCarDetailsByOwner(
                updatedCar,
                front,
                back,
                left,
                right,
                Bluetooth, GPS, Camera, Sunroof, Childlock, Childseat, DVD, USB,
                city, district, ward, street) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("ChangeCarDetailsByOwner", result.ActionName);
            Assert.AreEqual(1, result.RouteValues["CarId"]);

            var carFromDb = _context.Cars.FirstOrDefault(c => c.CarId == 1);
            Assert.NotNull(carFromDb);

            Assert.AreEqual(Mileage, carFromDb.Mileage);
            Assert.AreEqual(FuelConsumption, carFromDb.FuelConsumption);
            Assert.AreEqual(description, carFromDb.Description);
            Assert.AreEqual(frontFileName, carFromDb.FrontImage);
            Assert.AreEqual(backFileName, carFromDb.BackImage);
            Assert.AreEqual(leftFileName, carFromDb.LeftImage);
            Assert.AreEqual(rightFileName, carFromDb.RightImage);

            var addressFromDb = _context.Addresses.FirstOrDefault(a => a.AddressId == carFromDb.AddressId);

            Assert.NotNull(addressFromDb);
            Assert.AreEqual(city, addressFromDb.CityId);
            Assert.AreEqual(district, addressFromDb.DistrictId);
            Assert.AreEqual(ward, addressFromDb.WardId);
            Assert.AreEqual(street, addressFromDb.HouseNumberStreet);

            var additionalFunctionFromDb = _context.AdditionalFunctions.FirstOrDefault(f => f.FucntionId == carFromDb.FucntionId);
            Assert.NotNull(additionalFunctionFromDb);
            Assert.AreEqual(Bluetooth, additionalFunctionFromDb.Bluetooth);
            Assert.AreEqual(GPS, additionalFunctionFromDb.Gps);
            Assert.AreEqual(Camera, additionalFunctionFromDb.Camera);
            Assert.AreEqual(Sunroof, additionalFunctionFromDb.SunRoof);
            Assert.AreEqual(Childlock, additionalFunctionFromDb.ChildLock);
            Assert.AreEqual(Childseat, additionalFunctionFromDb.ChildSeat);
            Assert.AreEqual(DVD, additionalFunctionFromDb.Dvd);
            Assert.AreEqual(USB, additionalFunctionFromDb.Usb);
        }

        private List<ValidationResult> ValidateModel(Car car)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(car, serviceProvider: null, items: null);
            Validator.TryValidateObject(car, validationContext, validationResults, true);
            return validationResults;
        }


        [Test]
        [TestCase("front.jpg", "back.jpg", "left.jpg", "right.jpg", true, true, true, true, true, true, true, true,
    2, 2, 2, "456 Test St", true, true, -100, 100, 100, 100, "abc", "Mileage can not less than 0")]
        [TestCase("front.jpg", "back.jpg", "left.jpg", "right.jpg", true, true, true, true, true, true, true, true,
    2, 2, 2, "456 Test St", true, true, 100, -100, 100, 100, "abbbc", "FuelConsumption must be greater than 0!")]
        [TestCase("front.jpg", "back.jpg", "left.jpg", "right.jpg", true, true, true, true, true, true, true, true,
    2, 2, 2, "456 Test St", true, true, 100, 100, -100, 100, "asdf", "BasePrice must be greater than 0.")]
        [TestCase("front.jpg", "back.jpg", "left.jpg", "right.jpg", true, true, true, true, true, true, true, true,
    2, 2, 2, "456 Test St", true, true, 100, 100, 100, -100, "efwefa", "Deposit must be a positive number.")]
        [TestCase("front.jpg", "back.jpg", "left.jpg", "right.jpg", true, true, true, true, true, true, true, true,
    2, 2, 2, "456 Test St", true, true, 100, 100, 100, 100, "", "Description is not empty!")]
        public async Task ChangeCarDetailsByOwner_InvalidValues_ReturnsValidationError(
    string frontFileName, string backFileName, string leftFileName, string rightFileName,
    bool Bluetooth, bool GPS, bool Camera, bool Sunroof, bool Childlock, bool Childseat, bool DVD, bool USB,
    int city, int district, int ward, string street, bool TransmissionType, bool FuelType,
    double mileage, double fuelConsumption, decimal basePrice, decimal deposit, string description,
    string expectedErrorMessage)
        {
            //var directoryPath = @"D:\FSOFTASSignment\Project_RentalCar\rental-car\Rental_Car_DemoTests\bin\Debug\net8.0\wwwroot\img\";
            //if (!Directory.Exists(directoryPath))
            //{
            //    Directory.CreateDirectory(directoryPath);
            //}

            // Arrange
            IFormFile CreateMockFormFile(string fileName)
            {
                var mockFile = new Mock<IFormFile>();
                var content = "Fake file content";
                var ms = new MemoryStream();
                var writer = new StreamWriter(ms);
                writer.Write(content);
                writer.Flush();
                ms.Position = 0;

                mockFile.Setup(f => f.FileName).Returns(fileName);
                mockFile.Setup(f => f.Length).Returns(ms.Length);
                mockFile.Setup(f => f.OpenReadStream()).Returns(ms);
                mockFile.Setup(f => f.ContentDisposition).Returns($"inline; filename={fileName}");
                return mockFile.Object;
            }

            var front = CreateMockFormFile(frontFileName);
            var back = CreateMockFormFile(backFileName);
            var left = CreateMockFormFile(leftFileName);
            var right = CreateMockFormFile(rightFileName);

            var updatedCar = new Car
            {
                CarId = 1,
                Mileage = mileage,
                FuelConsumption = fuelConsumption,
                BasePrice = basePrice,
                Deposit = deposit,
                Description = description,
                LicensePlate = "50F-567.89"
            };

            var initialCarState = _context.Cars.FirstOrDefault(c => c.CarId == 1);

            // Act
            var validationResults = ValidateModel(updatedCar);

            // Assert
            Assert.IsTrue(validationResults.Any(vr => vr.ErrorMessage == expectedErrorMessage),
                          $"Expected validation error: {expectedErrorMessage}");

            // Verify that the car's details have not changed
            var carFromDb = _context.Cars.FirstOrDefault(c => c.CarId == 1);
            Assert.AreEqual(initialCarState.Mileage, carFromDb.Mileage);
            Assert.AreEqual(initialCarState.FuelConsumption, carFromDb.FuelConsumption);
            Assert.AreEqual(initialCarState.BasePrice, carFromDb.BasePrice);
            Assert.AreEqual(initialCarState.Deposit, carFromDb.Deposit);
            Assert.AreEqual(initialCarState.Description, carFromDb.Description);
        }
    }

    public class DummySession : ISession
    {
        private readonly Dictionary<string, byte[]> _sessionStorage = new Dictionary<string, byte[]>();

        public bool IsAvailable => true;
        public string Id => Guid.NewGuid().ToString();
        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public void Clear()
        {
            _sessionStorage.Clear();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _sessionStorage.Remove(key);
        }

        public void Set(string key, byte[] value)
        {
            _sessionStorage[key] = value;
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            return _sessionStorage.TryGetValue(key, out value);
        }
    }

}
