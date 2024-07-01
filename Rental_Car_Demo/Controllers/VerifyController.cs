using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Rental_Car_Demo.Models;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Rental_Car_Demo.Controllers
{
    public class VerifyController : Controller
    {
        RentCarDbContext db = new RentCarDbContext();
        public IActionResult Login()
        {
            string culture = "or-IN";
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)), new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                });
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            User user = new User();
            if (Request.Cookies.TryGetValue("UserEmail", out string rememberMeValue))
            {
                var values = rememberMeValue.Split('|');
                if (values.Length == 2)
                {
                    user = new User
                    {
                        Email = values[0],
                        Password = values[1],
                        RememberMe = true
                    };
                }
            }

            return View(user);
        }
        [HttpPost]
        public IActionResult Login(User userLogin)
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                string hashedPassword = HashPassword(userLogin.Password);
                var user = db.Users.Where(x => x.Email.Equals(userLogin.Email)
            && x.Password.Equals(hashedPassword)).FirstOrDefault();

                if (user != null)
                {
                    HttpContext.Session.SetString("User", JsonConvert.SerializeObject(user));

                    if (userLogin.RememberMe)
                    {
                        Response.Cookies.Append("UserEmail", $"{userLogin.Email}|{userLogin.Password}", new
                            CookieOptions
                        {
                            Expires = DateTime.UtcNow.AddDays(30),
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.None
                        });
                    }
                    else
                    {
                        Response.Cookies.Delete("UserEmail");
                    }

                    return (bool)user.Role ? RedirectToAction("LoginOwn", "Verify") : RedirectToAction("LoginCus", "Verify");
                }

                if (user == null && !string.IsNullOrEmpty(userLogin.Email) && !string.IsNullOrEmpty(userLogin.Password))
                {
                    ViewBag.Error = "Either email address or password is incorrect. Please try again";
                }
            }

            return View();
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public IActionResult LoginCus()
        {
            return View();
        }
        public IActionResult LoginOwn()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User");
            return RedirectToAction("Guest", "Verify");
        }
        public IActionResult Guest()
        {
            return View();
        }
    }
}
