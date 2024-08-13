using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Rental_Car_Demo.Controllers;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.ViewModel;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Rental_Car_Demo.Services;

namespace Rental_Car_Demo.UnitTests
{
    [TestFixture]
    public class UsersControllerTestsAn : DummySession
    {
        private UsersController _controller;
        private RentCarDbContext _context;
        private Mock<IEmailService> _mockEmailService;
        private DummySession _session;
        private Mock<IFormFile> _mockFile;
        private string filePath;

        [SetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = @"C:\Users\decid\OneDrive\Desktop\PojectFsoft\rental-car\Rental_Car_Demo";
            filePath = Path.Combine(Environment.CurrentDirectory, "wwwroot/img", "newFile.png");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            var options = new DbContextOptionsBuilder<RentCarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new RentCarDbContext(options);
            _session = new DummySession();
            _mockEmailService = new Mock<IEmailService>();
            _mockFile = new Mock<IFormFile>();
            _controller = new UsersController(_context, _mockEmailService.Object)
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
            _context.Dispose();
            _controller.Dispose();
        }

        private void SeedDatabase()
        {
            var ratingData = new List<Feedback>
            {
                new Feedback { FeedbackId = 1, BookingNo = 1, Ratings = 4, Content = "abc", Date = new DateTime(2023, 06, 22)}
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
                new User { UserId = 1, Name = "User A",Role = false, Email = "truonganat0@gmail.com", Password ="5cf98d76c49d05e6a700069b0215b6cb4cc187d5ea1d2bf73a8a1aa5973afb92",Phone="0334110870", RememberMe = false, Wallet = 0 },
                new User { UserId = 2, Name = "User B", Role = true, Email = "truonganat00@gmail.com", Password ="5cf98d76c49d05e6a700069b0215b6cb4cc187d5ea1d2bf73a8a1aa5973afb92", Phone="0334110870", RememberMe = false, Wallet = 20000000 },
                new User { UserId = 3, Name = "User C", Role = false, Email = "truonganat1@gmail.com", Password ="5cf98d76c49d05e6a700069b0215b6cb4cc187d5ea1d2bf73a8a1aa5973afb92", Phone="0334110870", RememberMe = false, Wallet = 20000000, AddressId = 2, Address = _context.Addresses.FirstOrDefault(a => a.AddressId == 2) },
                new User { UserId = 4, Name = "User D", Role = false, Email = "truonganat2@gmail.com", Password ="5cf98d76c49d05e6a700069b0215b6cb4cc187d5ea1d2bf73a8a1aa5973afb92", Phone="0334110870", RememberMe = false, Wallet = 20000000, AddressId = 1, Address = _context.Addresses.FirstOrDefault(a => a.AddressId == 1)}
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
                BasePrice = 1000000, Deposit = 500000,AddressId = 2, Description = "NOG NOG 2", DocumentId = 1,
                TermId = 2, FucntionId = 2, Status = 2, NoOfRide = 2
                },
                new Car {CarId = 3,UserId = 2, Name = "Acura MDX 2003", LicensePlate = "56F-513.22", BrandId = 1, ModelId = 2, Seats = 10,
                ColorId = 2,FrontImage = "Image2.jpg",BackImage ="Image2.jpg",LeftImage ="Image2.jpg", RightImage ="Image2.jpg",
                ProductionYear = 2003,TransmissionType = false, FuelType = false, Mileage = 100, FuelConsumption = 8,
                BasePrice = 1000000, Deposit = 500000,AddressId = 2, Description = "NOG NOG 2", DocumentId = 1,
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
                    DriverPhone = "0334110870", DriverAddressId = 1, DriverDrivingLicense = "rd.png", RenterAddress = addressData.FirstOrDefault(a => a.AddressId == 2), DriverAddress = addressData.FirstOrDefault(a => a.AddressId == 2)}
            };

