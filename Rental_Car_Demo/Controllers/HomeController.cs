using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public IActionResult SearchCarForm(string? address)
        {
            _logger.LogInformation($"Search parameters: address={address}");
            IEnumerable<Car> cars = _carRepository.GetAllCars(address);

            return View(cars);
        }

        public IActionResult SearchCarForm()
        {
            IEnumerable<Car> cars = _carRepository.GetAllCars();

            return View(cars);
        }


        public IActionResult SearchCar(string brandName, int? seats, bool? transmissionType, string brandLogo, decimal? minPrice, decimal? maxPrice, string address)
        {
            IEnumerable<Car> cars = _carRepository.SearchCars(brandName, seats, transmissionType, brandLogo, minPrice, maxPrice, address);

            //get user to block customer access this view
            var userString = HttpContext.Session.GetString("User");
            User user = null;
            if (!string.IsNullOrEmpty(userString))
            {
                user = JsonConvert.DeserializeObject<User>(userString);
            }
            if (user.Role == true)
            {
                return View("ErrorAuthorization");
            }

            return PartialView("_CarResultsPartial", cars);
        }


        [HttpGet]
        public JsonResult GetSuggestions(string query)
        {
            var addresses = _context.Addresses
                .Include(a => a.City)
                .Include(a => a.District)
                .Include(a => a.Ward)
                .Where(a => a.District.DistrictName.Contains(query) ||
                            a.Ward.WardName.Contains(query) ||
                            a.City.CityProvince.Contains(query) ||
                            a.HouseNumberStreet.Contains(query))
                .Select(a => new
                {
                    Address = $"{a.HouseNumberStreet}, {a.Ward.WardName}, {a.District.DistrictName}, {a.City.CityProvince}"
                })
                .ToList();

            return Json(addresses.Select(a => a.Address).ToList());
        }

        [HttpGet]
        public IActionResult GetUserFeedbacks(int userId)
        {
            //get user to block customer access this view
            var userString = HttpContext.Session.GetString("User");
            User user = null;
            if (!string.IsNullOrEmpty(userString))
            {
                user = JsonConvert.DeserializeObject<User>(userString);
            }
            if (user.Role == true || user.UserId != userId)
            {
                return View("ErrorAuthorization");
            }

            var feedbacks = _context.Feedbacks
                .Include(f => f.BookingNoNavigation)
                .ThenInclude(b => b.Car)
                .Where(f => f.BookingNoNavigation.UserId == userId)
                .ToList();

            return View(feedbacks);
        }


        [HttpGet]
        public IActionResult GetCarOwnerFeedbacks(int userId)
        {
            //get user to block customer access this view
            var userString = HttpContext.Session.GetString("User");
            User user = null;
            if (!string.IsNullOrEmpty(userString))
            {
                user = JsonConvert.DeserializeObject<User>(userString);
            }

            if (user.Role == false || user.UserId != userId) // neu user la customer thi khong duoc vao trang nay hoac co gang truy cap vao feedback nguoi khac
            {
                return View("ErrorAuthorization");
            }


            var feedbacks = _context.Feedbacks
                .Include(f => f.BookingNoNavigation)
                .ThenInclude(b => b.Car)
                .Where(f => f.BookingNoNavigation.Car.UserId == userId)
                .ToList();

            return View(feedbacks);
        }
    }
}

