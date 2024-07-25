using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.ViewModel;

namespace Rental_Car_Demo.Controllers
{
    public class PaymentController : Controller
    {
        RentCarDbContext db = new RentCarDbContext();

        public IActionResult MyWallet(int userId, DateTime? from, DateTime? to)
        {
            //get user to block customer access this view
            var userString = HttpContext.Session.GetString("User");
            User userLogged = null;
            if (!string.IsNullOrEmpty(userString))
            {
                userLogged = JsonConvert.DeserializeObject<User>(userString);
            }
            if (userLogged.UserId != userId)
            {
                return View("ErrorAuthorization");
            }


            var user = db.Users.SingleOrDefault(u => u.UserId == userId);
            var wallets = db.Wallets.Where(w => w.UserId == userId).OrderByDescending(w => w.CreatedAt).ToList();

            var viewModel = new UserWalletViewModel
            {
                User = user,
                Wallets = wallets
            };
            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Withdraw(int userId, decimal amount)
        {
            var user = db.Users.SingleOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                if (user.Wallet >= amount)
                {
                    user.Wallet -= amount;
                    var transaction = new Wallet
                    {
                        UserId = userId,
                        Amount = (-amount).ToString("N2"),
                        Type = "Withdraw",
                        CreatedAt = DateTime.Now
                    };
                    db.Wallets.Add(transaction);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, error = "Insufficient balance" });
                }
            }
            return Json(new { success = false, error = "User not found" });
        }
        [HttpPost]
        public IActionResult TopUp(int userId, decimal amount)
        {
            var user = db.Users.SingleOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                if (user.Wallet == null)
                {
                    user.Wallet = 0;
                }

                user.Wallet += amount;

                var transaction = new Wallet
                {
                    UserId = userId,
                    Amount = amount.ToString("N2"),
                    Type = "Top-Up",
                    CreatedAt = DateTime.Now
                };
                db.Wallets.Add(transaction);
                db.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false, error = "User not found" });
        }
        [HttpPost]
        public IActionResult SearchTransactions(int userId, DateTime? from, DateTime? to)
        {

            var user = db.Users.SingleOrDefault(u => u.UserId == userId);
            if (user != null)
            {

                var toEndOfDay = to.Value.Date.AddDays(1).AddTicks(-1);
                var wallets = db.Wallets.Where(w => w.UserId == userId && w.CreatedAt >= from && w.CreatedAt <= toEndOfDay).ToList();


                var viewModel = new UserWalletViewModel
                {
                    User = user,
                    Wallets = wallets
                };
                ViewBag.From = from;
                ViewBag.To = to;
                return View("MyWallet", viewModel);
            }

            return NotFound();
        }
        [HttpPost]
        public IActionResult ConfirmPayment(int carId)
        {
            var car = db.Cars.SingleOrDefault(c => c.CarId == carId);
            var booking = (from b in db.Bookings
                           join c in db.Cars on b.CarId equals c.CarId
                           where b.CarId == carId
                           && b.BookingNo == db.Bookings
                                                .Where(b2 => b2.CarId == carId)
                                                .Max(b2 => b2.BookingNo) && b.Status == 5
                           select new
                           {
                               Booking = b,
                               Car = c
                           }).SingleOrDefault();
            if (car != null && booking != null)
            {
                car.Status = 1;
                db.Cars.Update(car);
                db.SaveChanges();
            }
            else
            {
                TempData["ErrorMessage"] = "Customer has not paid the car rental.";
            }
            return RedirectToAction("ChangeCarDetailsByOwner", "Car", new { CarId = carId });
        }

    }
}
