using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Rental_Car_Demo.Models;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Rental_Car_Demo.Context;
using Rental_Car_Demo.Validation;
using Rental_Car_Demo.ViewModel;

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

    var viewModel = new RegisterAndLoginViewModel
    {
        Register = new RegisterViewModel(),
        User = new User()
    };

    if (Request.Cookies.TryGetValue("UserEmail", out string rememberMeValue))
    {
        var values = rememberMeValue.Split('|');
        if (values.Length == 2)
        {
            viewModel.User = new User
            {
                Email = values[0],
                Password = values[1],
                RememberMe = true
            };
        }
    }

    return View(viewModel);
}

        [HttpPost]
        public IActionResult Login(RegisterAndLoginViewModel userLogin)
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                string hashedPassword = HashPassword(userLogin.User.Password);
                var user = db.Users.Where(x => x.Email.Equals(userLogin.User.Email)
            && x.Password.Equals(hashedPassword)).FirstOrDefault();

                if (user != null)
                {
                    HttpContext.Session.SetString("User", JsonConvert.SerializeObject(user));

                    if (userLogin.User.RememberMe)
                    {
                        Response.Cookies.Append("UserEmail", $"{userLogin.User.Email}|{userLogin.User.Password}", new
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

                if (user == null && !string.IsNullOrEmpty(userLogin.User.Email) && !string.IsNullOrEmpty(userLogin.User.Password))
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






        RentCarDbContext context = new RentCarDbContext();
        CustomerContext customerContext = new CustomerContext();
        TokenGenerator tokenGenerator = new TokenGenerator();

        private readonly IEmailService _emailService;

        public VerifyController(IEmailService emailService)
        {
            this._emailService = emailService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterAndLoginViewModel model)
        {
            // Kiểm tra email trùng lặp
            if (IsEmailExist(model.Register.Email))
            {
                ModelState.AddModelError("Email", "Email already existed. Please try another email.");
            }

            // Kiểm tra tính hợp lệ của ModelState
            //if (ModelState.IsValid)
            //{
                // Hash mật khẩu
                var hashedPassword = HashPassword(model.Register.Password);
                bool isCarOwner = model.Register.Role == "carOwner";

                var customer = new User
                {
                    Email = model.Register.Email,
                    Password = hashedPassword,
                    Name = model.Register.Name,
                    Phone = model.Register.Phone,
                    Role = isCarOwner
                };

                try
                {
                    // Thêm customer vào context và lưu thay đổi
                    context.Add(customer);
                    context.SaveChanges();


                    // Hiển thị thông báo đăng ký thành công
                    TempData["SuccessMessage"] = "Account created successfully!";
                    return RedirectToAction("Login", "Verify");
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi nếu xảy ra
                    ModelState.AddModelError("", "An error occurred while creating the account: " + ex.Message);
                }
            //}

            // Nếu có lỗi, hiển thị lại form đăng ký với thông báo lỗi
            return View("Fail");
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            string tokenValue = tokenGenerator.GenerateToken(32);
            DateTime exTime = tokenGenerator.GetExpirationTime();
            string em = "";
            em = model.Email;

            var token = new TokenInfor()
            {
                Token = tokenValue,
                UserId = customerContext.getCustomerIdByEmail(model.Email),
                //UserId = 1,
                ExpirationTime = exTime,
                IsLocked = false
            };

            context.Add(token);
            context.SaveChanges();

            int? customerId = token.UserId;

            string resetLink = Url.Action("ResetPassword2", "Verify", new { customerId = customerId, tokenValue = tokenValue }, Request.Scheme);
            string subject = "Link Reset Password";
            _emailService.SendEmail(model.Email, subject, resetLink);

            return View();
        }


        public IActionResult ResetPassword2(int customerId, string tokenValue)
        {

            ResetPassword2ViewModel model = new ResetPassword2ViewModel
            {
                CustomerId = customerId,
            };

            var token = context.TokenInfors.FirstOrDefault(t => t.Token == tokenValue);

            if (token == null || token.IsLocked == true || token.ExpirationTime < DateTime.Now)
            {
                return View("Fail");
            }

            token.IsLocked = true;
            context.Update(token);
            context.SaveChanges();

            return View(model);
        }

        //private void isValidToken(string tokenValue)
        //{
        //    var storedToken = context.Tokens.FirstOrDefault(t => t.Token == tokenValue);

        //}


        [HttpPost]
        public IActionResult ResetPassword2(ResetPassword2ViewModel model)
        {
            if (ModelState.IsValid)
            {
                var customer = context.Users.FirstOrDefault(t => t.UserId == model.CustomerId);
                var hashPass = HashPassword(customer.Password);
                customer.Password = hashPass;
                context.Update(customer);
                context.SaveChanges();


                return View("Register");
            }
            return View("Fail");

        }



        public bool IsEmailExist(string email)
        {
            return context.Users.Any(u => u.Email == email);
        }

    }
}