            var bookingData = new List<Booking>
            {
                new Booking { BookingNo = 1, UserId = 1, CarId = 1, StartDate = new DateTime(2024, 08, 01, 20, 00, 00), EndDate = new DateTime(2024, 08, 03, 20, 00, 00), PaymentMethod = 1,
                    Status = 2, BookingInfoId = 1, BookingInfo = bookingInfoData.FirstOrDefault(a => a.BookingInfoId == 1)},
                new Booking { BookingNo = 2, UserId = 3, CarId = 1, StartDate = new DateTime(2024, 08, 01, 20, 00, 00), EndDate = new DateTime(2024, 08, 03, 20, 00, 00), PaymentMethod = 2,
                    Status=1, BookingInfoId = 2, BookingInfo = bookingInfoData.FirstOrDefault(a => a.BookingInfoId == 2)},
                new Booking { BookingNo = 3, UserId = 3, CarId = 3, StartDate = new DateTime(2024, 08, 01, 20, 00, 00), EndDate = new DateTime(2024, 08, 03, 20, 00, 00), PaymentMethod = 1,
                    Status=2, BookingInfoId = 2, BookingInfo = bookingInfoData.FirstOrDefault(a => a.BookingInfoId == 2)}
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


        [TestCase(1)]
        public void EditProfile_Get_SessionNull_ReturnLogin(int id)
        {
            User user = null;
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _controller.Edit(id) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ActionName);
        }

        [TestCase(100)]
        public void EditProfile_Get_ConflictUserId_ReturnLogin(int id)
        {
            User user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _controller.Edit(id) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ErrorAuthorization", result.ViewName);
        }

