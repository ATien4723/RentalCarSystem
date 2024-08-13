using System.Diagnostics.CodeAnalysis;
using Moq;
using Rental_Car_Demo.Controllers;
using Rental_Car_Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Rental_Car_Demo.Repository.CarRepository;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Rental_Car_Demo.Tests
{
    [TestFixture]
    public class HomeControllerTests
    {
        private Mock<ICarRepository> _mockCarRepository;
        private RentCarDbContext _context;
        private HomeController _controller;
        private DefaultHttpContext _httpContext;
        private DummySession _dummySession;

        [SetUp]
        public void SetUp()
        {
            // Mock ICarRepository
            _mockCarRepository = new Mock<ICarRepository>();

            var options = new DbContextOptionsBuilder<RentCarDbContext>()
                .UseInMemoryDatabase(databaseName: "RentCarTestDb")
                .Options;

            _context = new RentCarDbContext(options);
            SeedDatabase(_context);

            

            _controller = new HomeController(_mockCarRepository.Object, _context);

            _dummySession = new DummySession();
            _httpContext = new DefaultHttpContext { Session = _dummySession };
            _controller.ControllerContext.HttpContext = _httpContext;
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _controller.Dispose();
        }

        private void SeedDatabase(RentCarDbContext _context)
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
                    NoFoodInCar = true
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
                    Camera = true
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

            var users = new List<User>
            {
                new User
                {
                    UserId= 99,
                    Email = "tiendz@gmail.com",
                    Name = "tien pro",
                    Password = "tiendz1",
                    Phone = "099999999",
                    Role = false,
                    Wallet = 0
                },
                new User
                {
                    UserId = 100,
                    Email = "hehe@gmail.com",
                    Password = "tiendz1",
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
                    UserId = 99,
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
                    NoOfRide = 1
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
                    Status = 1,
                    NoOfRide = 2
                }
                ,
                 new Car
                {
                    CarId = 3,
                    UserId = 2,
                    Name = "Car 3",
                    LicensePlate = "50F-567.99",
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
                    AddressId = 3,
                    Description = "Description 2",
                    DocumentId = 2,
                    TermId = 2,
                    FucntionId = 2,
                    Status = 1,
                    NoOfRide = 2
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
            _context.CarDocuments.AddRange(carDocument);
            _context.Cars.AddRange(cars);
            _context.SaveChanges();
        }

  

        [Test]

        [TestCase (null, null, null, null, null, Description = "Default values")]
        [TestCase ("Adress", null, null, null, null, Description = "Only address")]
        [TestCase ("Empty Address", "2024-07-01", "12:00", "2024-07-02", "14:00", Description = "Empty address with specific dates and times")]
        [TestCase ("Some Address", "2023-07-01", "12:00", "2023-07-02", "14:00", Description = "With provided values")]
        [TestCase ("aa", "2024-01-01", "08:30", "2024-01-03", "18:00", Description = "Empty address with provided dates and times")]
        [TestCase ("", null, "08:30", "2024-01-03", "18:00", Description = "Empty address, empty pickup date")]
        [TestCase ("", "2024-01-01", null, "2024-01-03", "18:00", Description = "Empty address, empty pickup time")]
        [TestCase ("Another Address", "2024-01-01", "08:30", null, null, Description = "Valid address, pickup date and time, empty dropoff date and time")]
        [TestCase ("Address", "2024-01-01", "08:30", "2024-01-03", null, Description = "Valid address, valid pickup date and time, valid dropoff date, empty dropoff time")]
        [TestCase ("Address", "2024-01-01", "08:30", null, "18:00", Description = "Valid address, valid pickup date and time, empty dropoff date, valid dropoff time")]

        public void SearchCarForm_Returns_ViewResult_With_Cars(string? address, string? pickupDate, string? pickupTime, string? dropoffDate, string? dropoffTime)
        {
            // Convert strings to DateOnly and TimeOnly
            DateOnly? pickupDateOnly = pickupDate != null ? DateOnly.Parse(pickupDate) : (DateOnly?)null;
            TimeOnly? pickupTimeOnly = pickupTime != null ? TimeOnly.Parse(pickupTime) : (TimeOnly?)null;
            DateOnly? dropoffDateOnly = dropoffDate != null ? DateOnly.Parse(dropoffDate) : (DateOnly?)null;
            TimeOnly? dropoffTimeOnly = dropoffTime != null ? TimeOnly.Parse(dropoffTime) : (TimeOnly?)null;


            // Act
            var result = _controller.SearchCarForm(address, pickupDateOnly, pickupTimeOnly, dropoffDateOnly, dropoffTimeOnly) as ViewResult;


            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as IEnumerable<Car>;
            Assert.IsNotNull(model);

        }


        [Test]
        [TestCase(new string[] { "Brand A" }, new int[] { }, new bool[] { }, new bool[] { }, new string[] { },  null)]
        [TestCase(new string[] { "Brand A" }, new int[] { 5, 6 }, new bool[] { true }, new bool[] { false }, new string[] { }, "nha so 1")]
        [TestCase(new string[] { "Brand A" }, new int[] { 5 }, new bool[] { }, new bool[] { },  new string[] { }, null)]
        [TestCase(new string[] { "Brand A" }, new int[] { 5 }, new bool[] { true }, new bool[] { },  new string[] { }, null)]
        [TestCase(new string[] { "Brand A" }, new int[] { 5 }, new bool[] { true }, new bool[] { false },  new string[] { }, null)]
        [TestCase(new string[] { "Brand A" }, new int[] { 6 }, new bool[] { false }, new bool[] { true },  new string[] { }, null)]
        [TestCase(new string[] { "Brand A" }, new int[] { 4 }, new bool[] { false }, new bool[] { false },  new string[] { }, null)]
        [TestCase(new string[] { "Brand A" }, new int[] { 4 }, new bool[] { true }, new bool[] { true },  new string[] { "0-100000" }, "Some address")]
        [TestCase(new string[] { "Brand B" }, new int[] { 2 }, new bool[] { false }, new bool[] { true },  new string[] { "100000-500000" }, "Some address")]
        [TestCase(new string[] { "Brand A" }, new int[] { 4 }, new bool[] { true }, new bool[] { false },  new string[] { "500000-1000000" }, "Some address")]
        [TestCase(new string[] { "Brand B" }, new int[] { 4 }, new bool[] { false }, new bool[] { true }, new string[] { "1000000-5000000" }, "Some address")]
        [TestCase(new string[] { "Brand A" }, new int[] { 4 }, new bool[] { true }, new bool[] { true },  new string[] { "5000000-10000000" }, "Some address")]
        [TestCase(new string[] { "Brand B" }, new int[] { 2 }, new bool[] { false }, new bool[] { false },  new string[] { "10000000-100000000" }, "Some address")]
        public void SearchCar_ValidParameters_ReturnsPartialViewWithCars(string[] brandNames, int[] seats, bool[] transmissionTypes, bool[] fuelTypes, string[] priceRange, string address)
        {
            // Act
            var result = _controller.SearchCar(brandNames, seats, transmissionTypes, fuelTypes, priceRange, address) as PartialViewResult;
            var model = result.Model as IEnumerable<Car>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("_CarResultsPartial", result.ViewName);
            Assert.IsNotNull(model);
        }



        [Test]
        [TestCase (null)]
        [TestCase ("")]
        [TestCase ("a")]
        [TestCase ("-1")]
        [TestCase ("a very long query that exceeds the maximum allowed length for the search query in the GetSuggestions method")]
        [TestCase ("invalid_query!@#")]
        public void GetSuggestions_InvalidQuery_ReturnsEmptyList(string? query)
        {
            // Act
            var result = _controller.GetSuggestions (query) as ContentResult;

            // Assert
            Assert.AreEqual ("application/json", result.ContentType);

            var suggestions = System.Text.Json.JsonSerializer.Deserialize<List<string>> (result.Content);
            Assert.IsNotNull (suggestions);
            Assert.IsEmpty (suggestions);
        }

        [TestCase("Phường Phúc Xá")]
        [TestCase("Hà Nội")]
        [TestCase("Nha so 1")]
        public void GetSuggestions_ValidQuery_ReturnsSuggestions(string query)
        {
            // Act
            var result = _controller.GetSuggestions (query) as ContentResult;

            // Assert
            Assert.IsNotNull (result);
            Assert.AreEqual ("application/json", result.ContentType);

            var suggestions = System.Text.Json.JsonSerializer.Deserialize<List<string>> (result.Content);
            Assert.IsNotNull (suggestions);
            Assert.IsTrue (suggestions.Any ()); // Verify that there are suggestions
        }

        [Test]
        public void GetUserFeedbacks_ReturnsFeedbackView()
        {
            // Arrange
            var userId = 1;
            var user = new User { UserId = 1 }; // Authorized user
            var userString = JsonConvert.SerializeObject(user);
            _httpContext.Session.SetString("User", userString);


            // Act
            var result = _controller.GetUserFeedbacks(userId) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.IsInstanceOf<List<Feedback>>(result.Model, "The model should be of type List<Feedback>");
        }



        [Test]
        [TestCase(2, 1, false, "ErrorAuthorization")]
        [TestCase(1, 1, true, "ErrorAuthorization")]
        [TestCase(2, 1, true, "ErrorAuthorization")]

        public void GetUserFeedbacks_UserLogin_ReturnsErrorAuthorizationView(int userIdFeedback, int loggedId, bool loggedRole, string viewName)
        {
            //Arrange
            var user = new User { UserId = loggedId, Role = loggedRole };

            var userString = JsonConvert.SerializeObject(user);

            _httpContext.Session.SetString("User", userString);

            // Act
            var result = _controller.GetUserFeedbacks(userIdFeedback) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(viewName, result.ViewName);
        }

        [Test]
        public void GetUserFeedbacks_UserNotLoggedIn_RedirectsToGuest()
        {
            // Arrange
            var userId = 1;
            // No user session set

            // Act
            var result = _controller.GetUserFeedbacks(userId) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.AreEqual("Guest", result.ActionName, "Redirect should go to Guest action");
            Assert.AreEqual("Users", result.ControllerName, "Redirect should go to Users controller");
        }

        [Test]
        public void GetCarOwnerFeedbacks_ReturnsFeedbackView()
        {
            // Arrange
            var userId = 1;
            var user = new User { UserId = 1 }; // Authorized user
            var userString = JsonConvert.SerializeObject(user);
            _httpContext.Session.SetString("User", userString);


            // Act
            var result = _controller.GetCarOwnerFeedbacks(userId) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.IsInstanceOf<List<Feedback>>(result.Model, "The model should be of type List<Feedback>");
        }

        [Test]
        [TestCase(1, 1, false, "ErrorAuthorization")]
        [TestCase(2, 1, false, "ErrorAuthorization")]
        [TestCase(2, 1, true, "ErrorAuthorization")]

        public void GetCarOwnerFeedbacks_UserLogin_ReturnsErrorAuthorizationView(int userIdFeedback, int loggedId, bool loggedRole, string viewName)
        {
            //Arrange
            var user = new User { UserId = loggedId, Role = loggedRole };

            var userString = JsonConvert.SerializeObject(user);

            _httpContext.Session.SetString("User", userString);

            // Act
            var result = _controller.GetCarOwnerFeedbacks(userIdFeedback) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(viewName, result.ViewName);
        }

        [Test]
        public void GetCarOwnerFeedbacks_UserNotLoggedIn_RedirectsToGuest()
        {
            var userId = 1;

            // Act
            var result = _controller.GetUserFeedbacks(userId) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.AreEqual("Users", result.ControllerName, "Redirect should go to Users controller");
            Assert.AreEqual ("Guest", result.ActionName, "Redirect should go to Guest action");
        }
    }

}
