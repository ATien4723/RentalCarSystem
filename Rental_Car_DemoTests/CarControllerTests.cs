using Microsoft.AspNetCore.Mvc;
using Rental_Car_Demo.Controllers;
using Rental_Car_Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using FluentAssertions;
using Rental_Car_Demo.Validation;
using Rental_Car_Demo.Repository.UserRepository;
using Newtonsoft.Json;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;

namespace Rental_Car_Demo.UnitTests
{
    [TestFixture]
    public class CarControllerTests
    {
        private CarController _controller;
        private RentCarDbContext _context;
        private DummySession _dummySession;
        private DefaultHttpContext _httpContext;
        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<RentCarDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            _context = new RentCarDbContext(options);

            _dummySession = new DummySession();

            _httpContext = new DefaultHttpContext
            {
                Session = _dummySession
            };

            _controller = new CarController(_context)
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

            var brandData = new List<CarBrand>
        {
        new CarBrand { BrandId = 1, BrandName = "Acura"},
        new CarBrand { BrandId = 2, BrandName = "Alfa Romeo"},
        new CarBrand { BrandId = 3, BrandName = "Audi"},
        new CarBrand { BrandId = 4, BrandName = "BMW"},
        };

            var modelData = new List<CarModel>
        {
        new CarModel { ModelId = 1, ModelName = "ILX",BrandId = 1},
        new CarModel { ModelId = 2, ModelName = "MDX",BrandId = 1},
        new CarModel { ModelId = 3, ModelName = "MDX Sport Hybrid", BrandId = 1},
        new CarModel { ModelId = 4, ModelName = "NSX",BrandId = 1},
        };

            var cityData = new List<City>
        {
        new City { CityId = 1, CityProvince = "Thành phố Hà Nội"},
        new City { CityId = 2, CityProvince = "Tỉnh Hà Giang"},
        new City { CityId = 3, CityProvince = "Tỉnh Cao Bằng"},
        new City { CityId = 4, CityProvince = "Tỉnh Bắc Kạn"},
        };

            var districtData = new List<District>
        {
        new District { CityId = 1, DistrictName = "Quận Ba Đình",DistrictId = 1},
        new District { CityId = 2, DistrictName = "Huyện Đồng Văn   ",DistrictId = 2},
        new District { CityId = 3, DistrictName = "Huyện Bảo Lâm",DistrictId = 3},
        new District { CityId = 4, DistrictName = "Huyện Bạch Thông",DistrictId = 4},
        };

            var wardData = new List<Ward>
        {
        new Ward { WardId = 1, WardName = "Phường Phúc Xá",DistrictId = 1},
        new Ward { WardId = 2, WardName = "Phường Phúc Tân",DistrictId = 2},
        new Ward { WardId = 3, WardName = "Phường Phú Thượng",DistrictId = 3},
        new Ward { WardId = 4, WardName = "Phường Thượng Thanh",DistrictId = 4},
        };


            var addressData = new List<Address>
        {
        new Address { AddressId = 1, CityId = 1,DistrictId = 1,WardId = 1, HouseNumberStreet = "Nha so 1"},
        new Address { AddressId = 2, CityId = 2,DistrictId = 2,WardId = 2, HouseNumberStreet = "Nha so 2"},
        new Address { AddressId = 3, CityId = 3,DistrictId = 3,WardId = 3, HouseNumberStreet = "Nha so 3"},
        new Address { AddressId = 4, CityId = 4,DistrictId = 4,WardId = 4, HouseNumberStreet = "Nha so 4"},

        };


            var termData = new List<TermOfUse>
        {
        new TermOfUse { TermId = 1, NoSmoking = true, NoFoodInCar = true, NoPet = false, Specify = "No Minh Quan" },
        new TermOfUse { TermId = 2, NoSmoking = true, NoFoodInCar = true, NoPet = false}
        };

            var functionData = new List<AdditionalFunction>
        {
        new AdditionalFunction { FucntionId = 1, Bluetooth = true, Gps = true, Camera = false,SunRoof = false,ChildLock = false,ChildSeat = true,Dvd = false , Usb = true },
        new AdditionalFunction { FucntionId = 2, Bluetooth = false, Gps = false, Camera = false,SunRoof = false,ChildLock = false,ChildSeat = true,Dvd = false , Usb = true  }
        };

