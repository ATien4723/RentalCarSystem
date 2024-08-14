using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Rental_Car_Demo.Controllers;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rental_Car_Demo.UnitTests
{
    [TestFixture]
    public class BookingControllerTests
    {
        private BookingController _controller;
        private RentCarDbContext _context;
        private DummySession _dummySession;
        private DefaultHttpContext _httpContext;
        private IEmailService _emailService;
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

            _controller = new BookingController(_emailService,_context)
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
        new District { CityId = 2, DistrictName = "Huyện Đồng Văn",DistrictId = 2},
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
                StartDate = new DateTime(2024, 7, 20),
                EndDate = new DateTime(2024, 7, 22),
                PaymentMethod = 2,
                Status = 1,
                },

                new Booking
                {
                BookingNo = 4,
                UserId = 2,
                CarId = 2,
                StartDate = new DateTime(2024, 7, 23),
                EndDate = new DateTime(2024, 7, 24),
                PaymentMethod = 2,
                Status = 5,
                },

                new Booking
                {
                BookingNo = 5,
                UserId = 2,
                CarId = 2,
                StartDate = new DateTime(2024, 7, 26),
                EndDate = new DateTime(2024, 7, 28),
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
            _context.SaveChanges();

            //mock session
            var user = _context.Users.Find(2);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _dummySession.SetString("User", userJson);

        }

        [Test]
        [TestCase(null, null, 1, 1, "newest", "newest")]
        [TestCase(null, null, 1, 1, "latest", "latest")]
        [TestCase(null, null, 1, 1, "highest", "highest")]
        [TestCase(null, null, 1, 1, "lowest", "lowest")]
    
        public void ConfirmPickupForDetailsPage_ValidSortOrder_InBookingList_ReturnViewBookingList(DateTime? startDate, DateTime? endDate, int carId, int bookingNo, string sortOrder, string expectedSortOrder)
        {   
            var result = _controller.confirmPickupForDetailsPage(startDate, endDate, carId, bookingNo, sortOrder) as ViewResult;

            var expectedBooking = _context.Bookings.Find(bookingNo);

            Assert.That(3, Is.EqualTo(expectedBooking.Status));

            Assert.That(expectedSortOrder, Is.EqualTo(result.ViewData["SortOrder"]));

            Assert.That("ViewBookingList", Is.EqualTo(result.ViewName));

        }

        [Test]
        [TestCase(null, null, 1, 1, "Lowest")]  // SortOrder invalid
        [TestCase("2024-07-31", "2024-7-30", 1, 1, "newest")] // DateTime invalid
        [TestCase("2024-07-31", "2024-7-31", 1, 1, "newest")] // DateTime invalid
        [TestCase("2024-7-31", "2024-08-15", 4, 1, "latest")]
        [TestCase("2024-7-31", "2024-08-15", 0, 1, "latest")]
        [TestCase("2024-7-31", "2024-08-15", -100, 1, "latest")] //carId invalid
        [TestCase("2024-7-20", "2024-7-22", 1, 0, "highest")]
        [TestCase("2024-7-2", "2024-7-22", 1, 6, "lowest")]
        [TestCase("2024-7-2", "2024-7-22", 1, -100, "lowest")] //Booking No invalid
        public void ConfirmPickupForDetailsPage_InValidInput_InBookingList_ReturnNotFoundPage(DateTime? startDate, DateTime? endDate, int carId, int bookingNo, string sortOrder)
        {
            var result = _controller.confirmPickupForDetailsPage(startDate, endDate, carId, bookingNo, sortOrder) as NotFoundObjectResult;

            Assert.IsInstanceOf<NotFoundObjectResult>(result);

        }

        [Test]
        [TestCase(null, null, 1, 1, "")]
        public void ConfirmPickupForDetailsPage_CarOwnerLogin_ReturnErrorPage(DateTime? startDate, DateTime? endDate, int carId, int bookingNo, string sortOrder)
        {
            var user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _dummySession.SetString("User", userJson);
            var result = _controller.confirmPickupForDetailsPage(startDate, endDate, carId, bookingNo, sortOrder) as ViewResult;

            Assert.That(result.ViewName,Is.EqualTo("ErrorAuthorization"));

        }

        [Test]
        [TestCase("2024-07-31", "2024-08-01", 1, 1, "newest")]
        [TestCase("2024-7-31", "2024-08-15", 1, 1, "latest")]
        [TestCase("2024-7-20", "2024-7-22", 1, 1, "highest")]
        [TestCase("2024-7-2", "2024-7-22", 1, 1, "lowest")]

        public void ConfirmPickupForDetailsPage_ValidInput_InEditBooking_ReturnViewBookingList(string startDate, string endDate, int carId, int bookingNo, string sortOrder)
        {
            
            var result = _controller.confirmPickupForDetailsPage(DateTime.Parse(startDate), DateTime.Parse(endDate), carId, bookingNo, sortOrder) as RedirectToActionResult;

            var expectedBooking = _context.Bookings.Find(bookingNo);

            Assert.That(3, Is.EqualTo(expectedBooking.Status));

            Assert.That("EditBookingDetail", Is.EqualTo(result.ActionName));

        }

        [Test]
        [TestCase("2024-07-20", "2024-07-22", 2, 4, "Xe dep lam", 4, "Xe dep lam", 4)]
        [TestCase("2024-07-20", "2024-07-22", 2, 4, "Xe te qua", 1, "Xe te qua", 1)]
        [TestCase("2024-07-20", "2024-07-22", 2, 4, "", 3, "", 3)]
        [TestCase("2024-07-20", "2024-07-22", 2, 4, "Good car", 5, "Good car", 5)] 
        [TestCase("2024-07-20", "2024-07-22", 2, 4, "Average car", 2, "Average car", 2)] 
        [TestCase("2024-07-20", "2024-07-22", 2, 4, "@5%^$##&*)(", 2, "@5%^$##&*)(", 2)] 
        [TestCase("2024-07-20", "2024-07-22", 2, 4, "aaaaaaaaaaaaadadddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd", 2, "aaaaaaaaaaaaadadddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd", 2)] 
         
        public void giveRating_ValidInput_ReturnView(DateTime startDate, DateTime endDate, int carId, int bookingNo, string content, int ratings,string expectedContent,int expectedRating)
        {
            var expectedBooking = _context.Bookings.Find(bookingNo);

            Assert.That(5, Is.EqualTo(expectedBooking.Status));

            var resultAction = _controller.giveRating(startDate,endDate,carId,bookingNo,content,ratings) as RedirectToActionResult;

            var resultFeedback = _context.Feedbacks.Last();

            Assert.That(resultFeedback.Ratings, Is.EqualTo(expectedRating));

            Assert.That(resultFeedback.Content, Is.EqualTo(expectedContent));

            Assert.That(resultFeedback.BookingNo, Is.EqualTo(bookingNo));

            Assert.That("EditBookingDetail", Is.EqualTo(resultAction.ActionName));
        }

        [Test]
        [TestCase(null, "2024-07-22", 2, 4, "Xe dep lam", 4)]
        [TestCase("2024-07-24", null, 2, 4, "Xe dep lam", 4)]
        [TestCase(null, null, 2, 4, "Xe dep lam", 4)]
        [TestCase("2024-07-24", "2024-07-22", 2, 4, "Xe dep lam", 4)]
        [TestCase("2024-07-22", "2024-07-22", 2, 4, "Xe te qua", 1)] //DateTime invalid
        [TestCase("2024-07-20", "2024-07-22", 3, 4, "", 3)]
        [TestCase("2024-07-20", "2024-07-22", 0, 4, "Good car", 5)]
        [TestCase("2024-07-20", "2024-07-22", -100, 4, "Average car", 2)] // carId invalid
        [TestCase("2024-07-20", "2024-07-22", 2, 6, "@5%^$##&*)(", 2)]
        [TestCase("2024-07-20", "2024-07-22", 2, 0, "abc", 2)]
        [TestCase("2024-07-20", "2024-07-22", 2, -100, "abc", 2)]// BookingNo invalid
       


        public void giveRating_InvalidInput_ReturnNotFoundPage(DateTime? startDate, DateTime? endDate, int carId, int bookingNo, string content, int ratings)
        {

            var resultAction = _controller.giveRating(startDate, endDate, carId, bookingNo, content, ratings) as NotFoundObjectResult;

            Assert.IsInstanceOf<NotFoundObjectResult>(resultAction);
        }

        [Test]
        [TestCase(null, "2024-07-22", 2, 4)]
        [TestCase("2024-07-24", null, 2, 4)]
        [TestCase(null, null, 2, 4)]
        [TestCase("2024-07-24", "2024-07-22", 2, 4)]
        [TestCase("2024-07-22", "2024-07-22", 2, 4)] //DateTime invalid
        [TestCase("2024-07-20", "2024-07-22", 3, 4)]
        [TestCase("2024-07-20", "2024-07-22", 0, 4)]
        [TestCase("2024-07-20", "2024-07-22", -100, 4)] // carId invalid
        [TestCase("2024-07-20", "2024-07-22", 2, 6)]
        [TestCase("2024-07-20", "2024-07-22", 2, 0)]
        [TestCase("2024-07-20", "2024-07-22", 2, -100)]// BookingNo invalid
        public void skipRating_Invalid_ReturnNotFoundPage(DateTime? startDate, DateTime? endDate, int carId, int bookingNo)
        {

            var result = _controller.skipRating(startDate, endDate,carId,bookingNo) as NotFoundObjectResult;

            Assert.IsInstanceOf<NotFoundObjectResult>(result);

        }

        [Test]
        [TestCase("2024-07-21", "2024-07-22", 2, 4)]
        [TestCase("2024-07-12", "2024-07-22", 2, 4)]
        [TestCase("2024-04-1", "2024-07-29", 2, 4)]
        public void skipRating_Invalid_ReturnView(DateTime? startDate, DateTime? endDate, int carId, int bookingNo)
        {
            var expectedBooking = _context.Bookings.Find(bookingNo);

            Assert.That(5, Is.EqualTo(expectedBooking.Status));

            var result = _controller.skipRating(startDate, endDate, carId, bookingNo) as RedirectToActionResult;

            var resultFeedback = _context.Feedbacks.Last();

            Assert.That(resultFeedback.Ratings, Is.EqualTo(-1));

            Assert.That(resultFeedback.BookingNo, Is.EqualTo(bookingNo));

            Assert.That(result.ActionName, Is.EqualTo("EditBookingDetail"));

            Assert.That(result.RouteValues["carId"], Is.EqualTo(carId));

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
}
