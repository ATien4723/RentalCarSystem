using NUnit.Framework;
using Moq;
using Microsoft.EntityFrameworkCore;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.CarRepository;
using System.Collections.Generic;
using System.Linq;

namespace Rental_Car_Demo.Tests
{
    [TestFixture]
    public class CarDAOTests
    {
        private CarDAO _carDao;
        private Mock<RentCarDbContext> _mockContext;
        private Mock<DbSet<Car>> _mockCarSet;

        [SetUp]
        public void SetUp()
        {
            _mockContext = new Mock<RentCarDbContext>();
            _mockCarSet = new Mock<DbSet<Car>>();

            var carData = new List<Car>
            {
                new Car { CarId = 1, Brand = new CarBrand { BrandName = "Toyota" }, Seats = 4, TransmissionType = true, BasePrice = 100, Status = 1, Address = new Address { HouseNumberStreet = "123 Main St", Ward = new Ward { WardName = "Ward1" }, District = new District { DistrictName = "District1" }, City = new City { CityProvince = "City1" } }},
                new Car { CarId = 2, Brand = new CarBrand { BrandName = "Honda" }, Seats = 5, TransmissionType = false, BasePrice = 150, Status = 1, Address = new Address { HouseNumberStreet = "456 Main St", Ward = new Ward { WardName = "Ward2" }, District = new District { DistrictName = "District2" }, City = new City { CityProvince = "City2" } }}
            }.AsQueryable();

            _mockCarSet.As<IQueryable<Car>>().Setup(m => m.Provider).Returns(carData.Provider);
            _mockCarSet.As<IQueryable<Car>>().Setup(m => m.Expression).Returns(carData.Expression);
            _mockCarSet.As<IQueryable<Car>>().Setup(m => m.ElementType).Returns(carData.ElementType);
            _mockCarSet.As<IQueryable<Car>>().Setup(m => m.GetEnumerator()).Returns(carData.GetEnumerator());

            _mockContext.Setup(c => c.Cars).Returns(_mockCarSet.Object);

            _carDao = CarDAO.Instance;
        }

        [Test]
        public void CreateCar_ShouldAddCar()
        {
            var newCar = new Car { CarId = 3, Brand = new CarBrand { BrandName = "Ford" }, Seats = 5, TransmissionType = true, BasePrice = 200, Status = 1, Address = new Address { HouseNumberStreet = "789 Main St", Ward = new Ward { WardName = "Ward3" }, District = new District { DistrictName = "District3" }, City = new City { CityProvince = "City3" } } };

            _carDao.CreateCar(newCar);

            _mockCarSet.Verify(m => m.Add(It.IsAny<Car>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Test]
        public void GetAllCars_ShouldReturnAllCars()
        {
            var cars = _carDao.GetAllCars();

            Assert.AreEqual(2, cars.Count());
        }

        [Test]
        public void GetAllCars_WithAddress_ShouldReturnFilteredCars()
        {
            var address = "123 Main St";
            var cars = _carDao.GetAllCars(address);

            Assert.AreEqual(1, cars.Count());
            Assert.AreEqual("Toyota", cars.First().Brand.BrandName);
        }

        [Test]
        public void SearchCars_ShouldReturnFilteredCars()
        {
            var cars = _carDao.SearchCars("Toyota", 4, true, null, 50, 150, null);

            Assert.AreEqual(1, cars.Count());
            Assert.AreEqual("Toyota", cars.First().Brand.BrandName);
        }
    }
}
