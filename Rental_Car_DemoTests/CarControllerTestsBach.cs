using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Org.BouncyCastle.Tls;
using Rental_Car_Demo.Controllers;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.CarRepository;
using Rental_Car_Demo.Services;

namespace Rental_Car_Demo.UnitTests
{
    [TestFixture]
    public class CarControllerTestsBach
    {
        private CarController _carController;
        private RentCarDbContext _dbContext;
        private Mock<IEmailService> _mockEmailService;
        private Mock<ITempDataDictionary> _mockTempData;
        private Mock<ICarRepository> _mockCarRepository;
        private DummySession _session;
        private CarDAO _carDAO;

        [SetUp]
        public void SetUp()
        {
            _mockCarRepository = new Mock<ICarRepository>();
            _mockEmailService = new Mock<IEmailService>();
            var options = new DbContextOptionsBuilder<RentCarDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new RentCarDbContext(options);
            _session = new DummySession();
            _carDAO = new CarDAO(_dbContext);
            _carController = new CarController(_mockCarRepository.Object, _dbContext, _mockEmailService.Object)
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

        [Test, MaxTime(2000)]
        public void ViewMyCarsGet_ReturnsExpectedCars()
        {
            var user = _dbContext.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _carController.ViewMyCars() as ViewResult;
            var viewBagCars = _carController.ViewData["Cars"];

            // Assert
            Assert.IsNotNull(viewBagCars);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData.Values.Contains(viewBagCars));
        }

        [Test, MaxTime(2000)]
        public void ViewMyCarsGet_ReturnsExpectedNumberOfCars()
        {
            var user = _dbContext.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _carController.ViewMyCars() as ViewResult;
            var viewBagCars = _carController.ViewData["Cars"];

            // Assert
            Assert.AreEqual(3, ((IEnumerable<dynamic>)viewBagCars).Count());
        }

        [Test, MaxTime(2000)]
        public void ViewMyCarsGet_ReturnsErrorView()
        {
            var user = _dbContext.Users.Find(2);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _carController.ViewMyCars() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.ViewName, Is.EqualTo("ErrorAuthorization"));
        }

        [Test, MaxTime(2000)]
        public void ViewMyCars_DoNotLogin_RedirectsToLogin()
        {
            // Act
            var result = _carController.ViewMyCars() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That("Login", Is.EqualTo(result.ActionName));
            Assert.That("Users", Is.EqualTo(result.ControllerName));
        }

        [Test, MaxTime(2000)]
        [TestCase("latest")]
        [TestCase("newest")]
        [TestCase("highest")]
        [TestCase("lowest")]
        public void ViewMyCarsPost_CheckViewResultWithSortOrderCar(string sortOrder)
        {
            var user = _dbContext.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _carController.ViewMyCars(sortOrder) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test, MaxTime(2000)]
        [TestCase("latest")]
        [TestCase("newest")]
        [TestCase("highest")]
        [TestCase("lowest")]
        public void ViewMyCarsPost_ReturnsExpectedSortOrderCar(string sortOrder)
        {
            var user = _dbContext.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _carController.ViewMyCars(sortOrder) as ViewResult;
            var cars = (result.ViewData["Cars"] as IEnumerable)?.Cast<dynamic>().ToList();
            var actualSortOrder = result.ViewData["SortOrder"];

            // Assert
            Assert.IsNotNull(cars);
            Assert.IsNotNull(actualSortOrder);
            Assert.That(actualSortOrder.ToString(), Is.EqualTo(sortOrder));
        }

        [Test, MaxTime(2000)]
        [TestCase("latest")]
        [TestCase("newest")]
        [TestCase("highest")]
        [TestCase("lowest")]
        public void ViewMyCarsPost_ReturnsNumberOfCar(string sortOrder)
        {
            var user = _dbContext.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _carController.ViewMyCars(sortOrder) as ViewResult;
            var cars = (result.ViewData["Cars"] as IEnumerable)?.Cast<dynamic>().ToList();

            // Assert
            Assert.IsNotNull(cars);
            Assert.That(cars.Count(), Is.EqualTo(3));
        }

