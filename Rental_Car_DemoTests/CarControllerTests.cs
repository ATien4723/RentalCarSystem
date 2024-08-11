//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.ConstrainedExecution;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using Newtonsoft.Json;
//using Rental_Car_Demo.Controllers;
//using Rental_Car_Demo.Models;
//using Rental_Car_Demo.Repository.CarRepository;
//using Rental_Car_Demo.Validation;

//namespace Rental_Car_Demo.UnitTests
//{
//    [TestFixture]
//    public class CarControllerTests
//    {
//        private CarController _carController;
//        private RentCarDbContext _dbContext;
//        private DummySession _session;

//        [SetUp]
//        public void SetUp()
//        {
//            var options = new DbContextOptionsBuilder<RentCarDbContext>()
//                .UseInMemoryDatabase(databaseName: "TestDatabase")
//                .Options;

//            _dbContext = new RentCarDbContext(options);
//            _session = new DummySession();

//            _carController = new CarController()
//            {
//                ControllerContext = new ControllerContext
//                {
//                    HttpContext = new DefaultHttpContext
//                    {
//                        Session = _session
//                    }
//                }
//            };

//            SeedDatabase();
//        }

//        [Test]
//        public void ViewMyCarsGet_ReturnsExpectedCars()
//        {
//            var user = _dbContext.Users.Find(1);
//            var settings = new JsonSerializerSettings
//            {
//                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
//            };
//            var userJson = JsonConvert.SerializeObject(user, settings);
//            _session.SetString("User", userJson);

//            // Act
//            var result = _carController.ViewMyCars() as ViewResult;
//            var viewBagCars = _carController.ViewData["Cars"];

//            // Assert
//            Assert.IsNotNull(viewBagCars);
//            Assert.IsNotNull(result);
//            Assert.IsNotNull(result.ViewData);
//            Assert.IsTrue(result.ViewData.Values.Contains(viewBagCars));
//        }

//        [Test]
//        public void ViewMyCarsGet_ReturnsErrorView()
//        {
//            var user = _dbContext.Users.Find(2);
//            var settings = new JsonSerializerSettings
//            {
//                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
//            };
//            var userJson = JsonConvert.SerializeObject(user, settings);
//            _session.SetString("User", userJson);

//            // Act
//            var result = _carController.ViewMyCars() as ViewResult;

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.That(result.ViewName, Is.EqualTo("ErrorAuthorization"));
//        }

//        [TestCase("latest")]
//        [TestCase("newest")]
//        [TestCase("highest")]
//        [TestCase("lowest")]
//        public void ViewMyCarsPost_ReturnsSortOrderCar(string sortOrder)
//        {
//            var user = _dbContext.Users.Find(1);
//            var settings = new JsonSerializerSettings
//            {
//                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
//            };
//            var userJson = JsonConvert.SerializeObject(user, settings);
//            _session.SetString("User", userJson);
//            // Act
//            var result = _carController.ViewMyCars(sortOrder) as ViewResult;
//            var cars = _carController.ViewData["Cars"];
//            var viewBag = _carController.ViewBag.SortOrder.ToString();

//            // Assert
//            Assert.IsNotNull(_carController.ViewBag.Cars);
//            Assert.IsNotNull(_carController.ViewBag.SortOrder);
//            Assert.AreEqual(viewBag, sortOrder);
//            //Assert.IsNull(cars);
//        }


//        [TearDown]
//        public void TearDown()
//        {
//            _dbContext.Database.EnsureDeleted();
//            _carController.Dispose();
//            _dbContext.Dispose();
//        }
//        private void SeedDatabase()
//        {
//            var ratingData = new List<Feedback>
//            {
//                new Feedback { FeedbackId = 1, BookingNo = 1, Ratings = 4, Content = "abc", Date = new DateTime(2023, 06, 22)}
//            };
//            var brandData = new List<CarBrand>
//            {
//                new CarBrand { BrandId = 1, BrandName = "Acura"},
//                new CarBrand { BrandId = 2, BrandName = "Alfa Romeo"},
//                new CarBrand { BrandId = 3, BrandName = "Audi"},
//                new CarBrand { BrandId = 4, BrandName = "BMW"},
//            };

