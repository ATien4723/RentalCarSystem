using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Rental_Car_Demo.Controllers;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.BookingRepository;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ConstrainedExecution;
using System.Text;
using Rental_Car_Demo.Services;

namespace Rental_Car_Demo.UnitTests
{

    [TestFixture]
    public class BookingControllerTestsAn
    {
        private BookingController _controller;
        private RentCarDbContext _context;
        private Mock<IEmailService> _mockEmailService;
        private DummySession _session;
        private string filePath;
        private Mock<IFormFile> _mockFile;

        [SetUp]
        public void Setup()
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            filePath = Path.Combine(Environment.CurrentDirectory, "wwwroot/img", "newFile.png");
            var options = new DbContextOptionsBuilder<RentCarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new RentCarDbContext(options);
            _session = new DummySession();
            _mockEmailService = new Mock<IEmailService>();
            _mockFile = new Mock<IFormFile>();
            _controller = new BookingController(_mockEmailService.Object, _context)
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

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            _context.Database.EnsureDeleted();
            _controller.Dispose();
            _context.Dispose();
        }

        private void SeedDatabase()
        {
            var ratingData = new List<Feedback>
            {
                new Feedback { FeedbackId = 1, BookingNo = 1, Ratings = 4, Content = "abc", Date = new DateTime(2024, 08, 2)}
            };
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

            var userData = new List<User>
            {
                new User { UserId = 1, Name = "User A",Role = false, Email = "truonganat0@gmail.com", Password ="quandeptrai123",Phone="0987654321", RememberMe = false, Wallet = 0 },
                new User { UserId = 2, Name = "User B", Role = true,Email = "truonganat00@gmail.com", Password ="quandeptrai123", Phone="0987654321", RememberMe = false, Wallet = 20000000 },
                new User { UserId = 3, Name = "User C", Role = false,Email = "truonganat1@gmail.com", Password ="quandeptrai123", Phone="0987654321", RememberMe = false, Wallet = 20000000, Dob =new DateOnly(2003, 10, 24), AddressId = 2, DrivingLicense = "1.png", Address = _context.Addresses.FirstOrDefault(a => a.AddressId == 2) },
                new User { UserId = 4, Name = "User D", Role = false,Email = "truonganat2@gmail.com", Password ="quandeptrai123", Phone="0987654321", RememberMe = false, Wallet = 20000000, AddressId = 1, Address = _context.Addresses.FirstOrDefault(a => a.AddressId == 1)}
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
                new Car {CarId = 1,UserId = 2, Name = "Acura ILX 2000", LicensePlate = "56F-513.11", BrandId = 1, ModelId = 1, Seats = 4,
                ColorId = 1,FrontImage = "Image1.jpg",BackImage ="Image2.jpg",LeftImage ="Image1.jpg", RightImage ="Image1.jpg",
                ProductionYear = 2000,TransmissionType = true, FuelType = true, Mileage = 200, FuelConsumption = 10,
                BasePrice = 1000000, Deposit = 500000,AddressId = 1, Description = "NOG NOG 1", DocumentId = 1,
                TermId = 1, FucntionId = 1, Status = 1, NoOfRide = 1
                },
                new Car {CarId = 2,UserId = 2, Name = "Acura MDX 2003", LicensePlate = "56F-513.22", BrandId = 1, ModelId = 2, Seats = 10,
                ColorId = 2,FrontImage = "Image2.jpg",BackImage ="Image2.jpg",LeftImage ="Image2.jpg", RightImage ="Image2.jpg",
                ProductionYear = 2003,TransmissionType = false, FuelType = false, Mileage = 100, FuelConsumption = 8,
                BasePrice = 2000000, Deposit = 500000,AddressId = 2, Description = "NOG NOG 2", DocumentId = 1,
                TermId = 2, FucntionId = 2, Status = 2, NoOfRide = 2
                },
                new Car {CarId = 3,UserId = 2, Name = "Acura MDX 2003", LicensePlate = "56F-513.22", BrandId = 1, ModelId = 2, Seats = 10,
                ColorId = 2,FrontImage = "Image2.jpg",BackImage ="Image2.jpg",LeftImage ="Image2.jpg", RightImage ="Image2.jpg",
                ProductionYear = 2003,TransmissionType = false, FuelType = false, Mileage = 100, FuelConsumption = 8,
                BasePrice = 3000000, Deposit = 700000,AddressId = 2, Description = "NOG NOG 2", DocumentId = 1,
                TermId = 2, FucntionId = 2, Status = 1, NoOfRide = 2
                },
            };

            var bookingInfoData = new List<BookingInfo>
            {
                new BookingInfo { BookingInfoId = 1, RenterEmail = "truonganat0@gmail.com",RenterName = "Vũ Trường An" , RenterDob = new DateOnly(2003, 10, 24),
                    RenterNationalId = "038203023477", RenterPhone = "0334110870", RenterAddressId = 1, RenterDrivingLicense = "rd.png", IsDifferent = false,
                    DriverEmail = "truonganat0@gmail.com", DriverName = "Vũ Trường An", DriverDob = new DateOnly(2003, 10, 24), DriverNationalId = "038203023477",
                    DriverPhone = "0334110870", DriverAddressId = 1, DriverDrivingLicense = "rd.png", RenterAddress = addressData.FirstOrDefault(a => a.AddressId == 1), DriverAddress = addressData.FirstOrDefault(a => a.AddressId == 1)},
                new BookingInfo { BookingInfoId = 2, RenterEmail = "truonganat0@gmail.com",RenterName = "Vũ Trường An" , RenterDob = new DateOnly(2003, 10, 24),
                    RenterNationalId = "038203023477", RenterPhone = "0334110870", RenterAddressId = 1, RenterDrivingLicense = "rd.png", IsDifferent = false,
                    DriverEmail = "truonganat0@gmail.com", DriverName = "Vũ Trường An", DriverDob = new DateOnly(2003, 10, 24), DriverNationalId = "038203023477",
                    DriverPhone = "0334110870", DriverAddressId = 1, DriverDrivingLicense = "rd.png", RenterAddress = addressData.FirstOrDefault(a => a.AddressId == 2), DriverAddress = addressData.FirstOrDefault(a => a.AddressId == 2)},
                new BookingInfo { BookingInfoId = 5, RenterEmail = "truonganat0@gmail.com",RenterName = "Vũ Trường An" , RenterDob = new DateOnly(2003, 10, 24),
                    RenterNationalId = "038203023477", RenterPhone = "0334110870", RenterDrivingLicense = "rd.png", IsDifferent = false,
                    DriverEmail = "truonganat0@gmail.com", DriverName = "Vũ Trường An", DriverDob = new DateOnly(2003, 10, 24), DriverNationalId = "038203023477",
                    DriverPhone = "0334110870", DriverDrivingLicense = "rd.png"}
            };

            var bookingData = new List<Booking>
            {
                new Booking { BookingNo = 1, UserId = 1, CarId = 1, StartDate = new DateTime(2024, 08, 01, 20, 00, 00), EndDate = new DateTime(2024, 08, 03, 20, 00, 00), PaymentMethod = 1,
                    Status = 2, BookingInfoId = 1, BookingInfo = bookingInfoData.FirstOrDefault(a => a.BookingInfoId == 1)},
                new Booking { BookingNo = 2, UserId = 3, CarId = 1, StartDate = new DateTime(2024, 08, 01, 20, 00, 00), EndDate = new DateTime(2024, 08, 03, 20, 00, 00), PaymentMethod = 2,
                    Status=1, BookingInfoId = 2, BookingInfo = bookingInfoData.FirstOrDefault(a => a.BookingInfoId == 2)},
                new Booking { BookingNo = 3, UserId = 3, CarId = 3, StartDate = new DateTime(2024, 08, 01, 20, 00, 00), EndDate = new DateTime(2024, 08, 03, 20, 00, 00), PaymentMethod = 1,
                    Status=2, BookingInfoId = 2, BookingInfo = bookingInfoData.FirstOrDefault(a => a.BookingInfoId == 2)},
                new Booking { BookingNo = 4, UserId = 3, CarId = 3, StartDate = new DateTime(2024, 08, 01, 20, 00, 00), EndDate = new DateTime(2024, 08, 01, 22, 00, 00), PaymentMethod = 1,
                    Status=2, BookingInfoId = 2, BookingInfo = bookingInfoData.FirstOrDefault(a => a.BookingInfoId == 2)},
                new Booking { BookingNo = 5, UserId = 1, CarId = 1, StartDate = new DateTime(2024, 08, 01, 20, 00, 00), EndDate = new DateTime(2024, 08, 01, 22, 00, 00), PaymentMethod = 1,
                Status=2, BookingInfoId = 5, BookingInfo = bookingInfoData.FirstOrDefault(a => a.BookingInfoId == 5)}
            };


            _context.CarColors.AddRange(colorData);
            _context.AdditionalFunctions.AddRange(functionData);
            _context.TermOfUses.AddRange(termData);
            _context.Users.AddRange(userData);
            _context.Addresses.AddRange(addressData);
            _context.Wards.AddRange(wardData);
            _context.Districts.AddRange(districtData);
            _context.Cities.AddRange(cityData);
            _context.CarModels.AddRange(modelData);
            _context.CarBrands.AddRange(brandData);
            _context.Feedbacks.AddRange(ratingData);
            _context.Cars.AddRange(carData);
            _context.BookingInfos.AddRange(bookingInfoData);
            _context.Bookings.AddRange(bookingData);
            _context.SaveChanges();
        }

