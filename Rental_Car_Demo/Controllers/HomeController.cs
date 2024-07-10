using Microsoft.AspNetCore.Mvc;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.CarRepository;
using System.Diagnostics;

namespace Rental_Car_Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICarRepository _carRepository;

        public HomeController(ILogger<HomeController> logger, ICarRepository carRepository)
        {
            _logger = logger;
            _carRepository = carRepository;
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

        public IActionResult SearchCar()
        {
            IEnumerable<Car> cars = _carRepository.GetAllCars ();
            return View (cars);
        }

    }
}