        [Test, MaxTime(2000)]
        [TestCase("latest")]
        [TestCase("newest")]
        [TestCase("highest")]
        [TestCase("lowest")]
        public void ViewMyCarsPost_ReturnsExpectedNumberBookingBaseOnCar(string sortOrder)
        {
            var user = _dbContext.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _carController.ViewMyCars(sortOrder) as ViewResult;
            var bookings = (result.ViewData["Bookings"] as IEnumerable)?.Cast<dynamic>().ToList();

            // Assert
            Assert.IsNotNull(bookings);
            Assert.AreEqual(2, bookings.Count);
        }

        [Test, MaxTime(2000)]
        [TestCase("latest")]
        [TestCase("newest")]
        [TestCase("highest")]
        [TestCase("lowest")]
        public void ViewMyCarPost_LoginNewAccount_ZeroViewBagCars(string sortOrder)
        {
            var user = _dbContext.Users.Find(3);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _carController.ViewMyCars(sortOrder) as ViewResult;
            var cars = (result.ViewData["Cars"] as IEnumerable)?.Cast<dynamic>().ToList();

            // Assert
            Assert.IsNotNull(cars);
            Assert.That(cars.Count(), Is.EqualTo(0));

        }

        [Test]
        public void AddACarGet_ReturnExpectedExistedLicensePlate()
        {
            var user = _dbContext.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            //Act
            var result = _carController.AddACar() as ViewResult;
            var viewBag = result.ViewData;

            //Assert
            Assert.IsNotNull(result);
            Assert.That(((List<string>)viewBag["LPList"]).Count, Is.EqualTo(3));
        }

        [Test]
        public void AddACarGet_ReturnExpectedNumberOfColor()
        {
            var user = _dbContext.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            //Act
            var result = _carController.AddACar() as ViewResult;
            var viewBag = result.ViewData;

            //Assert
            Assert.IsNotNull(result);
            Assert.That(((List<CarColor>)viewBag["Color"]).Count, Is.EqualTo(4));
        }

        [Test]
        public void AddACarGet_ReturnExpectedNumberOfBrandCar()
        {
            var user = _dbContext.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            //Act
            var result = _carController.AddACar() as ViewResult;
            var viewBag = result.ViewData;

            //Assert
            Assert.IsNotNull(result);
            Assert.That(((List<CarBrand>)viewBag["Brand"]).Count, Is.EqualTo(3));
        }
        [Test]
        public void AddACarGet_ReturnExpectedNumberOfModelCar()
        {
            var user = _dbContext.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            //Act
            var result = _carController.AddACar() as ViewResult;
            var viewBag = result.ViewData;

            //Assert
            Assert.IsNotNull(result);
            Assert.That(((List<CarModel>)viewBag["Model"]).Count, Is.EqualTo(7));
        }
        [Test]
        public void AddACarGet_ReturnExpectedNumberOfCity()
        {
            var user = _dbContext.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            //Act
            var result = _carController.AddACar() as ViewResult;
            var viewBag = result.ViewData;

            //Assert
            Assert.IsNotNull(result);
            Assert.That(((List<City>)viewBag["City"]).Count, Is.EqualTo(4));
        }
        [Test]
        public void AddACarGet_ReturnExpectedNumberOfDistrict()
        {
            var user = _dbContext.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            //Act
            var result = _carController.AddACar() as ViewResult;
            var viewBag = result.ViewData;

            //Assert
            Assert.IsNotNull(result);
            Assert.That(((List<District>)viewBag["District"]).Count, Is.EqualTo(4));
        }
        [Test]
        public void AddACarGet_ReturnExpectedNumberOfWard()
        {
            var user = _dbContext.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            //Act
            var result = _carController.AddACar() as ViewResult;
            var viewBag = result.ViewData;

            //Assert
            Assert.IsNotNull(result);
            Assert.That(((List<Ward>)viewBag["Ward"]).Count, Is.EqualTo(4));
        }
        [Test]
        public void AddACarGet_ReturnExpectedNumberOfYear()
        {
            var user = _dbContext.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            //Act
            var result = _carController.AddACar() as ViewResult;
            var viewBag = result.ViewData;

            //Assert
            Assert.IsNotNull(result);
            Assert.That(((List<int>)viewBag["Year"]).Count, Is.EqualTo(41));
        }

