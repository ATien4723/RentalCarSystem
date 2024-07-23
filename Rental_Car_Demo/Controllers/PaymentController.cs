using Microsoft.AspNetCore.Mvc;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.ViewModel;

namespace Rental_Car_Demo.Controllers
{
    public class PaymentController : Controller
    {
        RentCarDbContext db = new RentCarDbContext();
        public IActionResult MyWallet(int userId, DateTime? from, DateTime? to)
        {
            var user = db.Users.SingleOrDefault(u => u.UserId == userId);
            var wallets = db.Wallets.Where(w => w.UserId == userId).ToList();

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
                        Amount = (amount).ToString("N2"),
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
                if(user.Wallet == null ) {
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

    }
}