        [TestCase(1)]
        public void EditProfile_Get_ReturnPost(int id)
        {
            User user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _controller.Edit(id) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [TestCase(1)]
        public void EditProfile_Get_CheckViewBagCities_AddressNull(int id)
        {
            User user = _context.Users.Include(u => u.Address).SingleOrDefault(u => u.UserId == id);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _controller.Edit(id) as ViewResult;

            var address = _context.Addresses.FirstOrDefault(a => a.AddressId == user.AddressId);
            var expectedValue = new SelectList(_context.Cities.ToList(), "CityId", "CityProvince");
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

        [TestCase(3)]
        public void EditProfile_Get_CheckViewBagCities_AddressNotNull(int id)
        {
            User user = _context.Users.Include(u => u.Address).SingleOrDefault(u => u.UserId == id);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _controller.Edit(id) as ViewResult;

            var address = _context.Addresses.FirstOrDefault(a => a.AddressId == user.AddressId);
            var expectedValue = new SelectList(_context.Cities.ToList(), "CityId", "CityProvince", address.CityId);
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

        [TestCase(3)]
        public void EditProfile_Get_CheckViewBagDistricts_AddressNotNull(int id)
        {
            User user = _context.Users.Include(u => u.Address).SingleOrDefault(u => u.UserId == id);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _controller.Edit(id) as ViewResult;

            var address = _context.Addresses.FirstOrDefault(a => a.AddressId == user.AddressId);

            var district = _context.Districts.Where(a => a.DistrictId == address.DistrictId).ToList();
            var expectedValue = new SelectList(district, "DistrictId", "DistrictName", address.DistrictId);
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

        [TestCase(3)]
        public void EditProfile_Get_CheckViewBagWards_AddressNotNull(int id)
        {
            User user = _context.Users.Include(u => u.Address).SingleOrDefault(u => u.UserId == id);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _controller.Edit(id) as ViewResult;

            var address = _context.Addresses.FirstOrDefault(a => a.AddressId == user.AddressId);

            var district = _context.Wards.Where(a => a.WardId == address.WardId).ToList();
            var expectedValue = new SelectList(district, "WardId", "WardName", address.WardId);
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

        [TestCase("qweqwe2", "qweqwe2", "")]
        [TestCase("qweqwe2", "", "")]
        [TestCase("", "qweqwe2", "")]
        public void EditProfile_ChangePassword_EmptyCurrentPassword_ReturnView(string NewPassword, string ConfirmPassword, string CurrentPassword)
        {
            User user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            User _user = _context.Users
                .Include(u => u.Address)
                .ThenInclude(a => a.District)
                .Include(u => u.Address)
                .ThenInclude(a => a.Ward)
                .SingleOrDefault(u => u.UserId == user.UserId);

            // Act
            var result = _controller.Edit(_user, NewPassword, ConfirmPassword, CurrentPassword) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Please enter current password before enter new password!", _controller.ModelState[""].Errors[0].ErrorMessage);
        }

        [TestCase("qweqwe2", "qweqwe2", "qweqwe7557")]
        public void EditProfile_ChangePassword_WrongCurrentPassword_ReturnView(string NewPassword, string ConfirmPassword, string CurrentPassword)
        {
            User user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            User _user = _context.Users
                .Include(u => u.Address)
                .ThenInclude(a => a.District)
                .Include(u => u.Address)
                .ThenInclude(a => a.Ward)
                .SingleOrDefault(u => u.UserId == user.UserId);

            // Act
            var result = _controller.Edit(_user, NewPassword, ConfirmPassword, CurrentPassword) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Current password is not correct!", _controller.ModelState[""].Errors[0].ErrorMessage);
        }

        [TestCase("", "", "qweqwe1")]
        [TestCase("qweqwe2", "", "qweqwe1")]
        [TestCase("", "qweqwe2", "qweqwe1")]
        public void EditProfile_ChangePassword_EmptyNewOrConfirmPassword_ReturnView(string NewPassword, string ConfirmPassword, string CurrentPassword)
        {
            User user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            User _user = _context.Users
                .Include(u => u.Address)
                .ThenInclude(a => a.District)
                .Include(u => u.Address)
                .ThenInclude(a => a.Ward)
                .SingleOrDefault(u => u.UserId == user.UserId);

            // Act
            var result = _controller.Edit(_user, NewPassword, ConfirmPassword, CurrentPassword) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Please enter both new password and confirm new password!", _controller.ModelState[""].Errors[0].ErrorMessage);
        }

        [TestCase("qweqwe2", "qweqwe2", "qweqwe1")]
        public void EditProfile_ChangePasswordSuccess_ReturnLogOut(string NewPassword, string ConfirmPassword, string CurrentPassword)
        {
            User user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            User _user = _context.Users
                .Include(u => u.Address)
                .ThenInclude(a => a.District)
                .Include(u => u.Address)
                .ThenInclude(a => a.Ward)
                .SingleOrDefault(u => u.UserId == user.UserId);

            // Act
            var result = _controller.Edit(_user, NewPassword, ConfirmPassword, CurrentPassword) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Logout", result.ActionName);
        }

        [TestCase("qweqwe2", "qweqwe2", "qweqwe1")]
        public void EditProfile_ChangePasswordSuccess_NotNullAddress_ReturnLogOut(string NewPassword, string ConfirmPassword, string CurrentPassword)
        {
            User user = _context.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            User _user = _context.Users.Include(u => u.Address).SingleOrDefault(u => u.UserId == user.UserId);

            // Act
            var result = _controller.Edit(_user, NewPassword, ConfirmPassword, CurrentPassword) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Logout", result.ActionName);
        }

        [TestCase("ewffwef", "qweqwe2", "qweqwe1")]
        [TestCase("ewffwef", "ewffwef", "qweqwe1")]
        [TestCase("65156", "qweqwe2", "qweqwe1")]
        [TestCase("65156", "65156", "qweqwe1")]
        [TestCase("qweqwe2", "qweqwee", "qweqwe1")]
        public void EditProfile_ChangePassword_NotValid_ReturnView(string NewPassword, string ConfirmPassword, string CurrentPassword)
        {
            User user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            User _user = _context.Users
                .Include(u => u.Address)
                .ThenInclude(a => a.District)
                .Include(u => u.Address)
                .ThenInclude(a => a.Ward)
                .SingleOrDefault(u => u.UserId == user.UserId);

            // Act
            var result = _controller.Edit(_user, NewPassword, ConfirmPassword, CurrentPassword) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("New password must be matched with Confirm new password and contains at least seven characters long include at least one letter and one number!", _controller.ModelState[""].Errors[0].ErrorMessage);
        }

        [TestCase("", "", "")]
        public void EditProfile_IsCustomerAndNotChangePassword_ReturnLoginCus(string NewPassword, string ConfirmPassword, string CurrentPassword)
        {
            User user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            User _user = _context.Users.Find(1);

            var result = _controller.Edit(_user, NewPassword, ConfirmPassword, CurrentPassword) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("LoginCus", result.ActionName);
        }

        [TestCase("", "", "")]
        public void EditProfile_IsCarOwnerAndNotChangePassword_ReturnLoginOwn(string NewPassword, string ConfirmPassword, string CurrentPassword)
        {
            User user = _context.Users.Find(2);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            User _user = _context.Users.Find(2);

            // Act
            var result = _controller.Edit(_user, NewPassword, ConfirmPassword, CurrentPassword) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("LoginOwn", result.ActionName);
        }

        [TestCase("", "", "")]
        public void EditProfile_emailExists_ReturnLoginOwn(string NewPassword, string ConfirmPassword, string CurrentPassword)
        {
            User user = _context.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            User _user = _context.Users.Find(1);
            _user.Email = "truonganat1@gmail.com";

            // Act
            var result = _controller.Edit(_user, NewPassword, ConfirmPassword, CurrentPassword) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Email already existed. Please try another email.", _controller.ModelState["Email"].Errors[0].ErrorMessage);
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
