using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository;
using Rental_Car_Demo.Repository.CarRepository;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Rental_Car_Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICarRepository _carRepository;

        private RentCarDbContext _context;

        public HomeController(ILogger<HomeController> logger, ICarRepository carRepository, RentCarDbContext context)
        {
            _logger = logger;
            _carRepository = carRepository;
            _context = context;
        }

        public IActionResult Index()
        {
            return View ();
        }

        public IActionResult Privacy()
        {
            return View ();
        }

        [ResponseCache (Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View (new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        

        [HttpPost]
        public IActionResult SearchCarForm(string address)
        {
            _logger.LogInformation ($"Search parameters: address={address}");
            IEnumerable<Car> cars = _carRepository.GetAllCars (address);

            return View (cars);
        }

        public IActionResult SearchCar(string brandName, int? seats, bool? transmissionType, string brandLogo, decimal? minPrice, decimal? maxPrice, string address)
        {
            IEnumerable<Car> cars = _carRepository.SearchCars (brandName, seats, transmissionType, brandLogo, minPrice, maxPrice, address);

            return PartialView ("_CarResultsPartial", cars);
        }


        [HttpGet]
        public JsonResult GetSuggestions(string query)
        {
            var addresses = _context.Cars
                .Include(car => car.Address)
                    .ThenInclude(address => address.City)
                .Include(car => car.Address)
                    .ThenInclude(address => address.District)
                .Include(car => car.Address)
                    .ThenInclude(address => address.Ward)
                .Where(car => car.Status == 1 &&
                  (car.Address.District.DistrictName.Contains(query) ||
                   car.Address.Ward.WardName.Contains(query) ||
                   car.Address.City.CityProvince.Contains(query) ||
                   car.Address.HouseNumberStreet.Contains(query)))
                .Select(car => new
                {
                    Address = $"{car.Address.HouseNumberStreet}, {car.Address.Ward.WardName}, {car.Address.District.DistrictName}, {car.Address.City.CityProvince}"
                })
                .ToList();


            return Json (addresses.Select (a => a.Address).ToList ());
        }

        [HttpGet]
        public IActionResult GetUserFeedbacks(int userId)
        {
            var feedbacks = _context.Feedbacks
                .Include (f => f.BookingNoNavigation)
                .ThenInclude (b => b.Car)
                .Where (f => f.BookingNoNavigation.UserId == userId)
                .ToList ();

            return View (feedbacks);
        }
    }
}

