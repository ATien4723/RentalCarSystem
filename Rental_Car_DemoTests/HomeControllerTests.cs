using Moq;
using Rental_Car_Demo.Controllers;
using Rental_Car_Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Rental_Car_Demo.Repository.CarRepository;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Castle.Components.DictionaryAdapter.Xml;

namespace Rental_Car_Demo.Tests
{
    [TestFixture]
    public class HomeControllerTests
    {
        private Mock<ICarRepository> _mockCarRepository;
        private Mock<RentCarDbContext> _mockContext;
        private HomeController _controller;
        private DefaultHttpContext _httpContext;
        private DummySession _dummySession;

        [SetUp]
        public void SetUp()
        {
            // Mock ICarRepository
            _mockCarRepository = new Mock<ICarRepository>();
            _mockContext = new Mock<RentCarDbContext>();
            // Mock data for testing


            var cityData = new List<City>
            {
                new City { CityId = 1, CityProvince = "Thành phố Hà Nội"},
                new City { CityId = 2, CityProvince = "Tỉnh Hà Giang"},
                new City { CityId = 3, CityProvince = "Tỉnh Cao Bằng"},
                new City { CityId = 4, CityProvince = "Tỉnh Bắc Kạn"},
            }.AsQueryable();

            var districtData = new List<District>
            {
                new District { CityId = 1, DistrictName = "Quận Ba Đình",DistrictId = 1},
                new District { CityId = 2, DistrictName = "Huyện Đồng Văn   ",DistrictId = 2},
                new District { CityId = 3, DistrictName = "Huyện Bảo Lâm",DistrictId = 3},
                new District { CityId = 4, DistrictName = "Huyện Bạch Thông",DistrictId = 4},
            }.AsQueryable();

            var wardData = new List<Ward>
            {
                new Ward { WardId = 1, WardName = "Phường Phúc Xá",DistrictId = 1},
                new Ward { WardId = 2, WardName = "Phường Phúc Tân",DistrictId = 2},
                new Ward { WardId = 3, WardName = "Phường Phú Thượng",DistrictId = 3},
                new Ward { WardId = 4, WardName = "Phường Thượng Thanh",DistrictId = 4},
            }.AsQueryable();


            var addressData = new List<Address>
            {
                new Address { AddressId = 1, CityId = 1,DistrictId = 1,WardId = 1, HouseNumberStreet = "Nha so 1"},
                new Address { AddressId = 2, CityId = 2,DistrictId = 2,WardId = 2, HouseNumberStreet = "Nha so 2"},
                new Address { AddressId = 3, CityId = 3,DistrictId = 3,WardId = 3, HouseNumberStreet = "Nha so 3"},
                new Address { AddressId = 4, CityId = 4,DistrictId = 4,WardId = 4, HouseNumberStreet = "Nha so 4"},
            }.AsQueryable();

            var carModels = new List<CarModel>
            {
                new CarModel { ModelId = 1, ModelName = "Model X" },
                new CarModel { ModelId = 2, ModelName = "Model Y" }
            }.AsQueryable();

            var carBrands = new List<CarBrand>
            {
                new CarBrand { BrandId = 1, BrandName = "Brand A" },
                new CarBrand { BrandId = 2, BrandName = "Brand B" }
            }.AsQueryable();

            var cars = new List<Car>
            {
                new Car { CarId = 1, ModelId = 1, BrandId = 1, Model = carModels.First(cm => cm.ModelId == 1), Brand = carBrands.First(b => b.BrandId == 1), AddressId = 1},
                new Car { CarId = 2, ModelId = 2, BrandId = 2, Model = carModels.First(cm => cm.ModelId == 2), Brand = carBrands.First(b => b.BrandId == 2), AddressId = 2}
            }.AsQueryable();


            // Setup the mock to return the data
            //_mockCarRepository.Setup(repo => repo.GetAllCars()).Returns(cars);


            // Setup the mock repository methods
            _mockCarRepository.Setup(repo => repo.GetAllCars(It.IsAny<string>())).Returns((string address) =>
            {
                if (string.IsNullOrEmpty(address))
                {
                    return cars;
                }
                else
                {
                    return cars.Where(c => c.CarId == 1);
                }
            });

            _mockCarRepository.Setup(repo => repo.SearchCars(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<string>()))
            .Returns(cars);



            // Initialize DummySession
            _dummySession = new DummySession();

            // Initialize HttpContext and assign DummySession
            _httpContext = new DefaultHttpContext();
            _httpContext.Session = _dummySession;


            // Setup the HomeController with mocks
            _controller = new HomeController(_mockCarRepository.Object, _mockContext.Object);

            // Inject mocked HttpContext into ControllerContext
            _controller.ControllerContext.HttpContext = _httpContext;

        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }


        [Test]

        [TestCase(null, null, null, null, null, Description = "Default values")]
        [TestCase("Some Address", "2023-07-01", "12:00", "2023-07-02", "14:00", Description = "With provided values")]
        [TestCase("", "2024-01-01", "08:30", "2024-01-03", "18:00", Description = "Empty address with provided dates and times")]
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


        [TestCase(true, "ErrorAuthorization")]
        [TestCase(false, "_CarResultsPartial")]
        public void SearchCar_UserRoleBasedView(bool role, string expectedViewName)
        {
            // Arrange
            var user = new User { Role = role };
            var userString = JsonConvert.SerializeObject(user);

            _dummySession.Set("User", Encoding.UTF8.GetBytes(userString));
            _httpContext.Session = _dummySession;

            // Inject the mocked HttpContext into the controller's ControllerContext
            //_controller.ControllerContext.HttpContext = _httpContext;

            // Act
            var result = _controller.SearchCar("Brand A", null, null, null, null, null, null);

            // Assert
            if (role)
            {
                var viewResult = result as ViewResult;
                Assert.IsNotNull(viewResult);
                Assert.AreEqual(expectedViewName, viewResult.ViewName);
            }
            else
            {
                var partialViewResult = result as PartialViewResult;
                Assert.IsNotNull(partialViewResult);
                Assert.AreEqual(expectedViewName, partialViewResult.ViewName);
                Assert.IsInstanceOf<IEnumerable<Car>>(partialViewResult.Model);
            }
        }


        [Test]
        public void GetUserFeedbacks_ReturnsFeedbackView()
        {
            // Arrange
            var userId = 1;
            var user = new User { UserId = 1}; // Authorized user
            var userString = JsonConvert.SerializeObject(user);
            _httpContext.Session.SetString("User", userString);

            var feedbacks = new List<Feedback>
                    {
                        new Feedback { FeedbackId = 1, BookingNoNavigation = new Booking { Car = new Car() }, BookingNo = 123}
                    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Feedback>>();

            mockDbSet.As<IQueryable<Feedback>>().Setup(m => m.Provider).Returns(feedbacks.Provider);
            mockDbSet.As<IQueryable<Feedback>>().Setup(m => m.Expression).Returns(feedbacks.Expression);
            mockDbSet.As<IQueryable<Feedback>>().Setup(m => m.ElementType).Returns(feedbacks.ElementType);
            mockDbSet.As<IQueryable<Feedback>>().Setup(m => m.GetEnumerator()).Returns(feedbacks.GetEnumerator());

            _mockContext.Setup(c => c.Feedbacks).Returns(mockDbSet.Object);

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