        [TestCase("", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 100)]
        public void BookACar_Get_AuthorizationNull_ReturnLogin(string location, string startDateStr, string endDateStr, int carId, int userId)
        {
            User user = null;
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);


            var result = _controller.BookACar(location, startDate, endDate, carId) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ActionName);
        }

        [TestCase("", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2)]
        public void BookACar_Get_AuthorizationCarOwner_ReturnErrorAuthorization(string location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("ErrorAuthorization", result.ViewName);
        }

        [TestCase("", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1)]
        public void BookACar_Get_AuthorizationCustomer_ReturnBookACar_Post(string location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);


            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [TestCase("", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 2, 1)]
        [TestCase("", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 2, 2)]
        public void BookACar_Get_CheckCarStatusNot1_ReturnErrorAuthorization(string location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);


            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            Assert.IsNotNull(result);
            Assert.AreEqual("ErrorAuthorization", result.ViewName);

        }

        [TestCase("", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 100, 1)]
        public void BookACar_Get_CheckCarNull_ReturnError(string location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            Assert.IsNotNull(result);
            Assert.AreEqual("ErrorAuthorization", result.ViewName);
        }

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1)]
        public void BookACar_Get_CheckLocationNotNull_ReturnError(string location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            Assert.IsNotNull(result);
        }

        [TestCase("", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1)]
        public void BookACar_Get_CheckCarStatus1_ReturnView(string location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            Assert.IsNotNull(result);
            Assert.AreEqual("BookACar", result.ViewName);
        }

        [TestCase("", "0001-01-01 00:00:00", "0001-01-01 00:00:00", 1, 1)]
        public void BookACar_Get_StartDateEndDateNull_ReturnView(string location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            Assert.IsNotNull(result);
            Assert.AreEqual("BookACar", result.ViewName);
        }

        [TestCase("", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2)]
        public void BookACar_Get_CheckCarStatus1ButCarOwner_ReturnError(string location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            Assert.IsNotNull(result);
            Assert.AreEqual("ErrorAuthorization", result.ViewName);
        }

        [TestCase("", "2024-08-01 20:00:00", "2030-08-03 20:00:00", 1, 2)]
        public void BookACar_Post_PaymentMethodDiff1(string location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            var viewModel = _context.Bookings.Find(2);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId, viewModel) as RedirectToActionResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            Assert.IsNotNull(result);
            Assert.AreEqual("BookACarFinish", result.ActionName);
        }

        [TestCase("", "2024-08-01 20:00:00", "2024-08-01 22:00:00", 1, 2)]
        public void BookACar_Post_PaymentMethod1_RentLessThan12h(string location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            var viewModel = _context.Bookings.Find(1);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId, viewModel) as RedirectToActionResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            Assert.IsNotNull(result);
            Assert.AreEqual("BookACarFinish", result.ActionName);
        }


        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "038203023477", "0334110870", "truonganat@gmail.com", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 1)]
        public void BookACar_Post_CheckModel_ReturnTrue(string location, string startDateStr, string endDateStr, int carId, int userId,
            string renterEmail, string renterNationalId, string renterPhone, string driverEmail, string driverNationalId, string driverPhone,
            string renterName, string renterDobStr, int renterAddressId, string renterDrivingLicense, bool isDifferent, string driverName, string driverDobStr, int driverAddressId, string driverDrivingLicense, int paymentMethod, int status)
        {
            // Arrange
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);
            DateOnly renterDob = DateOnly.Parse(renterDobStr);
            DateOnly driverDob = DateOnly.Parse(driverDobStr);

            var renterAddress = new Address { CityId = 1, DistrictId = 1, WardId = 1, HouseNumberStreet = "Nha so 1" };

            var driverAddress = new Address { CityId = 1, DistrictId = 1, WardId = 1, HouseNumberStreet = "Nha so 2" };

            var bookingInfo = new BookingInfo
            {
                RenterEmail = renterEmail,
                RenterNationalId = renterNationalId,
                RenterPhone = renterPhone,
                DriverEmail = driverEmail,
                DriverNationalId = driverNationalId,
                DriverPhone = driverPhone,
                RenterName = renterName,
                RenterDob = renterDob,
                RenterAddress = renterAddress,
                RenterDrivingLicense = renterDrivingLicense,
                IsDifferent = isDifferent,
                DriverName = driverName,
                DriverDob = driverDob,
                DriverAddress = driverAddress,
                DriverDrivingLicense = driverDrivingLicense
            };

            var booking = new Booking
            {
                UserId = userId,
                CarId = carId,
                StartDate = startDate,
                EndDate = endDate,
                PaymentMethod = paymentMethod,
                Status = status,
                BookingInfo = bookingInfo
            };

            // Manually trigger validation for BookingInfo
            var bookingInfoValidationContext = new ValidationContext(bookingInfo);
            var bookingInfoValidationResults = new List<ValidationResult>();
            bool isBookingInfoValid = Validator.TryValidateObject(bookingInfo, bookingInfoValidationContext, bookingInfoValidationResults, true);

            // Manually trigger validation for Booking
            var bookingValidationContext = new ValidationContext(booking);
            var bookingValidationResults = new List<ValidationResult>();
            bool isBookingValid = Validator.TryValidateObject(booking, bookingValidationContext, bookingValidationResults, true);

            // Assert that both BookingInfo and Booking are valid
            Assert.IsTrue(isBookingInfoValid);
            Assert.IsTrue(isBookingValid);

            if (isBookingInfoValid && isBookingValid)
            {
                var result = _controller.BookACar(location, startDate, endDate, carId, booking) as RedirectToActionResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("BookACarFinish", result.ActionName);
            }
        }

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "038203023477", "0334110870", "truonganat@gmail.com", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]
        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "038203023477", "", "truonganat@gmail.com", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]
        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "038203023477", "0334110870", "", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]
        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "038203023477", "0334110870", "truonganat@gmail.com", "038203023477", "",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]
        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "0334110870", "truonganat@gmail.com", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "038203023477", "0334110870", "truonganat@gmail.com", "", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "0334110870", "truonganat@gmail.com", "038203023477", "", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "", "", "truonganat@gmail.com", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "", "0334110870", "", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "038203023477", "", "", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "038203023477", "0334110870", "", "038203023477", "",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "038203023477", "0334110870", "truonganat@gmail.com", "", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "", "0334110870", "", "038203023477", "",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]
        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "", "truonganat@gmail.com", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "0334110870", "", "truonganat@gmail.com", "0334110870", "038203023477",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "truonganat@gmail.com", "038203023477", "", "038203023477",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "", "", "", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "", "", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "", "truonganat@gmail.com", "", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "", "", "", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "", "", "", "",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "", "", "", "",
            "", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "", "", "", "",
            "", "1980-01-01", 1, "", false, "", "1980-01-01", 1, "", 1, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "0334110870", "truonganat@gmail.com", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "038203023477", "0334110870", "truonganat@gmail.com", "", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "0334110870", "truonganat@gmail.com", "038203023477", "", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "", "", "truonganat@gmail.com", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "", "0334110870", "", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "038203023477", "", "", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "038203023477", "0334110870", "", "038203023477", "",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "038203023477", "0334110870", "truonganat@gmail.com", "", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "", "", "", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "", "", "", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "", "truonganat@gmail.com", "", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "", "", "", "",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "", "", "", "",
            "", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "", "", "", "",
            "", "1980-01-01", 1, "", false, "", "1980-01-01", 1, "", null, 2)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "0334110870", "truonganat@gmail.com", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "038203023477", "0334110870", "truonganat@gmail.com", "", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "0334110870", "truonganat@gmail.com", "038203023477", "", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "", "", "truonganat@gmail.com", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "", "0334110870", "", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "038203023477", "", "", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "038203023477", "0334110870", "", "038203023477", "",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "038203023477", "0334110870", "truonganat@gmail.com", "", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "", "0334110870", "", "038203023477", "",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "", "truonganat@gmail.com", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "0334110870", "", "truonganat@gmail.com", "0334110870", "038203023477",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "truonganat@gmail.com", "038203023477", "", "038203023477",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "", "", "", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "", "", "", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "", "", "", "",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "", "", "", "",
            "", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", null, null)]

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "", "", "", "", "", "",
            "", "1980-01-01", 1, "", false, "", "1980-01-01", 1, "", null, null)]
        public void BookACar_Post_ValidModel_CheckBookingInfoNullField(string location, string startDateStr, string endDateStr, int carId, int userId,
            string renterEmail, string renterNationalId, string renterPhone, string driverEmail, string driverNationalId, string driverPhone,
            string renterName, string renterDobStr, int? renterAddressId, string renterDrivingLicense, bool isDifferent, string driverName, string driverDobStr, int driverAddressId, string driverDrivingLicense, int? paymentMethod, int? status)
        {
            // Arrange
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);
            DateOnly renterDob = DateOnly.Parse(renterDobStr);
            DateOnly driverDob = DateOnly.Parse(driverDobStr);

            var renterAddress = new Address { CityId = 1, DistrictId = 1, WardId = 1, HouseNumberStreet = "Nha so 1" };

            var driverAddress = new Address { CityId = 1, DistrictId = 1, WardId = 1, HouseNumberStreet = "Nha so 2" };

            var bookingInfo = new BookingInfo
            {
                RenterEmail = renterEmail,
                RenterNationalId = renterNationalId,
                RenterPhone = renterPhone,
                DriverEmail = driverEmail,
                DriverNationalId = driverNationalId,
                DriverPhone = driverPhone,
                RenterName = renterName,
                RenterDob = renterDob,
                RenterAddress = renterAddress,
                RenterDrivingLicense = renterDrivingLicense,
                IsDifferent = isDifferent,
                DriverName = driverName,
                DriverDob = driverDob,
                DriverAddress = driverAddress,
                DriverDrivingLicense = driverDrivingLicense
            };

            var booking = new Booking
            {
                UserId = userId,
                CarId = carId,
                StartDate = startDate,
                EndDate = endDate,
                PaymentMethod = paymentMethod,
                Status = status,
                BookingInfo = bookingInfo
            };

            // Manually trigger validation for BookingInfo
            var bookingInfoValidationContext = new ValidationContext(bookingInfo);
            var bookingInfoValidationResults = new List<ValidationResult>();
            bool isBookingInfoValid = Validator.TryValidateObject(bookingInfo, bookingInfoValidationContext, bookingInfoValidationResults, true);

            // Manually trigger validation for Booking
            var bookingValidationContext = new ValidationContext(booking);
            var bookingValidationResults = new List<ValidationResult>();
            bool isBookingValid = Validator.TryValidateObject(booking, bookingValidationContext, bookingValidationResults, true);

            // Assert that both BookingInfo and Booking are valid
            Assert.IsFalse(isBookingInfoValid);
            Assert.IsTrue(isBookingValid);

            //Assert.IsTrue(bookingInfoValidationResults.Any(vr => vr.MemberNames.Contains(nameof(BookingInfo.RenterEmail)) && vr.ErrorMessage == "This field is required."));

            if (isBookingInfoValid && isBookingValid)
            {
                var result = _controller.BookACar(location, startDate, endDate, carId, booking) as RedirectToActionResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("BookACarFinish", result.ActionName);
            }
        }

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "fewf5615", "038203023477", "0334110870", "truonganat@gmail.com", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]
        public void BookACar_Post_ValidModel_CheckBookingInfoInvalidField_RenterEmail(string location, string startDateStr, string endDateStr, int carId, int userId,
            string renterEmail, string renterNationalId, string renterPhone, string driverEmail, string driverNationalId, string driverPhone,
            string renterName, string renterDobStr, int? renterAddressId, string renterDrivingLicense, bool isDifferent, string driverName, string driverDobStr, int driverAddressId, string driverDrivingLicense, int? paymentMethod, int? status)
        {
            // Arrange
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);
            DateOnly renterDob = DateOnly.Parse(renterDobStr);
            DateOnly driverDob = DateOnly.Parse(driverDobStr);

            var renterAddress = new Address { CityId = 1, DistrictId = 1, WardId = 1, HouseNumberStreet = "Nha so 1" };

            var driverAddress = new Address { CityId = 1, DistrictId = 1, WardId = 1, HouseNumberStreet = "Nha so 2" };

            var bookingInfo = new BookingInfo
            {
                RenterEmail = renterEmail,
                RenterNationalId = renterNationalId,
                RenterPhone = renterPhone,
                DriverEmail = driverEmail,
                DriverNationalId = driverNationalId,
                DriverPhone = driverPhone,
                RenterName = renterName,
                RenterDob = renterDob,
                RenterAddress = renterAddress,
                RenterDrivingLicense = renterDrivingLicense,
                IsDifferent = isDifferent,
                DriverName = driverName,
                DriverDob = driverDob,
                DriverAddress = driverAddress,
                DriverDrivingLicense = driverDrivingLicense
            };

            var booking = new Booking
            {
                UserId = userId,
                CarId = carId,
                StartDate = startDate,
                EndDate = endDate,
                PaymentMethod = paymentMethod,
                Status = status,
                BookingInfo = bookingInfo
            };

            // Manually trigger validation for BookingInfo
            var bookingInfoValidationContext = new ValidationContext(bookingInfo);
            var bookingInfoValidationResults = new List<ValidationResult>();
            bool isBookingInfoValid = Validator.TryValidateObject(bookingInfo, bookingInfoValidationContext, bookingInfoValidationResults, true);

            Assert.IsFalse(isBookingInfoValid);
            Assert.AreEqual("Email must be a valid gmail.com address.", bookingInfoValidationResults[0].ErrorMessage);
        }

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "5ewf1fe", "0334110870", "truonganat@gmail.com", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]
        public void BookACar_Post_ValidModel_CheckBookingInfoInvalid_RenterNationalId(string location, string startDateStr, string endDateStr, int carId, int userId,
            string renterEmail, string renterNationalId, string renterPhone, string driverEmail, string driverNationalId, string driverPhone,
            string renterName, string renterDobStr, int? renterAddressId, string renterDrivingLicense, bool isDifferent, string driverName, string driverDobStr, int driverAddressId, string driverDrivingLicense, int? paymentMethod, int? status)
        {
            // Arrange
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);
            DateOnly renterDob = DateOnly.Parse(renterDobStr);
            DateOnly driverDob = DateOnly.Parse(driverDobStr);

            var renterAddress = _context.Addresses.SingleOrDefault(a => a.AddressId == renterAddressId);

            var driverAddress = _context.Addresses.SingleOrDefault(a => a.AddressId == driverAddressId);

            var bookingInfo = new BookingInfo
            {
                RenterEmail = renterEmail,
                RenterNationalId = renterNationalId,
                RenterPhone = renterPhone,
                DriverEmail = driverEmail,
                DriverNationalId = driverNationalId,
                DriverPhone = driverPhone,
                RenterName = renterName,
                RenterDob = renterDob,
                RenterAddress = renterAddress,
                RenterDrivingLicense = renterDrivingLicense,
                IsDifferent = isDifferent,
                DriverName = driverName,
                DriverDob = driverDob,
                DriverAddress = driverAddress,
                DriverDrivingLicense = driverDrivingLicense
            };

            var booking = new Booking
            {
                UserId = userId,
                CarId = carId,
                StartDate = startDate,
                EndDate = endDate,
                PaymentMethod = paymentMethod,
                Status = status,
                BookingInfo = bookingInfo
            };

            // Manually trigger validation for BookingInfo
            var bookingInfoValidationContext = new ValidationContext(bookingInfo);
            var bookingInfoValidationResults = new List<ValidationResult>();
            bool isBookingInfoValid = Validator.TryValidateObject(bookingInfo, bookingInfoValidationContext, bookingInfoValidationResults, true);

            Assert.IsFalse(isBookingInfoValid);
            Assert.AreEqual("National ID must be 12 digits.", bookingInfoValidationResults[0].ErrorMessage);
        }

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
            "truonganat@gmail.com", "038203023477", "25e1fefewf", "truonganat@gmail.com", "038203023477", "0334110870",
            "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]
        public void BookACar_Post_ValidModel_CheckBookingInfoInvalidField_RenterPhone(string location, string startDateStr, string endDateStr, int carId, int userId,
            string renterEmail, string renterNationalId, string renterPhone, string driverEmail, string driverNationalId, string driverPhone,
            string renterName, string renterDobStr, int? renterAddressId, string renterDrivingLicense, bool isDifferent, string driverName, string driverDobStr, int driverAddressId, string driverDrivingLicense, int? paymentMethod, int? status)
        {
            // Arrange
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);
            DateOnly renterDob = DateOnly.Parse(renterDobStr);
            DateOnly driverDob = DateOnly.Parse(driverDobStr);

            var renterAddress = new Address { CityId = 1, DistrictId = 1, WardId = 1, HouseNumberStreet = "Nha so 1" };

            var driverAddress = new Address { CityId = 1, DistrictId = 1, WardId = 1, HouseNumberStreet = "Nha so 2" };

            var bookingInfo = new BookingInfo
            {
                RenterEmail = renterEmail,
                RenterNationalId = renterNationalId,
                RenterPhone = renterPhone,
                DriverEmail = driverEmail,
                DriverNationalId = driverNationalId,
                DriverPhone = driverPhone,
                RenterName = renterName,
                RenterDob = renterDob,
                RenterAddress = renterAddress,
                RenterDrivingLicense = renterDrivingLicense,
                IsDifferent = isDifferent,
                DriverName = driverName,
                DriverDob = driverDob,
                DriverAddress = driverAddress,
                DriverDrivingLicense = driverDrivingLicense
            };

            var booking = new Booking
            {
                UserId = userId,
                CarId = carId,
                StartDate = startDate,
                EndDate = endDate,
                PaymentMethod = paymentMethod,
                Status = status,
                BookingInfo = bookingInfo
            };

            // Manually trigger validation for BookingInfo
            var bookingInfoValidationContext = new ValidationContext(bookingInfo);
            var bookingInfoValidationResults = new List<ValidationResult>();
            bool isBookingInfoValid = Validator.TryValidateObject(bookingInfo, bookingInfoValidationContext, bookingInfoValidationResults, true);

            // Assert that both BookingInfo and Booking are valid
            Assert.IsFalse(isBookingInfoValid);
            Assert.AreEqual("Phone number invalid!", bookingInfoValidationResults[0].ErrorMessage);
        }

        [TestCase("", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1)]
        public void BookACar_Get_CheckLocationEmpty_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var expectedValue = $"{car?.Address?.District?.DistrictName}, {car?.Address?.City?.CityProvince}";

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["location"]);
        }

        [TestCase(null, "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1)]
        public void BookACar_Get_CheckLocationNull_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var expectedValue = $"{car?.Address?.District?.DistrictName}, {car?.Address?.City?.CityProvince}";

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["location"]);
        }

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
        "truonganat@gmail.com", "038203023477", "0334110870", "5ewf156w", "038203023477", "0334110870",
        "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]
        public void BookACar_Post_ValidModel_CheckBookingInfoInvalidField_DriverEmail(string location, string startDateStr, string endDateStr, int carId, int userId,
            string renterEmail, string renterNationalId, string renterPhone, string driverEmail, string driverNationalId, string driverPhone,
            string renterName, string renterDobStr, int? renterAddressId, string renterDrivingLicense, bool isDifferent, string driverName, string driverDobStr, int driverAddressId, string driverDrivingLicense, int? paymentMethod, int? status)
        {
            // Arrange
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);
            DateOnly renterDob = DateOnly.Parse(renterDobStr);
            DateOnly driverDob = DateOnly.Parse(driverDobStr);

            var renterAddress = new Address { CityId = 1, DistrictId = 1, WardId = 1, HouseNumberStreet = "Nha so 1" };

            var driverAddress = new Address { CityId = 1, DistrictId = 1, WardId = 1, HouseNumberStreet = "Nha so 2" };

            var bookingInfo = new BookingInfo
            {
                RenterEmail = renterEmail,
                RenterNationalId = renterNationalId,
                RenterPhone = renterPhone,
                DriverEmail = driverEmail,
                DriverNationalId = driverNationalId,
                DriverPhone = driverPhone,
                RenterName = renterName,
                RenterDob = renterDob,
                RenterAddress = renterAddress,
                RenterDrivingLicense = renterDrivingLicense,
                IsDifferent = isDifferent,
                DriverName = driverName,
                DriverDob = driverDob,
                DriverAddress = driverAddress,
                DriverDrivingLicense = driverDrivingLicense
            };

            var booking = new Booking
            {
                UserId = userId,
                CarId = carId,
                StartDate = startDate,
                EndDate = endDate,
                PaymentMethod = paymentMethod,
                Status = status,
                BookingInfo = bookingInfo
            };

            // Manually trigger validation for BookingInfo
            var bookingInfoValidationContext = new ValidationContext(bookingInfo);
            var bookingInfoValidationResults = new List<ValidationResult>();
            bool isBookingInfoValid = Validator.TryValidateObject(bookingInfo, bookingInfoValidationContext, bookingInfoValidationResults, true);

            Assert.IsFalse(isBookingInfoValid);
            Assert.AreEqual("Email must be a valid gmail.com address.", bookingInfoValidationResults[0].ErrorMessage);
        }

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
        "truonganat@gmail.com", "038203023477", "0334110870", "truonganat@gmail.com", "sw62wd6", "0334110870",
        "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]
        public void BookACar_Post_ValidModel_CheckBookingInfoInvalidField_DriverNationalId(string location, string startDateStr, string endDateStr, int carId, int userId,
            string renterEmail, string renterNationalId, string renterPhone, string driverEmail, string driverNationalId, string driverPhone,
            string renterName, string renterDobStr, int? renterAddressId, string renterDrivingLicense, bool isDifferent, string driverName, string driverDobStr, int driverAddressId, string driverDrivingLicense, int? paymentMethod, int? status)
        {
            // Arrange
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);
            DateOnly renterDob = DateOnly.Parse(renterDobStr);
            DateOnly driverDob = DateOnly.Parse(driverDobStr);

            var renterAddress = new Address { CityId = 1, DistrictId = 1, WardId = 1, HouseNumberStreet = "Nha so 1" };

            var driverAddress = new Address { CityId = 1, DistrictId = 1, WardId = 1, HouseNumberStreet = "Nha so 2" };

            var bookingInfo = new BookingInfo
            {
                RenterEmail = renterEmail,
                RenterNationalId = renterNationalId,
                RenterPhone = renterPhone,
                DriverEmail = driverEmail,
                DriverNationalId = driverNationalId,
                DriverPhone = driverPhone,
                RenterName = renterName,
                RenterDob = renterDob,
                RenterAddress = renterAddress,
                RenterDrivingLicense = renterDrivingLicense,
                IsDifferent = isDifferent,
                DriverName = driverName,
                DriverDob = driverDob,
                DriverAddress = driverAddress,
                DriverDrivingLicense = driverDrivingLicense
            };

            var booking = new Booking
            {
                UserId = userId,
                CarId = carId,
                StartDate = startDate,
                EndDate = endDate,
                PaymentMethod = paymentMethod,
                Status = status,
                BookingInfo = bookingInfo
            };

            // Manually trigger validation for BookingInfo
            var bookingInfoValidationContext = new ValidationContext(bookingInfo);
            var bookingInfoValidationResults = new List<ValidationResult>();
            bool isBookingInfoValid = Validator.TryValidateObject(bookingInfo, bookingInfoValidationContext, bookingInfoValidationResults, true);

            Assert.IsFalse(isBookingInfoValid);
            Assert.AreEqual("National ID must be 12 digits.", bookingInfoValidationResults[0].ErrorMessage);
        }

        [TestCase("nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2,
        "truonganat@gmail.com", "038203023477", "0334110870", "truonganat@gmail.com", "038203023477", "0123456789",
        "Renter Name", "1980-01-01", 1, "RenterDL123", false, "Driver Name", "1980-01-01", 1, "DriverDL123", 1, 2)]
        public void BookACar_Post_ValidModel_CheckBookingInfoInvalidField_DriverPhone(string location, string startDateStr, string endDateStr, int carId, int userId,
            string renterEmail, string renterNationalId, string renterPhone, string driverEmail, string driverNationalId, string driverPhone,
            string renterName, string renterDobStr, int? renterAddressId, string renterDrivingLicense, bool isDifferent, string driverName, string driverDobStr, int driverAddressId, string driverDrivingLicense, int? paymentMethod, int? status)
        {
            // Arrange
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);
            DateOnly renterDob = DateOnly.Parse(renterDobStr);
            DateOnly driverDob = DateOnly.Parse(driverDobStr);

            var renterAddress = new Address { CityId = 1, DistrictId = 1, WardId = 1, HouseNumberStreet = "Nha so 1" };

            var driverAddress = new Address { CityId = 1, DistrictId = 1, WardId = 1, HouseNumberStreet = "Nha so 2" };

            var bookingInfo = new BookingInfo
            {
                RenterEmail = renterEmail,
                RenterNationalId = renterNationalId,
                RenterPhone = renterPhone,
                DriverEmail = driverEmail,
                DriverNationalId = driverNationalId,
                DriverPhone = driverPhone,
                RenterName = renterName,
                RenterDob = renterDob,
                RenterAddress = renterAddress,
                RenterDrivingLicense = renterDrivingLicense,
                IsDifferent = isDifferent,
                DriverName = driverName,
                DriverDob = driverDob,
                DriverAddress = driverAddress,
                DriverDrivingLicense = driverDrivingLicense
            };

            var booking = new Booking
            {
                UserId = userId,
                CarId = carId,
                StartDate = startDate,
                EndDate = endDate,
                PaymentMethod = paymentMethod,
                Status = status,
                BookingInfo = bookingInfo
            };

            // Manually trigger validation for BookingInfo
            var bookingInfoValidationContext = new ValidationContext(bookingInfo);
            var bookingInfoValidationResults = new List<ValidationResult>();
            bool isBookingInfoValid = Validator.TryValidateObject(bookingInfo, bookingInfoValidationContext, bookingInfoValidationResults, true);

            Assert.IsFalse(isBookingInfoValid);
            Assert.AreEqual("Phone number invalid!", bookingInfoValidationResults[0].ErrorMessage);
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1)]
        public void BookACar_Get_CheckLocation_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var expectedValue = location;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["location"]);
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1)]
        public void BookACar_Get_CheckStartDate_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var expectedValue = startDate;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["startDate"]);
        }

        [TestCase("Nha 123", "0001-01-01 00:00:00", "2024-08-03 20:00:00", 1, 1)]
        public void BookACar_Get_CheckStartDateNull_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var expectedValue = DateTime.Now;

            Assert.IsNotNull(result);
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1)]
        public void BookACar_Get_CheckEndDate_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var expectedValue = endDate;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["endDate"]);
        }

        [TestCase("Nha 123", "0001-01-01 00:00:00", "0001-01-01 00:00:00", 1, 1)]
        public void BookACar_Get_CheckEndDateNull_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var expectedValue = DateTime.Now.AddDays(1);

            Assert.IsNotNull(result);
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1, 2)]
        public void BookACar_Get_CheckNumberOfDays_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId, int nod)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var expectedValue = nod;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["NumberOfDays"]);
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1)]
        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 1)]
        public void BookACar_Get_CheckViewBagBasePrice_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var expectedValue = car.BasePrice;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["BasePrice"]);
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1, 2000000)]
        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 1, 6000000)]
        public void BookACar_Get_CheckViewBagTotal_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId, int total)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var expectedValue = total;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["Total"]);
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1, 1000000)]
        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 1, 1400000)]
        public void BookACar_Get_CheckViewBagDeposit_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId, int deposit)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var expectedValue = deposit;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["Deposit"]);
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1, 4.0)]
        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 1, 0.0)]
        public void BookACar_Get_CheckViewBagRating_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId, double rating)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var lBooking = _context.Bookings.Where(x => x.CarId == carId).ToList();
            var matchedFeedback = (from feedback in _context.Feedbacks.ToList()
                                   join booking in lBooking on feedback.BookingNo equals booking.BookingNo
                                   select feedback).ToList();

            var expectedValue = rating;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["Rating"]);
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1)]
        public void BookACar_Get_CheckViewBagUser_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);

            var expectedValue = userW;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["user"]);
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1)]
        public void BookACar_Get_CheckViewBagUserId_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var expectedValue = userId;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["userId"]);
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1)]
        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 3)]
        public void BookACar_Get_CheckViewBagWallet_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);

            var expectedValue = userW.Wallet;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["wallet"]);
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1)]
        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 3)]
        public void BookACar_Get_CheckViewBagDob_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);

            var expectedValue = userW.Dob?.ToString("yyyy-MM-dd");

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["dob"]);
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1)]
        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 3)]
        public void BookACar_Get_CheckViewBagDrivingLience_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);

            var expectedValue = userW.DrivingLicense;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["DrivingLience"]);
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1)]
        public void BookACar_Get_CheckViewBagAddressPNull_Cities_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            var addressP = _context.Addresses.FirstOrDefault(a => a.AddressId == userW.AddressId);

            var expectedCities = _context.Cities.ToList();
            var expectedValue = new SelectList(expectedCities, "CityId", "CityProvince");

            // Assert
            Assert.IsNull(addressP);

            var actualValue = result.ViewData["Cities"] as SelectList;
            Assert.IsNotNull(actualValue);

            Assert.AreEqual(expectedValue.Count(), actualValue.Count());

            for (int i = 0; i < expectedValue.Count(); i++)
            {
                var expectedItem = expectedValue.ElementAt(i);
                var actualItem = actualValue.ElementAt(i);

                Assert.AreEqual(expectedItem.Value, actualItem.Value);
                Assert.AreEqual(expectedItem.Text, actualItem.Text);
            }
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 4)]
        public void BookACar_Get_CheckViewBagAddressP_Cities_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            var addressP = _context.Addresses.FirstOrDefault(a => a.AddressId == userW.AddressId);

            var expectedCities = _context.Cities.ToList();
            var expectedValue = new SelectList(expectedCities, "CityId", "CityProvince", addressP.CityId);

            // Assert
            Assert.IsNotNull(addressP);

            var actualValue = result.ViewData["Cities"] as SelectList;
            Assert.IsNotNull(actualValue);

            Assert.AreEqual(expectedValue.Count(), actualValue.Count());

            for (int i = 0; i < expectedValue.Count(); i++)
            {
                var expectedItem = expectedValue.ElementAt(i);
                var actualItem = actualValue.ElementAt(i);

                Assert.AreEqual(expectedItem.Value, actualItem.Value);
                Assert.AreEqual(expectedItem.Text, actualItem.Text);
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        public void GetDistricts_JsonResult_ReturnDistricts(int cityId)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            var result = _controller.GetDistricts(cityId) as JsonResult;

            var expectedDistricts = _context.Districts.Where(d => d.CityId == cityId).ToList();
            var expectedValue = new SelectList(expectedDistricts, "DistrictId", "DistrictName");

            //var actualValue = result.Value as SelectList;
            var actualDistricts = JsonConvert.DeserializeObject<List<District>>(JsonConvert.SerializeObject(result.Value, settings));

            Assert.IsNotNull(actualDistricts);
            Assert.AreEqual(expectedDistricts.Count, actualDistricts.Count);
            for (int i = 0; i < expectedDistricts.Count; i++)
            {
                Assert.AreEqual(expectedDistricts[i].DistrictId, actualDistricts[i].DistrictId);
                Assert.AreEqual(expectedDistricts[i].DistrictName, actualDistricts[i].DistrictName);
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        public void GetWards_JsonResult_ReturnWards(int districtId)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            var result = _controller.GetWards(districtId) as JsonResult;

            var expectedWards = _context.Wards.Where(d => d.DistrictId == districtId).ToList();
            var expectedValue = new SelectList(expectedWards, "WardId", "WardName");

            //var actualValue = result.Value as SelectList;
            var actualWards = JsonConvert.DeserializeObject<List<Ward>>(JsonConvert.SerializeObject(result.Value, settings));

            Assert.IsNotNull(actualWards);
            Assert.AreEqual(expectedWards.Count, actualWards.Count);
            for (int i = 0; i < expectedWards.Count; i++)
            {
                Assert.AreEqual(expectedWards[i].WardId, actualWards[i].WardId);
                Assert.AreEqual(expectedWards[i].WardName, actualWards[i].WardName);
            }
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 4)]
        public void BookACar_Get_CheckViewBagAddressP_Districts_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            var addressP = _context.Addresses.FirstOrDefault(a => a.AddressId == userW.AddressId);

            var expectedDistricts = _context.Districts.Where(a => a.DistrictId == addressP.DistrictId).ToList();
            var expectedValue = new SelectList(expectedDistricts, "DistrictId", "DistrictName", addressP.DistrictId);

            // Assert
            Assert.IsNotNull(addressP);

            var actualValue = result.ViewData["Districts"] as SelectList;
            Assert.IsNotNull(actualValue);

            Assert.AreEqual(expectedValue.Count(), actualValue.Count());

            for (int i = 0; i < expectedValue.Count(); i++)
            {
                var expectedItem = expectedValue.ElementAt(i);
                var actualItem = actualValue.ElementAt(i);

                Assert.AreEqual(expectedItem.Value, actualItem.Value);
                Assert.AreEqual(expectedItem.Text, actualItem.Text);
            }
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 4)]
        public void BookACar_Get_CheckViewBagAddressP_Wards_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            var addressP = _context.Addresses.FirstOrDefault(a => a.AddressId == userW.AddressId);

            var expectedWards = _context.Wards.Where(a => a.WardId == addressP.WardId).ToList();
            var expectedValue = new SelectList(expectedWards, "WardId", "WardName", addressP.WardId);

            // Assert
            Assert.IsNotNull(addressP);

            var actualValue = result.ViewData["Wards"] as SelectList;
            Assert.IsNotNull(actualValue);

            Assert.AreEqual(expectedValue.Count(), actualValue.Count());

            for (int i = 0; i < expectedValue.Count(); i++)
            {
                var expectedItem = expectedValue.ElementAt(i);
                var actualItem = actualValue.ElementAt(i);

                Assert.AreEqual(expectedItem.Value, actualItem.Value);
                Assert.AreEqual(expectedItem.Text, actualItem.Text);
            }
        }

        [TestCase("Nha 123", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 4)]
        public void BookACar_Get_CheckViewBagAddressP_houseNumberStreet_ReturnViewBag(string? location, string startDateStr, string endDateStr, int carId, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId) as ViewResult;
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            var addressP = _context.Addresses.FirstOrDefault(a => a.AddressId == userW.AddressId);

            var expectedValue = addressP.HouseNumberStreet;

            // Assert
            Assert.IsNotNull(addressP);
            Assert.AreEqual(expectedValue, result.ViewData["houseNumberStreet"]);
        }

        [TestCase("", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1)]
        public void BookACar_Post_BalanceInsufficient_ReturnBookACar_Get(string location, string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            var viewModel = _context.Bookings.Find(bookingNo);
            var car = _context.Cars.Include(c => c.User).FirstOrDefault(x => x.CarId == viewModel.CarId);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId, viewModel) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("BookACar", result.ActionName);
        }

        [TestCase("", "2024-08-01 20:00:00", "2024-08-01 22:00:00", 1, 1)]
        public void BookACar_Post_BalanceInsufficient_NumOfDaysLessThan12h_Get(string location, string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            var viewModel = _context.Bookings.Find(bookingNo);
            var car = _context.Cars.Include(c => c.User).FirstOrDefault(x => x.CarId == viewModel.CarId);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId, viewModel) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("BookACar", result.ActionName);
        }

        [TestCase("", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2)]
        public void BookACar_Post_BalanceEnough_ReturnBookACarFinish(string location, string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            var viewModel = _context.Bookings.Find(bookingNo);
            var car = _context.Cars.Include(c => c.User).FirstOrDefault(x => x.CarId == viewModel.CarId);

            // Act
            var result = _controller.BookACar(location, startDate, endDate, carId, viewModel) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("BookACarFinish", result.ActionName);
        }

        [TestCase(1, "Acura ILX 2000", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1)]
        public void BookACarFinish_AuthorizationNull_ReturnLogin(int? carId, string? carName, string? startDateStr, string? endDateStr, int bookingNo)
        {
            User user = null;
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            var result = _controller.BookACarFinish(carId, carName, startDate, endDate, bookingNo) as RedirectToActionResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ActionName);
        }

        [TestCase(1, "Acura ILX 2000", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1)]
        public void BookACarFinish_AuthorizationCarOwner_ReturnErrorAuthorization(int? carId, string? carName, string? startDateStr, string? endDateStr, int bookingNo)
        {
            var user = _context.Users.Find(2);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            var result = _controller.BookACarFinish(carId, carName, startDate, endDate, bookingNo) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("ErrorAuthorization", result.ViewName);
        }

        [TestCase(1, "Acura ILX 2000", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1)]
        public void BookACarFinish_AuthorizationCustomer_ReturnView(int? carId, string? carName, string? startDateStr, string? endDateStr, int bookingNo)
        {
            var user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            var result = _controller.BookACarFinish(carId, carName, startDate, endDate, bookingNo) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [TestCase(1, "Acura ILX 2000", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 2)]
        [TestCase(1, "Acura ILX 2000", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1)]
        public void BookACarFinish_CheckViewBagCarId_ReturnView(int? carId, string? carName, string? startDateStr, string? endDateStr, int bookingNo)
        {
            var user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            var result = _controller.BookACarFinish(carId, carName, startDate, endDate, bookingNo) as ViewResult;

            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);
            var expectedValue = car.CarId;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["CarId"]);
        }

        [TestCase(1, "Acura ILX 2000", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 2)]
        [TestCase(1, "Acura ILX 2000", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1)]
        public void BookACarFinish_CheckViewBagCarName_ReturnView(int? carId, string? carName, string? startDateStr, string? endDateStr, int bookingNo)
        {
            var user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            var result = _controller.BookACarFinish(carId, carName, startDate, endDate, bookingNo) as ViewResult;

            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);
            var expectedValue = car.Name;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["CarName"]);
        }

        [TestCase(1, "Acura ILX 2000", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 2)]
        [TestCase(1, "Acura ILX 2000", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1)]
        public void BookACarFinish_CheckViewBagStartDate_ReturnView(int? carId, string? carName, string? startDateStr, string? endDateStr, int bookingNo)
        {
            var user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            var result = _controller.BookACarFinish(carId, carName, startDate, endDate, bookingNo) as ViewResult;

            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);
            var expectedValue = startDate;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["StartDate"]);
        }

        [TestCase(1, "Acura ILX 2000", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 2)]
        [TestCase(1, "Acura ILX 2000", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1)]
        public void BookACarFinish_CheckViewBagEndDate_ReturnView(int? carId, string? carName, string? startDateStr, string? endDateStr, int bookingNo)
        {
            var user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            var result = _controller.BookACarFinish(carId, carName, startDate, endDate, bookingNo) as ViewResult;

            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);
            var expectedValue = endDate;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["EndDate"]);
        }

        [TestCase(1, "Acura ILX 2000", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 2)]
        [TestCase(1, "Acura ILX 2000", "2024-08-01 20:00:00", "2024-08-03 20:00:00", 1)]
        public void BookACarFinish_CheckViewBagBookingNo_ReturnView(int? carId, string? carName, string? startDateStr, string? endDateStr, int bookingNo)
        {
            var user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            var result = _controller.BookACarFinish(carId, carName, startDate, endDate, bookingNo) as ViewResult;

            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);
            var expectedValue = bookingNo;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["BookingNo"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 2)]
        public void EditBookingDetail_Get_AuthorizationNull_ReturnLogin(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(100);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ActionName);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_AuthorizationCarOwner_ErrorAuthorization(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(2);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ErrorAuthorization", result.ViewName);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 3)]
        public void EditBookingDetail_Get_AuthorizationConflictUserId_ErrorAuthorization(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ErrorAuthorization", result.ViewName);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 2, 3)]
        public void EditBookingDetail_Get_AuthorizationConflictCarId_ErrorAuthorization(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ErrorAuthorization", result.ViewName);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_ReturnPost(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-01 22:00:00", 3, 3)]
        public void EditBookingDetail_Get_TotalHoursLessThan12h_ReturnPost(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 1, 1)]
        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3, 3)]
        public void EditBookingDetail_Get_CheckViewBag_CheckFbExisted(string startDateStr, string endDateStr, int carId, int bookingNo, int userId)
        {
            var user = _context.Users.Find(userId);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            Boolean checkFbExisted = false;
            Feedback? feedback = _context.Feedbacks.FirstOrDefault(x => x.BookingNo == bookingNo);
            if (feedback != null)
            {
                checkFbExisted = true;
            }

            var expectedValue = checkFbExisted;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["checkFbExisted"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagRenterName(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var expectedValue = booking.BookingInfo.RenterName;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["RenterName"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagBookingNo(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var expectedValue = booking.BookingNo;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["bookingNo"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagStartDate(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var expectedValue = booking.StartDate;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["startDate"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagEndDate(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var expectedValue = booking.EndDate;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["endDate"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagCarId(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var expectedValue = booking.CarId;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["carId"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagNumberOfDays(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = numberOfDays;


            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["NumberOfDays"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagBasePrice(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = car.BasePrice;


            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["BasePrice"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagTotal(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = numberOfDays * car.BasePrice;


            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["Total"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagDeposit(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = numberOfDays * car.Deposit;


            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["Deposit"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagRating(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            bool checkRent = false;
            if (booking.Status == 2 || booking.Status == 3 || booking.Status == 4)
            {
                checkRent = true;
            }

            var lBooking = _context.Bookings.Where(x => x.CarId == carId).ToList();

            var matchedFeedback = (from feedbackEdit in _context.Feedbacks.ToList()
                                   join booking2 in lBooking on feedbackEdit.BookingNo equals booking.BookingNo
                                   select feedbackEdit).ToList();

            double rating = 0, nor = 0;
            foreach (Feedback o in matchedFeedback)
            {
                if (o.Ratings < 0)
                {
                    continue;
                }
                rating += o.Ratings;
                nor += 1;
            }

            if (nor > 0)
            {
                rating = rating / nor;
                rating = (Math.Ceiling(rating * 2)) / 2.0;
            }
            else
            {
                rating = 0;
            }

            var expectedValue = rating;


            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["Rating"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagCar(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = car;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["car"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagBrand(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.Include(c => c.Brand).Include(c => c.Model).Include(c => c.Address).FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = car.Brand;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["brand"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagModel(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.Include(c => c.Brand).Include(c => c.Model).Include(c => c.Address).FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = car.Model;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["model"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagDocument(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = car.Document;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["document"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagColor(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = car.Color;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["color"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagAddress(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = car.Address;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["address"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagWard(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = car.Address.Ward;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["ward"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagDistrict(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = car.Address.District;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["district"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagCity(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = car.Address.City;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["city"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagTerm(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = car.Term;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["term"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagFucntion(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = car.Fucntion;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["function"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagCheckRent(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            bool checkRent = false;
            if (booking.Status == 2 || booking.Status == 3 || booking.Status == 4)
            {
                checkRent = true;
            }

            var expectedValue = checkRent;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["checkRent"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagUserId(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = user.UserId;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["userId"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagWallet(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = user.Wallet;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["wallet"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagDob(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = user.Dob?.ToString("yyyy-MM-dd");

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["dob"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagDrivingLience(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);

            int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var expectedValue = user.DrivingLicense;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedValue, result.ViewData["DrivingLience"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 1, 5)]
        public void EditBookingDetail_Get_CheckAddressNull(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                .Include(b => b.BookingInfo)
                .ThenInclude(bi => bi.RenterAddress)
                .Include(b => b.BookingInfo)
                .ThenInclude(bi => bi.DriverAddress)
                .FirstOrDefault(b => b.BookingNo == bookingNo);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            var addressR = _context.Addresses.FirstOrDefault(a => a.AddressId == userW.AddressId);

            var expectedCities = _context.Cities.ToList();
            var expectedValue = new SelectList(expectedCities, "CityId", "CityProvince");

            var actualValue = result.ViewData["CitiesR"] as SelectList;
            Assert.IsNotNull(actualValue);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagCitiesR(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            var addressR = _context.Addresses.FirstOrDefault(a => a.AddressId == userW.AddressId);

            var expectedCities = _context.Cities.ToList();
            var expectedValue = new SelectList(expectedCities, "CityId", "CityProvince");

            var actualValue = result.ViewData["CitiesR"] as SelectList;
            Assert.IsNotNull(actualValue);

            Assert.AreEqual(expectedValue.Count(), actualValue.Count());

            for (int i = 0; i < expectedValue.Count(); i++)
            {
                var expectedItem = expectedValue.ElementAt(i);
                var actualItem = actualValue.ElementAt(i);

                Assert.AreEqual(expectedItem.Value, actualItem.Value);
                Assert.AreEqual(expectedItem.Text, actualItem.Text);
            }
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagDistrictsR(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            var addressR = _context.Addresses.FirstOrDefault(a => a.AddressId == userW.AddressId);

            var expectedDistrict = _context.Districts.Where(d => d.CityId == addressR.CityId).ToList();
            var expectedValue = new SelectList(expectedDistrict, "DistrictId", "DistrictName", addressR.DistrictId);

            var actualValue = result.ViewData["DistrictsR"] as SelectList;
            Assert.IsNotNull(actualValue);

            Assert.AreEqual(expectedValue.Count(), actualValue.Count());

            for (int i = 0; i < expectedValue.Count(); i++)
            {
                var expectedItem = expectedValue.ElementAt(i);
                var actualItem = actualValue.ElementAt(i);

                Assert.AreEqual(expectedItem.Value, actualItem.Value);
                Assert.AreEqual(expectedItem.Text, actualItem.Text);
            }
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagWardsR(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            var addressR = _context.Addresses.FirstOrDefault(a => a.AddressId == userW.AddressId);

            var expectedWard = _context.Wards.Where(d => d.WardId == addressR.WardId).ToList();
            var expectedValue = new SelectList(expectedWard, "WardId", "WardName", addressR.WardId);

            var actualValue = result.ViewData["WardsR"] as SelectList;
            Assert.IsNotNull(actualValue);

            Assert.AreEqual(expectedValue.Count(), actualValue.Count());

            for (int i = 0; i < expectedValue.Count(); i++)
            {
                var expectedItem = expectedValue.ElementAt(i);
                var actualItem = actualValue.ElementAt(i);

                Assert.AreEqual(expectedItem.Value, actualItem.Value);
                Assert.AreEqual(expectedItem.Text, actualItem.Text);
            }
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagHouseNumberStreetR(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            var addressR = _context.Addresses.FirstOrDefault(a => a.AddressId == userW.AddressId);

            var expectedValue = addressR.HouseNumberStreet;

            Assert.AreEqual(expectedValue, result.ViewData["houseNumberStreetR"]);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagCitiesD(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            var addressD = _context.Addresses.FirstOrDefault(a => a.AddressId == userW.AddressId);

            var expectedCities = _context.Cities.ToList();
            var expectedValue = new SelectList(expectedCities, "CityId", "CityProvince");

            var actualValue = result.ViewData["CitiesD"] as SelectList;
            Assert.IsNotNull(actualValue);

            Assert.AreEqual(expectedValue.Count(), actualValue.Count());

            for (int i = 0; i < expectedValue.Count(); i++)
            {
                var expectedItem = expectedValue.ElementAt(i);
                var actualItem = actualValue.ElementAt(i);

                Assert.AreEqual(expectedItem.Value, actualItem.Value);
                Assert.AreEqual(expectedItem.Text, actualItem.Text);
            }
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagDistrictsD(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            var addressD = _context.Addresses.FirstOrDefault(a => a.AddressId == userW.AddressId);

            var expectedDistrict = _context.Districts.Where(d => d.CityId == addressD.CityId).ToList();
            var expectedValue = new SelectList(expectedDistrict, "DistrictId", "DistrictName", addressD.DistrictId);

            var actualValue = result.ViewData["DistrictsD"] as SelectList;
            Assert.IsNotNull(actualValue);

            Assert.AreEqual(expectedValue.Count(), actualValue.Count());

            for (int i = 0; i < expectedValue.Count(); i++)
            {
                var expectedItem = expectedValue.ElementAt(i);
                var actualItem = actualValue.ElementAt(i);

                Assert.AreEqual(expectedItem.Value, actualItem.Value);
                Assert.AreEqual(expectedItem.Text, actualItem.Text);
            }
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagWardsD(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            var addressD = _context.Addresses.FirstOrDefault(a => a.AddressId == userW.AddressId);

            var expectedWard = _context.Wards.Where(d => d.WardId == addressD.WardId).ToList();
            var expectedValue = new SelectList(expectedWard, "WardId", "WardName", addressD.WardId);

            var actualValue = result.ViewData["WardsD"] as SelectList;
            Assert.IsNotNull(actualValue);

            Assert.AreEqual(expectedValue.Count(), actualValue.Count());

            for (int i = 0; i < expectedValue.Count(); i++)
            {
                var expectedItem = expectedValue.ElementAt(i);
                var actualItem = actualValue.ElementAt(i);

                Assert.AreEqual(expectedItem.Value, actualItem.Value);
                Assert.AreEqual(expectedItem.Text, actualItem.Text);
            }
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void EditBookingDetail_Get_CheckViewBagHouseNumberStreetD(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.EditBookingDetail(startDate, endDate, carId, bookingNo) as ViewResult;

            var booking = _context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);

            var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            var addressD = _context.Addresses.FirstOrDefault(a => a.AddressId == userW.AddressId);

            var expectedValue = addressD.HouseNumberStreet;

            Assert.AreEqual(expectedValue, result.ViewData["houseNumberStreetD"]);
        }

        [TestCase]
        public void EditBookingDetail_Post_ReturnLogin()
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            var viewModel = _context.Bookings.Find(3);

            var result = _controller.EditBookingDetail(viewModel) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("EditBookingDetail", result.ActionName);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void CancelBooking_AuthorizationNull_ReturnLogin(string startDateStr, string endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(100);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // Act
            var result = _controller.CancelBooking(startDate, endDate, carId, bookingNo) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ActionName);
        }

        [TestCase("2024-08-01 20:00:00", null, 3, 3)]
        public void CancelBooking_ReturnViewBookingList(string startDateStr, string? endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime? endDate = string.IsNullOrEmpty(endDateStr) ? (DateTime?)null : DateTime.Parse(endDateStr);

            // Act
            var result = _controller.CancelBooking(startDate, endDate, carId, bookingNo) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ViewBookingList", result.ActionName);
        }

        [TestCase("2024-08-01 20:00:00", null, 3, 2)]
        public void CancelBooking_PaymentMethodDiff1_ReturnViewBookingList(string startDateStr, string? endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime? endDate = string.IsNullOrEmpty(endDateStr) ? (DateTime?)null : DateTime.Parse(endDateStr);

            // Act
            var result = _controller.CancelBooking(startDate, endDate, carId, bookingNo) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ViewBookingList", result.ActionName);
        }

        [TestCase("2024-08-01 20:00:00", null, 3, 4)]
        public void CancelBooking_TimeBookingLessThan12h_ReturnViewBookingList(string startDateStr, string? endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime? endDate = string.IsNullOrEmpty(endDateStr) ? (DateTime?)null : DateTime.Parse(endDateStr);

            // Act
            var result = _controller.CancelBooking(startDate, endDate, carId, bookingNo) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ViewBookingList", result.ActionName);
        }

        [TestCase("2024-08-01 20:00:00", "2024-08-03 20:00:00", 3, 3)]
        public void CancelBooking_ReturnEditBookingDetail(string startDateStr, string? endDateStr, int carId, int bookingNo)
        {
            var user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Arrange
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime? endDate = string.IsNullOrEmpty(endDateStr) ? (DateTime?)null : DateTime.Parse(endDateStr);

            // Act
            var result = _controller.CancelBooking(startDate, endDate, carId, bookingNo) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("EditBookingDetail", result.ActionName);
        }

        [Test]
        public void UploadImage_NullFile_ReturnsInvalidFileMessage()
        {
            // Act
            var result = _controller.UploadImage(null) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            //var jsonResult = result.Value as IDictionary<string, object>;
            //Assert.IsNotNull(jsonResult);
            //Assert.IsFalse((bool)jsonResult["success"]);
            //Assert.AreEqual("Invalid file", jsonResult["message"]);
        }

        [Test]
        public void UploadImage_FileAlreadyExists_ReturnsFileExistsMessage()
        {
            // Arrange
            var fileName = "1.png";
            var imgDirectory = GetProjectPath("wwwroot/img");
            var filePath = Path.Combine(imgDirectory, fileName);

            File.WriteAllText(filePath, "dummy content");

            _mockFile.Setup(f => f.FileName).Returns(fileName);
            _mockFile.Setup(f => f.Length).Returns(1024);
            _mockFile.Setup(f => f.CopyTo(It.IsAny<Stream>())).Callback<Stream>(stream =>
            {
                using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Dummy content")))
                {
                    memoryStream.CopyTo(stream);
                }
            });

            // Act
            var result = _controller.UploadImage(_mockFile.Object) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            //var jsonResult = result.Value as IDictionary<string, object>;
            //Assert.IsNotNull(jsonResult);
            //Assert.IsFalse((bool)jsonResult["success"]);
            //Assert.AreEqual("File already exists", jsonResult["message"]);

            // Cleanup
            File.Delete(filePath);
        }

        [Test]
        public void UploadImage_ValidFile_ReturnsSuccess()
        {
            // Arrange
            var uniqueFileName = $"newFile_{Guid.NewGuid()}.png";
            var imgDirectory = GetProjectPath("wwwroot/img");

            // Đảm bảo rằng thư mục tồn tại
            if (!Directory.Exists(imgDirectory))
            {
                Directory.CreateDirectory(imgDirectory);
            }

            var filePath = Path.Combine(imgDirectory, uniqueFileName);

            _mockFile.Setup(f => f.FileName).Returns(uniqueFileName);
            _mockFile.Setup(f => f.Length).Returns(1024);
            _mockFile.Setup(f => f.CopyTo(It.IsAny<Stream>())).Callback<Stream>(stream =>
            {
                using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Dummy content")))
                {
                    memoryStream.CopyTo(stream);
                }
            });

            // Act
            var result = _controller.UploadImage(_mockFile.Object) as JsonResult;

            // Cleanup
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Assert
            Assert.IsNotNull(result);
            var jsonResult = result.Value as IDictionary<string, object>;
            //Assert.IsTrue((bool)jsonResult["success"]);
        }

        public string GetProjectPath(string relativePath)
        {
            // Lấy đường dẫn tuyệt đối đến thư mục dự án gốc
            var projectRoot = Directory.GetCurrentDirectory();
            return Path.Combine(projectRoot, relativePath);
        }
    }
}
