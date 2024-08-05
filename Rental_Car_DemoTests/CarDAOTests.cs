//using Microsoft.EntityFrameworkCore;
//using Rental_Car_Demo.Models;
//using Rental_Car_Demo.Repository.CarRepository;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Rental_Car_Demo.UnitTests
//{
//    [TestFixture]
//    public class CarDaoTests
//    {
//        private RentCarDbContext _context;
//        private CarDAO _carDao; // Assuming you have a DAO class for Car with the GetAllCars method

//        [SetUp]
//        public void SetUp()
//        {
//            var options = new DbContextOptionsBuilder<RentCarDbContext>()
//                .UseInMemoryDatabase(databaseName: "TestDatabase")
//                .Options;

//            _context = new RentCarDbContext(options);
//            _carDao = new CarDAO();

//            SeedDatabase();
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _context.Database.EnsureDeleted();
//            _context.Dispose();
//        }

//        private void SeedDatabase()
//        {
//            var city = new City { CityProvince = "SampleCity" };
//            var district = new District { DistrictName = "SampleDistrict" };
//            var ward = new Ward { WardName = "SampleWard" };
//            var address = new Address
//            {
//                HouseNumberStreet = "123 Main St",
//                City = city,
//                District = district,
//                Ward = ward
//            };
//            var cars = new List<Car>
//            {
//               new Car
//               {
//                   CarId = 1,
//                   Address = new Address
//                   {
//                        HouseNumberStreet = "123 Main St",
//                        Ward = new Ward { WardName = "Ward A" },
//                        District = new District { DistrictName = "District B" },
//                        City = new City { CityProvince = "City C" }
//                   },
//                   Status = 1,
//                   BackImage = "backImageUrl",
//                   Description = "Description",
//                   FrontImage = "frontImageUrl",
//                   LeftImage = "leftImageUrl",
//                   LicensePlate = "ABC123",
//                   Name = "Car Name",
//                   RightImage = "rightImageUrl"
//            },
//               new Car
//               {
//                   CarId = 2,
//                   Address = new Address
//                   {
//                       HouseNumberStreet = "456 Another St",
//                       Ward = new Ward { WardName = "Ward D" },
//                       District = new District { DistrictName = "District E" },
//                       City = new City { CityProvince = "City F" }
//                   },
//                   Status = 1,
//                   BackImage = "backImageUrl",
//                   Description = "Description",
//                   FrontImage = "frontImageUrl",
//                   LeftImage = "leftImageUrl",
//                   LicensePlate = "DEF456",
//                   Name = "Another Car Name",
//                   RightImage = "rightImageUrl"
//               }
//            };

//            _context.Cars.AddRange(cars);
//            _context.SaveChanges();
//        }

//        [Test]
//        public void GetAllCars_WithAddress_ReturnsFilteredCars()
//        {
//            // Arrange
//            string address = "Main St";

//            // Act
//            var cars = _carDao.GetAllCars(address);

//            // Assert
//            Assert.IsNotNull(cars);
//            Assert.IsTrue(cars.Any(), "There should be at least one car in the result.");
//            Assert.IsTrue(cars.All(c => (c.Address.HouseNumberStreet + ", " +
//                                         c.Address.Ward.WardName + ", " +
//                                         c.Address.District.DistrictName + ", " +
//                                         c.Address.City.CityProvince).Contains(address)),
//                            "All cars should match the address filter.");
//        }

//        [Test]
//        public void GetAllCars_WithoutAddress_ReturnsAllCars()
//        {
//            // Act
//            var cars = _carDao.GetAllCars(null);

//            // Assert
//            Assert.IsNotNull(cars);
//            Assert.IsTrue(cars.Any(), "There should be at least one car in the result.");
//        }

//        [Test]
//        public void GetAllCars_NoMatchingAddress_ReturnsEmpty()
//        {
//            // Arrange
//            string address = "NonexistentAddress";

//            // Act
//            var cars = _carDao.GetAllCars(address);

//            // Assert
//            Assert.IsNotNull(cars);
//            Assert.IsFalse(cars.Any(), "There should be no cars matching the non-existent address.");
//        }
//    }

//}
