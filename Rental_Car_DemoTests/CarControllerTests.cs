using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using Rental_Car_Demo.Context;
using Rental_Car_Demo.Controllers;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.CarRepository;
using Rental_Car_Demo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis;
using Org.BouncyCastle.Tls;

namespace Rental_Car_Demo.UnitTests
{
    [TestFixture]
    public class CarControllerTests
    {
        private CarController _controller;
        private RentCarDbContext _context;
        private Mock<IEmailService> _mockEmailService;
        private Mock<ITempDataDictionary> _mockTempData;
        private Mock<ICarRepository> _mockCarRepository;


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

            _controller = new CarController(_mockCarRepository.Object,_context, _mockEmailService.Object)
            {
                TempData = _mockTempData.Object
            };

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
                    Email = "nvutuankiet2003@gmail.com",
                    Password = HashPassword("kiet123"),
                    Name = "kiet ne",
                    Phone = "0334567890",
                    Role = false,
                    Wallet = 0
                },
                new User
                {
                    Email = "hehe@gmail.com",
                    Password = HashPassword("hehe123"),
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
                    Status = 2,
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


    }
}
