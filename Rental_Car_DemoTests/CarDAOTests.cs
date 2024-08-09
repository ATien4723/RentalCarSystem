using Microsoft.EntityFrameworkCore;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.CarRepository;
using System.Text;
using System.Security.Cryptography;

namespace Rental_Car_Demo.Tests
{
    [TestFixture]
    public class CarDAOTests
    {
        private RentCarDbContext _context;
        private CarDAO _carDAO;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<RentCarDbContext> ()
                .UseInMemoryDatabase (databaseName: "TestDatabase")
                .Options;

            _context = new RentCarDbContext (options);

            _carDAO = new CarDAO (_context);

            SeedDatabase ();
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
                new Address { AddressId = 5, CityId = 4, DistrictId = 4, WardId = 4, HouseNumberStreet = "Nha so 5"},
                new Address { AddressId = 6, CityId = 4, DistrictId = 4, WardId = 4, HouseNumberStreet = "Nha so 6"},
                new Address { AddressId = 7, CityId = 4, DistrictId = 4, WardId = 4, HouseNumberStreet = "Nha so 7"},
                new Address { AddressId = 8, CityId = 4, DistrictId = 4, WardId = 4, HouseNumberStreet = "Nha so 8"},
                new Address { AddressId = 9, CityId = 4, DistrictId = 4, WardId = 4, HouseNumberStreet = "Nha so 9"},
                new Address { AddressId = 10, CityId = 4, DistrictId = 4, WardId = 4, HouseNumberStreet = "Nha so 10"},
                new Address { AddressId = 11, CityId = 4, DistrictId = 4, WardId = 4, HouseNumberStreet = "Nha so 11"},
                new Address { AddressId = 12, CityId = 4, DistrictId = 4, WardId = 4, HouseNumberStreet = "Nha so 12"},

            };

            var carModels = new List<CarModel>
            {
                new CarModel { ModelId = 1, ModelName = "Model X" },
                new CarModel { ModelId = 2, ModelName = "Model Y" },
                new CarModel { ModelId = 3, ModelName = "Model Z" }
            };


            var carBrands = new List<CarBrand>
            {
                new CarBrand { BrandId = 1, BrandName = "Brand A" },
                new CarBrand { BrandId = 2, BrandName = "Brand B" },
                new CarBrand { BrandId = 3, BrandName = "Brand C" }

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
                    Email = "tiendz@gmail.com",
                    Name = "tien dz",
                    Password = "tien123",
                    Phone = "0999999999",
                    Role = false,
                    Wallet = 0
                },
                new User
                {
                    Email = "hehe@gmail.com",
                    Name = "hehe",
                    Password = "tien123",
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
                    BasePrice = 50000,
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
                    BasePrice = 500000,
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
                    LicensePlate = "50F-567.91",
                    BrandId = 2,
                    ModelId = 2,
                    Seats = 4,
                    FrontImage = "front3.jpg",
                    BackImage = "back3.jpg",
                    LeftImage = "left3.jpg",
                    RightImage = "right3.jpg",
                    ProductionYear = 2024,
                    TransmissionType = true,  // true la auto
                    FuelType = false, // true la xang
                    Mileage = 30000,
                    FuelConsumption = 200,
                    BasePrice = 5000000,
                    Deposit = 3000000,
                    ColorId = 2,
                    AddressId = 3,
                    Description = "Description 3",
                    DocumentId = 2,
                    TermId = 2,
                    FucntionId = 2,
                    Status = 1,
                    NoOfRide = 2
                },
                  new Car
                {
                    CarId = 4,
                    UserId = 1,
                    Name = "Car 4",
                    LicensePlate = "50F-567.92",
                    BrandId = 2,
                    ModelId = 2,
                    Seats = 4,
                    FrontImage = "front4.jpg",
                    BackImage = "back4.jpg",
                    LeftImage = "left4.jpg",
                    RightImage = "right4.jpg",
                    ProductionYear = 2024,
                    TransmissionType = true,
                    FuelType = false,
                    Mileage = 30000,
                    FuelConsumption = 200,
                    BasePrice = 100000000,
                    Deposit = 3000000,
                    ColorId = 2,
                    AddressId = 4,
                    Description = "Description 4",
                    DocumentId = 2,
                    TermId = 2,
                    FucntionId = 2,
                    Status = 1,
                    NoOfRide = 2
                },