//            var modelData = new List<CarModel>
//            {
//                new CarModel { ModelId = 1, ModelName = "ILX",BrandId = 1},
//                new CarModel { ModelId = 2, ModelName = "MDX",BrandId = 1},
//                new CarModel { ModelId = 3, ModelName = "MDX Sport Hybrid", BrandId = 1},
//                new CarModel { ModelId = 4, ModelName = "NSX",BrandId = 1},
//            };

//            var cityData = new List<City>
//            {
//                new City { CityId = 1, CityProvince = "Thành phố Hà Nội"},
//                new City { CityId = 2, CityProvince = "Tỉnh Hà Giang"},
//                new City { CityId = 3, CityProvince = "Tỉnh Cao Bằng"},
//                new City { CityId = 4, CityProvince = "Tỉnh Bắc Kạn"},
//            };

//            var districtData = new List<District>
//            {
//                new District { CityId = 1, DistrictName = "Quận Ba Đình",DistrictId = 1},
//                new District { CityId = 2, DistrictName = "Huyện Đồng Văn",DistrictId = 2},
//                new District { CityId = 3, DistrictName = "Huyện Bảo Lâm",DistrictId = 3},
//                new District { CityId = 4, DistrictName = "Huyện Bạch Thông",DistrictId = 4},
//            };

//            var wardData = new List<Ward>
//            {
//                new Ward { WardId = 1, WardName = "Phường Phúc Xá",DistrictId = 1},
//                new Ward { WardId = 2, WardName = "Phường Phúc Tân",DistrictId = 2},
//                new Ward { WardId = 3, WardName = "Phường Phú Thượng",DistrictId = 3},
//                new Ward { WardId = 4, WardName = "Phường Thượng Thanh",DistrictId = 4},
//            };


//            var addressData = new List<Address>
//            {
//                new Address { AddressId = 1, CityId = 1,DistrictId = 1,WardId = 1, HouseNumberStreet = "Nha so 1"},
//                new Address { AddressId = 2, CityId = 2,DistrictId = 2,WardId = 2, HouseNumberStreet = "Nha so 2"},
//                new Address { AddressId = 3, CityId = 3,DistrictId = 3,WardId = 3, HouseNumberStreet = "Nha so 3"},
//                new Address { AddressId = 4, CityId = 4,DistrictId = 4,WardId = 4, HouseNumberStreet = "Nha so 4"},

//            };

//            var userData = new List<User>
//            {
//                new User { UserId = 1, Name = "User A",Role = true, Email = "duyquan7b@gmail.com", Password ="quandeptrai123",Phone="0987654321" },
//                new User { UserId = 2, Name = "User B", Role = false,Email = "duyquan7b1@gmail.com", Password ="quandeptrai123", Phone="0987654321" }
//            };

//            var termData = new List<TermOfUse>
//            {
//                new TermOfUse { TermId = 1, NoSmoking = true, NoFoodInCar = true, NoPet = false, Specify = "No Minh Quan" },
//                new TermOfUse { TermId = 2, NoSmoking = true, NoFoodInCar = true, NoPet = false}
//            };

//            var functionData = new List<AdditionalFunction>
//            {
//                new AdditionalFunction { FucntionId = 1, Bluetooth = true, Gps = true, Camera = false,SunRoof = false,ChildLock = false,ChildSeat = true,Dvd = false , Usb = true },
//                new AdditionalFunction { FucntionId = 2, Bluetooth = false, Gps = false, Camera = false,SunRoof = false,ChildLock = false,ChildSeat = true,Dvd = false , Usb = true  }
//            };

//            var colorData = new List<CarColor>
//                {
//                new CarColor { ColorId = 1, ColorName = "White"},
//                new CarColor { ColorId = 2, ColorName = "Red"},
//                new CarColor { ColorId = 3, ColorName = "Blue"},
//                new CarColor { ColorId = 4, ColorName = "Yellow"},
//            };

