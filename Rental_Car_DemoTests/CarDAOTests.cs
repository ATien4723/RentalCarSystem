using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.CarRepository;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var options = new DbContextOptionsBuilder<RentCarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new RentCarDbContext(options);
            _carDAO = new CarDAO();
            SeedDatabase(_context);
        }

        private void SeedDatabase(RentCarDbContext context)
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
                new CarBrand { BrandId = 1, BrandName = "Brand A", BrandLogo = "LogoA"},
                new CarBrand { BrandId = 2, BrandName = "Brand B", BrandLogo = "LogoB" }
            };

            var cars = new List<Car>
            {
                new Car
                {
                    CarId = 1,
                    ModelId = 1,
                    BrandId = 1,
                    AddressId = 1,
                    BackImage = "back1.jpg",
                    Description = "Description 1",
                    FrontImage = "front1.jpg",
                    LeftImage = "left1.jpg",
                    LicensePlate = "ABC123",
                    Name = "Car 1",
                    RightImage = "right1.jpg"
                },
                new Car
                {
                    CarId = 2,
                    ModelId = 2,
                    BrandId = 2,
                    AddressId = 2,
                    BackImage = "back2.jpg",
                    Description = "Description 2",
                    FrontImage = "front2.jpg",
                    LeftImage = "left2.jpg",
                    LicensePlate = "XYZ789",
                    Name = "Car 2",
                    RightImage = "right2.jpg"
                }
            };

            context.Cities.AddRange(cityData);
            context.Districts.AddRange(districtData);
            context.Wards.AddRange(wardData);
            context.Addresses.AddRange(addressData);
            context.CarModels.AddRange(carModels);
            context.CarBrands.AddRange(carBrands);
            context.Cars.AddRange(cars);
            context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestCase("Brand A", null, null, null, null, null, null, null, 1, "Car 1")]
        [TestCase(null, null, null, null, "LogoB", null, null, null, 1, "Car 2")]
        [TestCase(null, null, null, null, "LogoA", 0, 100000, "", 1, "Car 1")]
        [TestCase("", null, null, null, "LogoB", 0, 10000, "", 0, "")]
        [TestCase("Nonexistent Brand", null, null, null, "LogoA", null, null, "", 0, "")]
        public void SearchCars_ShouldReturnExpectedCars(
        string? brandName,
        int? seats,
        bool? transmissionType,
        bool? fuelType,
        string? brandLogo,
        decimal? minPrice,
        decimal? maxPrice,
        string? address,
        int expectedCount,
        string expectedCarName)
        {
            var cars = _carDAO.SearchCars(brandName, seats, transmissionType, fuelType, brandLogo, minPrice, maxPrice, address);
            Assert.AreEqual(expectedCount, cars.Count());

            if (expectedCount > 0)
            {
                Assert.AreEqual(expectedCarName, cars.First().Name);
            }
        }
    }

}