                   new Car
                {
                    CarId = 5,
                    UserId = 1,
                    Name = "Car 5",
                    LicensePlate = "50F-567.93",
                    BrandId = 3,
                    ModelId = 2,
                    Seats = 5,
                    FrontImage = "front5.jpg",
                    BackImage = "back5.jpg",
                    LeftImage = "left5.jpg",
                    RightImage = "right5.jpg",
                    ProductionYear = 2024,
                    TransmissionType = false,
                    FuelType = false,
                    Mileage = 30000,
                    FuelConsumption = 200,
                    BasePrice = 100000000,
                    Deposit = 3000000,
                    ColorId = 2,
                    AddressId = 4,
                    Description = "Description 5",
                    DocumentId = 2,
                    TermId = 2,
                    FucntionId = 2,
                    Status = 1,
                    NoOfRide = 2
                }
            };

            _context.Users.AddRange (users);
            _context.Cities.AddRange (cityData);
            _context.Districts.AddRange (districtData);
            _context.Wards.AddRange (wardData);
            _context.Addresses.AddRange (addressData);
            _context.CarModels.AddRange (carModels);
            _context.CarBrands.AddRange (carBrands);
            _context.CarColors.AddRange (color);
            _context.AdditionalFunctions.AddRange (additionalFunction);
            _context.CarDocuments.AddRange (carDocument);
            _context.TermOfUses.AddRange (TermOfUse);
            _context.Cars.AddRange (cars);

            _context.SaveChanges ();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted ();
            _context.Dispose ();

        }





        [Test]

        [TestCase (null, null, null, null, null, null, null, 5)] // search bthg k giatri
        [TestCase(new string[] { "Brand A" }, null, null, null, null, null, null, 1)] //search car brandname
        [TestCase (new string[] { "Brand A" , "Brand B" }, null, null, null, null, null, null, 4)] //search car nhieu brandname
        [TestCase(null, new int[] { 4 }, null, null, null, null, null, 3)] // chon mot 1 loai cho
        [TestCase (null, new int[] { 4 , 5 }, null, null, null, null, null, 4)] // search nhieu hon 1 loai cho
        [TestCase(null, null, new bool[] { true }, null, null, null, null, 3)] // xe tu dong
        [TestCase (null, null, new bool[] { false }, null, null, null, null, 2)]// xe so thuong
        [TestCase (null, null, new bool[] { false , true }, null, null, null, null, 5)] // ca tu dong ca thuong
        [TestCase(null, null, null, new bool[] { true }, null, null, null, 1)] // xe xang
        [TestCase (null, null, null, new bool[] { false }, null, null, null, 4)] //xe dau
        [TestCase (null, null, null, new bool[] { true , false }, null, null, null, 5)] // ca xang va dau
        [TestCase (null, new int[] { 4 }, null, new bool[] { false }, null, null, null, 3)] // seats + dau
        [TestCase (null, new int[] { 4 }, null, new bool[] { true }, null, null, null, 0)] // seats + xang
        [TestCase (null, new int[] { 4 }, new bool[] { false }, null, null, null, null, 1)] // seats + xe so thuong
        [TestCase (null, new int[] { 4 }, new bool[] { true }, null, null, null, null, 2)] // seats + xe tu dong
        [TestCase (new string[] { "Brand A" }, new int[] { 4 }, null, null, null, null, null, 0)] //search car brandname + cho ngoi
        [TestCase (new string[] { "Brand B" }, new int[] { 4 , 5 }, null, null, null, null, null, 3)] //search car brandname + nhieu cho ngoi
        [TestCase (new string[] { "Brand A" }, null, new bool[] { true }, null, null, null, null, 1)] // search car brandname + xe so tu dong
        [TestCase (new string[] { "Brand A" }, null, new bool[] { false }, null, null, null, null, 0)] // search car brandname + xe so thuong
        [TestCase (new string[] { "Brand A" }, null, new bool[] { true , false }, null, null, null, null, 1)] // search car brandname + xe so tu dong , so thuong
        [TestCase (new string[] { "Brand B" }, null, null, new bool[] { true }, null, null, null, 0)] //search car brandname + xang
        [TestCase (new string[] { "Brand B" }, null, null, new bool[] { false }, null, null, null, 3)] //search car brandname + dau
        [TestCase (new string[] { "Brand B" }, null, null, new bool[] { true , false }, null, null, null, 3)] //search car brandname + xang , dau
        [TestCase (new string[] { "Brand A" }, new int[] { 4 }, null, null, null, null, null, 0)] //search car brandname + seats
        [TestCase (new string[] { "Brand A", "Brand B" }, new int[] { 4 }, null, null, null, null, null, 3)] //search car nhieu brandname + seats 
        [TestCase (new string[] { "Brand B" }, new int[] { 4 , 5 }, null, null, null, null, null, 3)] //search car brandname + nhieu seats 
        [TestCase (new string[] { "Brand A", "Brand B" }, new int[] { 4 , 5 }, null, null, null, null, null, 3)] //search car nhieu brandname + nhieu seats 
        [TestCase (new string[] { "Brand A" }, new int[] { 4 }, new bool[] { false }, null, null, null, null, 0)] //search car brandname + seats + so thuong
        [TestCase (new string[] { "Brand B" }, new int[] { 4 }, new bool[] { true }, null, null, null, null, 2)] //search car brandname + seats + so tu dong
        [TestCase (new string[] { "Brand A", "Brand B" }, new int[] { 4 }, new bool[] { true , false }, null, null, null, null, 3)] //search car brandname + seats + so tu dong , thuong
        [TestCase (new string[] { "Brand A", "Brand B" }, new int[] { 4 , 5 }, new bool[] { false }, null, null, null, null, 1)] //search car nhieu brandname + nhieu seats + so thuong
        [TestCase (new string[] { "Brand A", "Brand B" }, new int[] { 4 , 5 }, new bool[] { true }, null, null, null, null, 2)] //search car nhieu brandname + nhieu seats + so tu dong
        [TestCase (new string[] { "Brand A", "Brand B" }, new int[] { 4 , 5 }, new bool[] { true, false }, null, null, null, null, 3)] //search car nhieu brandname + nhieu seats + so thuong , tu dong
        [TestCase (new string[] { "Brand A"}, new int[] { 4, 5 }, new bool[] { false }, null, null, null, null, 0)] //search car brandname + nhieu seats + so thuong
        [TestCase (new string[] { "Brand A"}, new int[] { 4, 5 }, new bool[] { true }, null, null, null, null, 0)] //search car brandname + nhieu seats + so tu dong
        [TestCase (new string[] { "Brand A" }, new int[] { 4, 5 }, new bool[] { true , false }, null, null, null, null, 0)] //search car brandname + nhieu seats + so tu dong , thuong
        [TestCase (new string[] { "Brand A" }, new int[] { 4 }, null, new bool[] { true }, null, null, null, 0)] //search car brandname + seats + xang
        [TestCase (new string[] { "Brand B" }, new int[] { 4 }, null, new bool[] { false }, null, null, null, 3)] //search car brandname + seats + dau
        [TestCase (new string[] { "Brand A" }, new int[] { 4 }, null, new bool[] { true , false }, null, null, null, 0)] //search car brandname + seats + xang , dau
        [TestCase (new string[] { "Brand B" , "Brand A" }, new int[] { 4 }, null, new bool[] { true }, null, null, null, 0)] //search car nhieu brandname + seats + xang
        [TestCase (new string[] { "Brand B" , "Brand A" }, new int[] { 4 }, null, new bool[] { false }, null, null, null, 3)] //search car nhieu brandname + seats + dau
        [TestCase (new string[] { "Brand B" , "Brand A" }, new int[] { 4 }, null, new bool[] { true , false }, null, null, null, 3)] //search car nhieu brandname + seats + xang , dau
        [TestCase (new string[] { "Brand A" }, new int[] { 4 , 5 }, null, new bool[] { true }, null, null, null, 0)] //search car brandname + nhieu seats + xang
        [TestCase (new string[] { "Brand B" }, new int[] { 4 , 5 }, null, new bool[] { false }, null, null, null, 3)] //search car brandname + nhieu seats + dau
        [TestCase (new string[] { "Brand A" }, new int[] { 4 , 5 }, null, new bool[] { true, false }, null, null, null, 0)] //search car brandname + nhieu seats + xang , dau
        [TestCase (new string[] { "Brand A , Brand B" }, new int[] { 4, 5 }, null, new bool[] { true }, null, null, null, 0)] //search nhieu car brandname + nhieu seats + xang 
        [TestCase (new string[] { "Brand A , Brand B" }, new int[] { 4, 5 }, null, new bool[] { false }, null, null, null, 0)] //search nhieu car brandname + nhieu seats + dau
        [TestCase (new string[] { "Brand A , Brand B" }, new int[] { 4, 5 }, null, new bool[] { true , false }, null, null, null, 0)] //search nhieu car brandname + nhieu seats + xang , dau
        [TestCase (new string[] { "Brand A" }, null, null, null, 0, 100000, null, 1)] //search car brandname + price 0->100000
        [TestCase (new string[] { "Brand A" }, null, null, null, 100000, 500000, null, 0)] //search car brandname + price 100000->500000
        [TestCase (new string[] { "Brand A" }, null, null, null, 500000, 1000000, null, 0)] //search car brandname + price 500000->100000
        [TestCase (new string[] { "Brand A" }, null, null, null, 1000000, 5000000, null, 0)] //search car brandname + price 1000000->5000000
        [TestCase (new string[] { "Brand A" }, new int[] { 4 }, null, null, 0, 100000, null, 0)] //search car brandname + seat + price 0->100000
        [TestCase (new string[] { "Brand A" }, new int[] { 4 }, null, null, 100000, 500000, null, 0)] //search car brandname + seat + price 100000->500000
        [TestCase (new string[] { "Brand A" }, new int[] { 4 }, null, null, 500000, 1000000, null, 0)] //search car brandname + seat + price 500000->100000
        [TestCase (new string[] { "Brand A" }, new int[] { 4 }, null, null, 1000000, 5000000, null, 0)] //search car brandname + seat + price 1000000->5000000


        [TestCase (null, null, null, null, 0, 100000, null, 1)]
        [TestCase (null, null, null, null, 100000, 500000, null, 1)]
        [TestCase (null, null, null, null, 500000, 1000000, null, 1)]
        [TestCase (null, null, null, null, 1000000, 5000000, null, 1)]
        [TestCase (null, null, null, null, 5000000, 10000000, null, 1)]
        [TestCase(null, null, null, null, 10000000, 100000000, null, 2)]
        [TestCase(null, null, null, null, null, null, "Nha so 1", 1)]
        public void SearchCars_TestCases(string[]? brandNames, int[]? seats, bool[]? transmissionTypes, bool[]? fuelTypes, decimal? minPrice, decimal? maxPrice, string? address, int? expectedCount)
        {
            // Act
            var result = _carDAO.SearchCars(brandNames, seats, transmissionTypes, fuelTypes, minPrice, maxPrice, address).ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
        }
    }

}