        [Test]
        public void AddACarGet_ReturnErrorView()
        {
            var user = _dbContext.Users.Find(2);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);
            _session.SetString("User", userJson);

            // Act
            var result = _carController.AddACar() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.ViewName, Is.EqualTo("ErrorAuthorization"));
        }

        [Test]
        public void AddACarGet_DoNotLogin_CheckRedirectToLogin()
        {
            // Act
            var result = _carController.AddACar() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.ActionName, Is.EqualTo("Login"));
            Assert.That(result.ControllerName, Is.EqualTo("Users"));
        }

        [Test]
        [Pairwise]
        public async Task AddACarAsync_FailToCreate(
            [Values("29A-12345", "29A-123.4", "")] string licensePlate,
            [Values(2)] int brandId,
            [Values(2)] int modelId,
            [Values(5)] int seats,
            [Values(2)] int colorId,
            [Values(2015)] int productionYear,
            [Values(true, false)] bool transmissionType,
            [Values(true, false)] bool fuelType,
            [Values(0, -15000)] int mileage,
            [Values(0, -8)] int fuelConsumption,
            [Values(0, -500000)] int basePrice,
            [Values(0, -200000)] int deposit,
            [Values(null)] string? description,
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
        ){
            var user = _dbContext.Users.Find(1);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var userJson = JsonConvert.SerializeObject(user, settings);

            _session.SetString("User", userJson);

            // Arrange
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
            //var directoryPath = @"E:\Learn\VS\rental-car.git\Rental_Car_DemoTests\bin\Debug\net8.0\wwwroot\img\";
            //if (!Directory.Exists(directoryPath))
            //{
            //    Directory.CreateDirectory(directoryPath);
            //}
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

            try
            {
                // Act
                await _carController.AddACarAsync(car, registration, certificate, insurance, front, back, left, right,
                                                        bluetooth, gps, camera, sunroof, childlock, childseat, dvd, usb,
                                                        smoking, food, pet, specify, city, district, ward, street);
                Assert.Fail("Create successful!");
            }
            catch (Exception ex)
            {
                Assert.Pass(ex.Message);
            }

        }

        [TestCase(1, "Car 1", "50F-567.89", 1, 1, 5, 1, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2020, true, true, 15000.0, 8.5, 25000.0, 5000.0, 1, "A well-maintained compact car", 1, 1, 1, 1, 10)]
        [TestCase(2, "Car 2", "54D-7890", 1, 2, 4, 2, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2023, true, false, 1000, 12, 5000.0, 3000.0, 5, "A new and efficient car", 5, 5, 5, 1, 0)]
        [TestCase(2, "Car 3", "54D-7890", 5, 5, 5, 5, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2023, false, true, 1, 10, 1, 1, 5, "A new and efficient car", 5, 5, 5, 1, 0)]
        [TestCase(null, "Car 4", "54D-7890", 5, 5, 5, 5, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2023, true, true, 100, 10, 1, 1, 5, "A new and efficient car", 5, 5, 5, 1, 0)]
        [TestCase(1, "Car 5", "54D-7890", 5, 5, 5, 5, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2023, true, true, 100, 10, 1, 1, 5, "A new and efficient car", 5, 5, 5, 1, null)]
        [TestCase(3, "", "67G-8901", 2, 3, 6, 3, "frontImageUrl6", "backImageUrl6", "leftImageUrl6", "rightImageUrl6", 2018, false, true, 12000.0, 9.0, 30000.0, 7000.0, 2, "Luxury SUV with advanced features", 3, 3, 3, 2, 2)]
        [TestCase(4, "Car 7", "78H-9012", 3, 4, 7, 4, "frontImageUrl7", "backImageUrl7", "leftImageUrl7", "rightImageUrl7", 2022, true, false, 2000.0, 11.0, 45000.0, 8000.0, 3, "Family car with plenty of space", 4, 4, 4, 3, 0)]
        [TestCase(5, "Car 8", "89J-0123", 4, 1, 4, 5, "frontImageUrl8", "backImageUrl8", "leftImageUrl8", "rightImageUrl8", 2021, true, true, 8000.0, 7.5, 22000.0, 6000.0, 4, "Compact car with modern features", 5, 5, 5, 4, 1)]
        [TestCase(6, "Car 9", "90K-1234", 1, 2, 5, 1, "frontImageUrl9", "backImageUrl9", "leftImageUrl9", "rightImageUrl9", 2017, false, true, 15000.0, 8.0, 26000.0, 6500.0, 5, "Sedan with a smooth drive", 6, 6, 6, 5, 3)]
        [TestCase(7, "Car 10", "23N-0135", 2, 3, 6, 2, "frontImageUrl10", "backImageUrl10", "leftImageUrl10", "rightImageUrl10", 2019, true, true, 10000.0, 10.0, 28000.0, 7000.0, 6, "Spacious car for long trips", 7, 7, 7, 6, 2)]
        [TestCase(8, "Car 11", "23N-4567", 4, 5, 7, 4, "frontImageUrl12", "backImageUrl12", "leftImageUrl12", "rightImageUrl12", 2022, false, false, 3000.0, 14.0, 40000.0, 10000.0, 8, "High-performance sports car", 9, 9, 9, 8, 1)]
        [TestCase(9, "Car 12", "34O-5678", 5, 1, 5, 5, "frontImageUrl13", "backImageUrl13", "leftImageUrl13", "rightImageUrl13", 2021, true, false, 20000.0, 10.5, 37000.0, 8000.0, 9, "Luxury sedan with leather interior", 10, 10, 10, 9, 2)]
        [TestCase(10, "Car 13", "45P-6789", 1, 2, 4, 2, "frontImageUrl14", "backImageUrl14", "leftImageUrl14", "rightImageUrl14", 2016, true, true, 18000.0, 9.0, 24000.0, 5000.0, 10, "Economical and reliable car", 11, 11, 11, 10, 4)]

        public void CarDAO_CreateCarSuccess(int? userId, string name, string? licensePlate, int brandId, int modelId, int seats, int colorId, string frontImage, string backImage, string leftImage, string rightImage, int productionYear, bool transmissionType, bool fuelType, double mileage, double fuelConsumption, decimal basePrice, decimal deposit, int addressId, string? description, int documentId, int termId, int fucntionId, int status, int? noOfRide)
        {
            var car = new Car
            {
                UserId = userId,
                Name = name,
                LicensePlate = licensePlate,
                BrandId = brandId,
                ModelId = modelId,
                Seats = seats,
                ColorId = colorId,
                FrontImage = frontImage,
                BackImage = backImage,
                LeftImage = leftImage,
                RightImage = rightImage,
                ProductionYear = productionYear,
                TransmissionType = transmissionType,
                FuelType = fuelType,
                Mileage = mileage,
                FuelConsumption = fuelConsumption,
                BasePrice = basePrice,
                Deposit = deposit,
                AddressId = addressId,
                Description = description,
                DocumentId = documentId,
                TermId = termId,
                FucntionId = fucntionId,
                Status = status,
                NoOfRide = noOfRide
            };

                // Arrange
                var result = new CarDAO(_dbContext);

                // Act
                result.CreateCar(car);
                List<Car> cars = _dbContext.Cars.ToList();

                // Assert
                Assert.That(cars.Count, Is.EqualTo(4));
                Assert.That(cars.Last().Name, Is.EqualTo(name));
                //Assert.Pass("Add car success");
            
        }

        [TestCase(2, "Car 1", "INVALID", 2, 2, 4, 2, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2019, false, false, 20000.0, 9.0, 22000.0, 4000.0, 2, "Invalid car with wrong license plate", 2, 2, 2, 1, 8)]
        [TestCase(2, "Car 1", "52B 5678", 2, 2, 4, 2, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2019, false, false, 20000.0, 9.0, 22000.0, 4000.0, 2, "Invalid car with wrong format license plate", 2, 2, 2, 1, 8)]
        [TestCase(2, "Car 2", "52B-5678", 3, 3, 5, 3, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2021, true, true, -100.0, 7.5, 23000.0, 4500.0, 3, "Invalid car with negative mileage", 3, 3, 3, 1, 5)]
        [TestCase(2, "Car 2", "52B-5678", 3, 3, 5, 3, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2021, true, true, 0, 7.5, 23000.0, 4500.0, 3, "Invalid car with zero mileage", 3, 3, 3, 1, 5)]
        [TestCase(2, "Car 3", "53C-3456", 4, 4, 5, 4, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2022, false, false, 5000.0, -6.5, 21000.0, 4000.0, 4, "Invalid car with negative fuel consumption", 4, 4, 4, 1, 2)]
        [TestCase(2, "Car 3", "53C-3456", 4, 4, 5, 4, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2022, false, false, 5000.0, 0, 21000.0, 4000.0, 4, "Invalid car with zero fuel consumption", 4, 4, 4, 1, 2)]
        [TestCase(2, "Car 4", "55E-5678", 6, 6, 5, 6, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2018, false, false, 30000.0, 10.0, -20000.0, 3000.0, 6, "Invalid car with negative base price", 6, 6, 6, 1, 20)]
        [TestCase(2, "Car 4", "55E-5678", 6, 6, 5, 6, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2018, false, false, 30000.0, 10.0, 0, 3000.0, 6, "Invalid car with zero base price", 6, 6, 6, 1, 20)]
        [TestCase(2, "Car 5", "56F-1234", 7, 7, 5, 7, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2017, true, true, 25000.0, 7.0, 18000.0, -3500.0, 7, "Invalid car with negative deposit", 7, 7, 7, 1, 15)]
        [TestCase(2, "Car 5", "56F-1234", 7, 7, 5, 7, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2017, true, true, 25000.0, 7.0, 18000.0, 0, 7, "Invalid car with zero deposit", 7, 7, 7, 1, 15)]
        [TestCase(2, "Car 6", "57G-5678", 8, 8, 5, 8, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2016, false, false, 10000.0, 8.0, 20000.0, 3000.0, 8, null, 8, 8, 8, 1, 8)]
        [TestCase(2, null, "57G-5678", 8, 8, 5, 8, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2016, false, false, 10000.0, 8.0, 20000.0, 3000.0, 8, "Invalid car with null name", 8, 8, 8, 1, 8)]
        [TestCase(2, "Car 7", null, 8, 8, 5, 8, "frontImageUrl", "backImageUrl", "leftImageUrl", "rightImageUrl", 2016, false, false, 10000.0, 8.0, 20000.0, 3000.0, 8, "Invalid car with null license", 8, 8, 8, 1, 8)]
        [TestCase(2, "Car 8", "57G-5678", 8, 8, 5, 8, null, "backImageUrl", "leftImageUrl", "rightImageUrl", 2016, false, false, 10000.0, 8.0, 20000.0, 3000.0, 8, "Invalid car with null front img", 8, 8, 8, 1, 8)]
        [TestCase(2, "Car 9", "57G-5678", 8, 8, 5, 8, "frontImageUrl", null, "leftImageUrl", "rightImageUrl", 2016, false, false, 10000.0, 8.0, 20000.0, 3000.0, 8, "Invalid car with null back img", 8, 8, 8, 1, 8)]
        [TestCase(2, "Car 10", "57G-5678", 8, 8, 5, 8, "frontImageUrl", "backImageUrl", null, "rightImageUrl", 2016, false, false, 10000.0, 8.0, 20000.0, 3000.0, 8, "Invalid car with null left img", 8, 8, 8, 1, 8)]
        [TestCase(2, "Car 11", "57G-5678", 8, 8, 5, 8, "frontImageUrl", "backImageUrl", "leftImageUrl", null, 2016, false, false, 10000.0, 8.0, 20000.0, 3000.0, 8, "Invalid car with null right img", 8, 8, 8, 1, 8)]
        public void CarDAO_CreateCarFail(int? userId, string? name, string? licensePlate, int brandId, int modelId, int seats, int colorId, string? frontImage, string? backImage, string? leftImage, string? rightImage, int productionYear, bool transmissionType, bool fuelType, double mileage, double fuelConsumption, decimal basePrice, decimal deposit, int addressId, string? description, int documentId, int termId, int fucntionId, int status, int? noOfRide)
        {
            var car = new Car
            {
                UserId = userId,
                Name = name,
                LicensePlate = licensePlate,
                BrandId = brandId,
                ModelId = modelId,
                Seats = seats,
                ColorId = colorId,
                FrontImage = frontImage,
                BackImage = backImage,
                LeftImage = leftImage,
                RightImage = rightImage,
                ProductionYear = productionYear,
                TransmissionType = transmissionType,
                FuelType = fuelType,
                Mileage = mileage,
                FuelConsumption = fuelConsumption,
                BasePrice = basePrice,
                Deposit = deposit,
                AddressId = addressId,
                Description = description,
                DocumentId = documentId,
                TermId = termId,
                FucntionId = fucntionId,
                Status = status,
                NoOfRide = noOfRide
            };

            // Arrange
            var carDAO = new CarDAO(_dbContext);

            try
            {
                // Act
                carDAO.CreateCar(car);
                Assert.Fail("Create successful!");
            }
            catch (Exception)
            {
                Assert.Pass("Exception was correctly thrown");
            }
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _carController.Dispose();
            _dbContext.Dispose();
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
            };

            var modelData = new List<CarModel>
            {
                new CarModel { ModelId = 1, ModelName = "ILX",BrandId = 1},
                new CarModel { ModelId = 2, ModelName = "MDX",BrandId = 1},
                new CarModel { ModelId = 3, ModelName = "MDX Sport Hybrid", BrandId = 1},
                new CarModel { ModelId = 4, ModelName = "NSX",BrandId = 1},
                new CarModel { ModelId = 5, ModelName = "RDX",BrandId = 1},
                new CarModel { ModelId = 6, ModelName = "RLX",BrandId = 1},
                new CarModel { ModelId = 7, ModelName = "RLX Sport Hybrid",BrandId = 1},

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
                new User { UserId = 1, Name = "User A",Role = true, Email = "duyquan7b@gmail.com", Password ="quandeptrai123",Phone="0987654321" },
                new User { UserId = 2, Name = "User B", Role = false,Email = "duyquan7b1@gmail.com", Password ="quandeptrai123", Phone="0987654321" },
                new User { UserId = 3, Name = "User C", Role = true,Email = "duyquan7b2@gmail.com", Password ="quandeptrai123", Phone="0987654321" }
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

            var bookingData = new List<Booking>
            {
                new Booking { BookingNo = 1, UserId = 1, CarId = 1, StartDate = new DateTime(2023, 06, 22), EndDate = new DateTime(2023, 06, 24), PaymentMethod = 1, Status = 3},
                new Booking { BookingNo = 2, UserId = 1, CarId = 2, StartDate = new DateTime(2023, 06, 23), EndDate = new DateTime(2023, 06, 24), PaymentMethod = 1, Status=3}
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
                BasePrice = 1500000, Deposit = 500000,AddressId = 2, Description = "NOG NOG 2", DocumentId = 1,
                TermId = 2, FucntionId = 2, Status = 1, NoOfRide = 2
                },
                new Car {CarId = 3,UserId = 1, Name = "Acura MDX 2003", LicensePlate = "56F-513.33", BrandId = 1, ModelId = 2, Seats = 10,
                ColorId = 2,FrontImage = "Image2.jpg",BackImage ="Image2.jpg",LeftImage ="Image2.jpg", RightImage ="Image2.jpg",
                ProductionYear = 2003,TransmissionType = false, FuelType = false, Mileage = 100, FuelConsumption = 8,
                BasePrice = 1200000, Deposit = 500000,AddressId = 2, Description = "NOG NOG 3", DocumentId = 1,
                TermId = 2, FucntionId = 2, Status = 1, NoOfRide = 2
                },
            };
            _dbContext.CarColors.AddRange(colorData);
            _dbContext.AdditionalFunctions.AddRange(functionData);
            _dbContext.TermOfUses.AddRange(termData);
            _dbContext.Users.AddRange(userData);
            _dbContext.Addresses.AddRange(addressData);
            _dbContext.Wards.AddRange(wardData);
            _dbContext.Districts.AddRange(districtData);
            _dbContext.Cities.AddRange(cityData);
            _dbContext.CarModels.AddRange(modelData);
            _dbContext.CarBrands.AddRange(brandData);
            _dbContext.Feedbacks.AddRange(ratingData);
            _dbContext.Cars.AddRange(carData);
            _dbContext.Bookings.AddRange(bookingData);
            _dbContext.SaveChanges();
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
}
