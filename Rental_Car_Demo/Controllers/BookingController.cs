using Microsoft.AspNetCore.Mvc;

namespace Rental_Car_Demo.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