//            var bookingData = new List<Booking>
//            {
//                new Booking { BookingNo = 1, UserId = 1, CarId = 1, StartDate = new DateTime(2023, 06, 22), EndDate = new DateTime(2023, 06, 24), PaymentMethod = 1, Status = 3},
//                new Booking { BookingNo = 2, UserId = 1, CarId = 2, StartDate = new DateTime(2023, 06, 23), EndDate = new DateTime(2023, 06, 24), PaymentMethod = 1, Status=3}
//            };

//            var carData = new List<Car>
//            {
//                new Car {CarId = 1,UserId = 1, Name = "Acura ILX 2000", LicensePlate = "56F-513.11", BrandId = 1, ModelId = 1, Seats = 4,
//                ColorId = 1,FrontImage = "Image1.jpg",BackImage ="Image2.jpg",LeftImage ="Image1.jpg", RightImage ="Image1.jpg",
//                ProductionYear = 2000,TransmissionType = true, FuelType = true, Mileage = 200, FuelConsumption = 10,
//                BasePrice = 1000000, Deposit = 500000,AddressId = 1, Description = "NOG NOG 1", DocumentId = 1,
//                TermId = 1, FucntionId = 1, Status = 1, NoOfRide = 1
//                },
//                new Car {CarId = 2,UserId = 1, Name = "Acura MDX 2003", LicensePlate = "56F-513.22", BrandId = 1, ModelId = 2, Seats = 10,
//                ColorId = 2,FrontImage = "Image2.jpg",BackImage ="Image2.jpg",LeftImage ="Image2.jpg", RightImage ="Image2.jpg",
//                ProductionYear = 2003,TransmissionType = false, FuelType = false, Mileage = 100, FuelConsumption = 8,
//                BasePrice = 1000000, Deposit = 500000,AddressId = 2, Description = "NOG NOG 2", DocumentId = 1,
//                TermId = 2, FucntionId = 2, Status = 1, NoOfRide = 2
//                },
//            };
//            _dbContext.CarColors.AddRange(colorData);
//            _dbContext.AdditionalFunctions.AddRange(functionData);
//            _dbContext.TermOfUses.AddRange(termData);
//            _dbContext.Users.AddRange(userData);
//            _dbContext.Addresses.AddRange(addressData);
//            _dbContext.Wards.AddRange(wardData);
//            _dbContext.Districts.AddRange(districtData);
//            _dbContext.Cities.AddRange(cityData);
//            _dbContext.CarModels.AddRange(modelData);
//            _dbContext.CarBrands.AddRange(brandData);
//            _dbContext.Feedbacks.AddRange(ratingData);
//            _dbContext.Cars.AddRange(carData);
//            _dbContext.Bookings.AddRange(bookingData);
//            _dbContext.SaveChanges();
//        }

//        public class DummySession : ISession
//        {
//            private readonly Dictionary<string, byte[]> _sessionStore = new Dictionary<string, byte[]>();

//            public DummySession()
//            {
//            }

//            // Phương thức này để lấy giá trị string từ session
//            public string? GetString(string key)
//            {
//                if (_sessionStore.TryGetValue(key, out var value))
//                {
//                    return System.Text.Encoding.UTF8.GetString(value);
//                }
//                return null;
//            }

//            public void Clear() => _sessionStore.Clear();
//            public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
//            public void Dispose() { }
//            public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
//            public void Remove(string key) => _sessionStore.Remove(key);
//            public void Set(string key, byte[] value) => _sessionStore[key] = value;
//            public bool TryGetValue(string key, out byte[] value) => _sessionStore.TryGetValue(key, out value);
//            public IEnumerable<string> Keys => _sessionStore.Keys;

//            public bool IsAvailable => throw new NotImplementedException();

//            public string Id => throw new NotImplementedException();
//        }
//    }
//}