            var colorData = new List<CarColor>
        {
        new CarColor { ColorId = 1, ColorName = "White"},
        new CarColor { ColorId = 2, ColorName = "Red"},
        new CarColor { ColorId = 3, ColorName = "Blue"},
        new CarColor { ColorId = 4, ColorName = "Yellow"},
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
                BasePrice = 1000000, Deposit = 500000,AddressId = 2, Description = "NOG NOG 2", DocumentId = 1,
                TermId = 2, FucntionId = 2, Status = 1, NoOfRide = 2
                },
            };

            var bookingData = new List<Booking>
            {
                 new Booking
                {
                BookingNo = 1,
                UserId = 2,
                CarId = 1,
                StartDate = new DateTime(2024, 7, 31),
                EndDate = new DateTime(2024, 8, 1),
                PaymentMethod = 2,
                Status = 3,
                },
                new Booking
                {
                BookingNo = 2,
                UserId = 2,
                CarId = 2,
                StartDate = new DateTime(2024, 8, 3),
                EndDate = new DateTime(2024, 8, 15),
                PaymentMethod = 2,
                Status = 1,
                },
                new Booking
                {
                BookingNo = 3,
                UserId = 2,
                CarId = 2,
                StartDate = new DateTime(2024, 8, 3),
                EndDate = new DateTime(2024, 8, 15),
                PaymentMethod = 2,
                Status = 5,
                },
                new Booking
                {
                BookingNo = 4,
                UserId = 2,
                CarId = 2,
                StartDate = new DateTime(2024, 8, 3),
                EndDate = new DateTime(2024, 8, 15),
                PaymentMethod = 2,
                Status = 5,
                },
            };

            var userData = new List<User>
        {
        new User { UserId = 1, Name = "User A",Role = true, Email = "duyquan7b@gmail.com", Password ="quandeptrai123",Phone="0987654321" },
        new User { UserId = 2, Name = "User B", Role = false,Email = "duyquan7b1@gmail.com", Password ="quandeptrai123", Phone="0987654321" },
        new User { UserId = 3, Name = "User C", Role = false,Email = "minhquan@gmail.com", Password ="quandeptrai123", Phone="0987654321" }
        };

            var feedbackData = new List<Feedback>
            {
                new Feedback {FeedbackId = 1, BookingNo = 3,Content = "Xe dep lam", Ratings = 5,Date = DateTime.Now},
                new Feedback {FeedbackId = 2, BookingNo = 4,Content = "Xe te qua", Ratings = 1,Date = DateTime.Now},
            };

