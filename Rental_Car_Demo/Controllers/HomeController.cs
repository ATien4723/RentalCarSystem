using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository;
using Rental_Car_Demo.Repository.CarRepository;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Rental_Car_Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICarRepository _carRepository;

        private RentCarDbContext _context;

        public HomeController(ICarRepository carRepository, RentCarDbContext context)
        {
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


        //[HttpGet]
        //public IActionResult SearchCarForm(string? address, DateOnly? pickupDate, TimeOnly? pickupTime, DateOnly? dropoffDate, TimeOnly? dropoffTime)
        //{
        //    _logger.LogInformation($"Search parameters: address={address}");
        //    IEnumerable<Car> cars = _carRepository.GetAllCars(address);

        //    //get user to block customer access this view
        //    var userString = HttpContext.Session.GetString("User");
        //    User user = null;
        //    if (!string.IsNullOrEmpty(userString))
        //    {
        //        user = JsonConvert.DeserializeObject<User>(userString);
        //    }

        //    if ( user != null && user.Role == true)
        //    {
        //        return View("ErrorAuthorization");
        //    }

        //    ViewBag.location = address;
        //    ViewBag.pickupDate = pickupDate;
        //    ViewBag.pickupTime = pickupTime;
        //    ViewBag.dropoffDate = dropoffDate;
        //    ViewBag.dropoffTime = dropoffTime;
        //    return View(cars);
        //}




        [HttpGet]
        public IActionResult SearchCarForm(string? address, DateOnly? pickupDate, TimeOnly? pickupTime, DateOnly? dropoffDate, TimeOnly? dropoffTime)
        {

            // Get current date and time
            DateTime currentDateTime = DateTime.Now;

            // Set default pickup date to current date if not provided
            pickupDate ??= DateOnly.FromDateTime(currentDateTime);

            // Set default dropoff date to the day after the current date if not provided
            dropoffDate ??= DateOnly.FromDateTime(currentDateTime.AddDays(1));

            // Set default time to current time if not provided
            pickupTime ??= TimeOnly.FromDateTime(currentDateTime);
            dropoffTime ??= TimeOnly.FromDateTime(currentDateTime);

            // Pass values to ViewBag
            ViewBag.location = address;
            ViewBag.pickupDate = pickupDate;
            ViewBag.pickupTime = pickupTime;
            ViewBag.dropoffDate = dropoffDate;
            ViewBag.dropoffTime = dropoffTime;

            IEnumerable<Car> cars = _carRepository.GetAllCars(address);

            return View(cars);
        }

        public IActionResult SearchCar(string brandName, int? seats, bool? transmissionType,bool? fuelType, string brandLogo, string[] priceRange, string address)
        {
            decimal? minPrice = null;
            decimal? maxPrice = null;

            if ( priceRange != null && priceRange.Length > 0 ) {
                foreach ( var range in priceRange ) {
                    var prices = range.Split ('-');
                    if ( prices.Length == 2 ) {
                        if ( decimal.TryParse (prices[0], out decimal min) && decimal.TryParse (prices[1], out decimal max) ) {
                            if ( minPrice == null || min < minPrice ) minPrice = min;
                            if ( maxPrice == null || max > maxPrice ) maxPrice = max;
                        }
                    }
                }
            }

            IEnumerable<Car> cars = _carRepository.SearchCars(brandName, seats, transmissionType,fuelType, brandLogo, minPrice, maxPrice, address);

            var userString = HttpContext.Session.GetString ("User");
            User user = null;
            if ( !string.IsNullOrEmpty (userString) ) {
                user = JsonConvert.DeserializeObject<User> (userString);
            }
            if ( user != null && user.Role == true ) {
                return PartialView ("ErrorAuthorization");
            }

            return PartialView ("_CarResultsPartial", cars);
        }



        [HttpGet]
        public JsonResult GetSuggestions(string query)
        {
            if ( string.IsNullOrEmpty (query) ) {
                return Json (new List<string> ());
            }

            query = query.Trim ();

            if ( query.Length < 2 || query.Length > 100 ) {
                return Json (new List<string> ());
            }

            if ( Regex.IsMatch (query, @"[^a-zA-Z0-9\s]") ) {
                return Json (new List<string> ());
            }

            // Fetch matching addresses from the database
            var addresses = _context.Cars
                .Include (car => car.Address)
                    .ThenInclude (address => address.City)
                .Include (car => car.Address)
                    .ThenInclude (address => address.District)
                .Include (car => car.Address)
                    .ThenInclude (address => address.Ward)
                .Where (car => car.Status == 1 &&
                    ( car.Address.District.DistrictName.Contains (query) ||
                     car.Address.Ward.WardName.Contains (query) ||
                     car.Address.City.CityProvince.Contains (query) ||
                     car.Address.HouseNumberStreet.Contains (query) ))
                .Select (car => new
                {
                    Address = $"{car.Address.HouseNumberStreet}, {car.Address.Ward.WardName}, {car.Address.District.DistrictName}, {car.Address.City.CityProvince}"
                })
                .ToList ();

            return Json (addresses.Select (a => a.Address).ToList ());
        }

        [HttpGet]
        public IActionResult GetUserFeedbacks(int userId)
        {
            //get user to block customer access this view
            var userString = HttpContext.Session.GetString("User");

            if(userString == null)
            {
                return RedirectToAction("Guest", "Users");
            }

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