            _context.CarBrands.AddRange(brandData);
            _context.CarModels.AddRange(modelData);
            _context.Cities.AddRange(cityData);
            _context.Districts.AddRange(districtData);
            _context.Wards.AddRange(wardData);
            _context.Addresses.AddRange(addressData);
            _context.Users.AddRange(userData);
            _context.TermOfUses.AddRange(termData);
            _context.AdditionalFunctions.AddRange(functionData);
            _context.CarColors.AddRange(colorData);
            _context.Cars.AddRange(carData);
            _context.Bookings.AddRange(bookingData);
            _context.Feedbacks.AddRange(feedbackData);
            _context.SaveChanges();

        }



        [Test]
        [TestCase(2, "Hanoi", "2024-07-01", "2024-07-02")]
        [TestCase(2,null, "2024-07-01", "2024-07-02")]
        [TestCase(2, "Hanoi", null, "2024-07-02")]
        [TestCase(2, "Hanoi", "2024-07-01", null)]
        [TestCase(2, null, null, "2024-07-02")]
        [TestCase(2, "Hanoi", null, null)]
        [TestCase(2, null, "2024-07-01", null)]
        [TestCase(2, null, null, null)]

        public void ViewCarDetailsByCustomer_UserLogin_FoundedCar_ReturnCar(int CarId, string? location, DateTime? startDate, DateTime? endDate)
        {
            var user = _context.Users.Find(2);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _dummySession.SetString("User", userJson);


            var result = _controller.ViewCarDetailsByCustomer(CarId, location, startDate, endDate) as ViewResult;

            Car expectedCar = _context.Cars.Find(CarId);

            Assert.That(result.ViewData["Rating"], Is.EqualTo(3));

            Car carTest = (Car)result.ViewData["car"];

            carTest.Should().BeEquivalentTo(expectedCar, options => options
            .Excluding(c => c.Address)
            .Excluding(c => c.Brand).Excluding(c => c.Color)
            .Excluding(c => c.Document)
            .Excluding(c => c.Fucntion)
            .Excluding(c => c.Bookings)
            .Excluding(c => c.Model)
            .Excluding(c => c.Term)
            .Excluding(c => c.User)
        );
        }

        [Test]
        [TestCase(1, "Hanoi", "2024-07-01", "2024-07-02", true,0)]
        [TestCase(1, null, "2024-07-01", "2024-07-02", true, 0)]
        [TestCase(1, "Hanoi", null, "2024-07-02", true, 0)]
        [TestCase(1, "Hanoi", "2024-07-01", null, true, 0)]
        [TestCase(1, null, null, "2024-07-02", true, 0)]
        [TestCase(1, "Hanoi", null, null, true, 0)]
        [TestCase(1, null, "2024-07-01", null, true, 0)]
        [TestCase(1, null, null, null, true, 0)]
        [TestCase(2, "VinhPhuc", "2024-07-01", "2024-07-02", false, 3)]
        [TestCase(2, null, "2024-07-01", "2024-07-02", false, 3)]
        [TestCase(2, "VinhPhuc", null, "2024-07-02", false, 3)]
        [TestCase(2, "VinhPhuc", "2024-07-01", null, false, 3)]
        [TestCase(2, null, null, "2024-07-02", false, 3)]
        [TestCase(2, "VinhPhuc", null, null, false, 3)]
        [TestCase(2, null, "2024-07-01", null, false, 3)]
        [TestCase(2, null, null, null, false, 3)]
        public void ViewCarDetailsByCustomer_CusLogin_FoundedCar_CheckRentValue_ReturnCar(int CarId, string? location, DateTime? startDate, DateTime? endDate, bool checkRentExpected,double expectedRating)
        {
            var user = _context.Users.Find(2);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _dummySession.SetString("User", userJson);

            var result = _controller.ViewCarDetailsByCustomer(CarId, location, startDate, endDate) as ViewResult;

            Car carTest = (Car)result.ViewData["car"];

            Assert.AreEqual(expectedRating, result.ViewData["Rating"]);

            Assert.AreEqual(checkRentExpected, result.ViewData["checkRent"]);

        }


        [Test]
        [TestCase(2, "VinhPhuc", "2024-07-01", "2024-07-02")]
        [TestCase(2, null, "2024-07-01", "2024-07-02")]
        [TestCase(2, "VinhPhuc", null, "2024-07-02")]
        [TestCase(2, "VinhPhuc", "2024-07-01", null)]
        [TestCase(2, null, null, "2024-07-02")]
        [TestCase(2, "VinhPhuc", null, null)]
        [TestCase(2, null, "2024-07-01", null)]
        [TestCase(2, null, null, null)]
        public void ViewCarDetailsByCustomer_NoLogin_FoundedCar_ReturnView(int CarId, string? location, DateTime? startDate, DateTime? endDate)
        {

            var result = _controller.ViewCarDetailsByCustomer(CarId, location, startDate, endDate) as ViewResult;

            Car expectedCar = _context.Cars.Find(CarId);

            Car carTest = (Car)result.ViewData["car"];

            Assert.That(result.ViewData["Rating"], Is.EqualTo(3));

            carTest.Should().BeEquivalentTo(expectedCar, options => options
            .Excluding(c => c.Address)
            .Excluding(c => c.Brand).Excluding(c => c.Color)
            .Excluding(c => c.Document)
            .Excluding(c => c.Fucntion)
            .Excluding(c => c.Bookings)
            .Excluding(c => c.Model)
            .Excluding(c => c.Term)
            .Excluding(c => c.User)
        );
        }

        [Test]
        [TestCase(2, "VinhPhuc", "2024-07-01", "2024-07-02", false)]
        [TestCase(2, null, "2024-07-01", "2024-07-02", false)]
        [TestCase(2, "VinhPhuc", null, "2024-07-02", false)]
        [TestCase(2, "VinhPhuc", "2024-07-01", null, false)]
        [TestCase(2, null, null, "2024-07-02", false)]
        [TestCase(2, "VinhPhuc", null, null, false)]
        [TestCase(2, null, "2024-07-01", null, false)]
        [TestCase(2, null, null, null, false)]
        public void ViewCarDetailsByCustomer_NoLogin_FoundedCar_CheckRentValue_ReturnView(int CarId, string? location, DateTime? startDate, DateTime? endDate, bool checkRentExpected)
        {

            var result = _controller.ViewCarDetailsByCustomer(CarId, location, startDate, endDate) as ViewResult;

            Car expectedCar = _context.Cars.Find(CarId);

            Car carTest = (Car)result.ViewData["car"];

            Assert.AreEqual(checkRentExpected, result.ViewData["checkRent"]);
        }

        [Test]
        [TestCase(3, "Hanoi", "2024-07-01", "2024-07-02")]
        [TestCase(2, "Hanoi", "2024-07-03", "2024-07-02")]
        [TestCase(2, "Hanoi", "2024-07-02", "2024-07-02")]
        [TestCase(3, "Hanoi", "2024-07-02", "2024-07-02")]
        [TestCase(3, "Hanoi", "2024-07-03", "2024-07-02")]
        [TestCase(100, "Saigon", "2024-08-01", "2024-08-02")]
        [TestCase(-10, "VinhPhuc", "2024-07-01", "2024-07-02")]
        [TestCase(1000, null, "2024-07-01", "2024-07-02")]
        [TestCase(-100, "VinhPhuc", null, "2024-07-02")]
        [TestCase(0, "VinhPhuc", "2024-07-01", null)]
        [TestCase(1000, null, null, "2024-07-02")]
        [TestCase(1000, "VinhPhuc", null, null)]
        [TestCase(1000, null, "2024-07-01", null)]
        [TestCase(1000, null, null, null)]
        public void ViewCarDetailsByCustomer_NoLogin_NotFoundedCar_ReturnNotFoundPage(int CarId, string? location, DateTime? startDate, DateTime? endDate)
        {
            var result = _controller.ViewCarDetailsByCustomer(CarId, location, startDate, endDate) as NotFoundObjectResult;

            Assert.IsInstanceOf<NotFoundObjectResult>(result);

        }

        [Test]
        [TestCase(3, "Hanoi", "2024-07-01", "2024-07-02")]
        [TestCase(2, "Hanoi", "2024-07-03", "2024-07-02")]
        [TestCase(2, "Hanoi", "2024-07-02", "2024-07-02")]
        [TestCase(3, "Hanoi", "2024-07-02", "2024-07-02")]
        [TestCase(3, "Hanoi", "2024-07-03", "2024-07-02")]
        [TestCase(100, "Saigon", "2024-08-01", "2024-08-02")]
        [TestCase(-10, "VinhPhuc", "2024-07-01", "2024-07-02")]
        [TestCase(1000, null, "2024-07-01", "2024-07-02")]
        [TestCase(-100, "VinhPhuc", null, "2024-07-02")]
        [TestCase(0, "VinhPhuc", "2024-07-01", null)]
        [TestCase(1000, null, null, "2024-07-02")]
        [TestCase(1000, "VinhPhuc", null, null)]
        [TestCase(1000, null, "2024-07-01", null)]
        [TestCase(1000, null, null, null)]
        public void ViewCarDetailsByCustomer_InvalidInput_CusLogin_NotFoundedCar_ReturnNotFoundPage(int CarId, string? location, DateTime? startDate, DateTime? endDate)
        {
            var user = _context.Users.Find(2);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _dummySession.SetString("User", userJson);

            var result = _controller.ViewCarDetailsByCustomer(CarId, location, startDate, endDate) as NotFoundObjectResult;

            Assert.IsInstanceOf<NotFoundObjectResult>(result);

        }

        [Test]
        [TestCase(2, "Hanoi", "2024-07-01", "2024-07-02")]
        public void ViewCarDetailsByCustomer_CarOwnerLogin_ReturnErrorPage(int CarId, string? location, DateTime? startDate, DateTime? endDate)
        {
            var user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);

            _dummySession.SetString("User", userJson);

            var result = _controller.ViewCarDetailsByCustomer(CarId, location, startDate, endDate) as ViewResult;

            Assert.That( result.ViewName, Is.EqualTo("ErrorAuthorization"));


        }


        [Test]
        [Pairwise]
        public async Task AddACarAsync_RedirectsToExpectedAction(
        [Values("29A-123.45", "29A-1245")] string licensePlate,
        [Values(2)] int brandId,
        [Values(2)] int modelId,
        [Values(5)] int seats,
        [Values(2)] int colorId,
        [Values(2015)] int productionYear,
        [Values(true, false)] bool transmissionType,
        [Values(true, false)] bool fuelType,
        [Values(1,15000)] int mileage,
        [Values(1,8)] int fuelConsumption,
        [Values(1,500000)] int basePrice,
        [Values(1,200000)] int deposit,
        [Values("NOG NOG 2")] string description,
        [Values("registration.doc", "registration.docx", "registration.jpg", "registration.jpeg", "registration.png", "registration.pdf")] string registrationFileName,
        [Values("certificate.doc", "certificate.docx", "certificate.jpg", "certificate.jpeg", "certificate.png", "certificate.pdf")] string certificateFileName,
        [Values("insurance.doc", "insurance.docx", "insurance.jpg", "insurance.jpeg", "insurance.png", "insurance.pdf", null)] string? insuranceFileName,
        [Values("front.jpg", "front.png", "front.jpeg", "front.gif")] string frontFileName,
        [Values("back.jpg", "back.png", "back.jpeg", "back.gif")] string backFileName,
        [Values("left.jpg", "left.png", "left.jpeg", "left.gif")] string leftFileName,
        [Values("right.jpg", "right.png", "right.jpeg", "right.gif")] string rightFileName,
        [Values(true, false)] bool bluetooth,
        [Values(true, false)] bool gps,
        [Values(true, false)] bool camera,
        [Values(true, false)] bool sunroof,
        [Values(true, false)] bool childlock,
        [Values(true, false)] bool childseat,
        [Values(true, false)] bool dvd,
        [Values(true, false)] bool usb,
        [Values(true, false)] bool smoking,
        [Values(true, false)] bool food,
        [Values(true, false)] bool pet,
        [Values("Test specify", null)] string? specify,
        [Values(2)] int city,
        [Values(2)] int district,
        [Values(2)] int ward,
        [Values("456 Main St")] string street
    )
        {
            var user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);

            _dummySession.SetString("User", userJson);

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

            var registration = CreateMockFormFile(registrationFileName);
            var certificate = CreateMockFormFile(certificateFileName);
            IFormFile insurance = null;
            if (insuranceFileName != null)
            {
                insurance = CreateMockFormFile(insuranceFileName);

            }
            var front = CreateMockFormFile(frontFileName);
            var back = CreateMockFormFile(backFileName);
            var left = CreateMockFormFile(leftFileName);
            var right = CreateMockFormFile(rightFileName);

            var car = new Car
            {
                LicensePlate = licensePlate,
                BrandId = brandId,
                ModelId = modelId,
                Seats = seats,
                ColorId = colorId,
                ProductionYear = productionYear,
                TransmissionType = transmissionType,
                FuelType = fuelType,
                Mileage = mileage,
                FuelConsumption = fuelConsumption,
                BasePrice = basePrice,
                Deposit = deposit,
                Description = description,
            };

            var result = await _controller.AddACarAsync(car, registration, certificate, insurance, front, back, left, right,
                                                        bluetooth, gps, camera, sunroof, childlock, childseat, dvd, usb,
                                                        smoking, food, pet, specify, city, district, ward, street)
                                                        as RedirectToActionResult;

            var expectedCar = _context.Cars.Last();

            var expectedAddress = _context.Addresses.Find(expectedCar.AddressId);

            Assert.AreEqual(city, expectedAddress.CityId);
            Assert.AreEqual(district, expectedAddress.DistrictId);
            Assert.AreEqual(ward, expectedAddress.WardId);
            Assert.AreEqual(street, expectedAddress.HouseNumberStreet);
            Assert.AreEqual("Users", result.ControllerName);
            Assert.AreEqual(brandId, expectedCar.BrandId);
            Assert.AreEqual(modelId, expectedCar.ModelId);
            Assert.AreEqual(basePrice, expectedCar.BasePrice);
            Assert.AreEqual(deposit, expectedCar.Deposit);
        }

    }
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
