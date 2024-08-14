using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Rental_Car_Demo.Context;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.UserRepository;
using Rental_Car_Demo.ViewModel;
using System.Globalization;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Rental_Car_Demo.Services;
using NuGet.Common;
using Castle.Core.Resource;


namespace Rental_Car_Demo.Controllers
{
    public class UsersController : Controller
    {

        private readonly RentCarDbContext context;
        private readonly ICustomerContext _customerContext;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IEmailService _emailService;
        private readonly IFormFile _file;

        public UsersController(
            RentCarDbContext _context,
            ICustomerContext customerContext,
            ITokenGenerator tokenGenerator,
            IEmailService emailService)
        {
            context = _context;
            _customerContext = customerContext;
            _tokenGenerator = tokenGenerator;
            _emailService = emailService;
        }

        UserDAO userDAO = new UserDAO();


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
                var user = context.Users.Where(x => x.Email.Equals(userLogin.User.Email) && x.Password.Equals(hashedPassword)).FirstOrDefault();

                if (user != null)
                {
                    HttpContext.Session.SetString("User", JsonConvert.SerializeObject(user));

                    if (userLogin.User.RememberMe)
                    {

                        string rememberMeValue = $"{userLogin.User.Email}|{userLogin.User.Password}";
                        string encodedRememberMeValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(rememberMeValue));

                        Response.Cookies.Append("UserEmail", encodedRememberMeValue, new CookieOptions
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
                    TempData["ShowModal"] = "no";
                    return (bool)user.Role ? RedirectToAction("LoginOwn", "Users") : RedirectToAction("LoginCus", "Users");
                }

                if (user == null && !string.IsNullOrEmpty(userLogin.User.Email) && !string.IsNullOrEmpty(userLogin.User.Password))
                {
                    ViewBag.Error = "Either email address or password is incorrect. Please try again";
                }
            }

            TempData["ShowModal"] = "yes";
            return View("Guest");
        }

        public string HashPassword(string password)
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

            return View();
        }
        public IActionResult LoginOwn()
        {
            //get user to block customer access this view
            var userString = HttpContext.Session.GetString("User");
            User user = null;
            if (!string.IsNullOrEmpty(userString))
            {
                user = JsonConvert.DeserializeObject<User>(userString);
            }
            if (user.Role == false)
            {
                return View("ErrorAuthorization");
            }

            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User");
            return RedirectToAction("Guest", "Users");
        }

        public IActionResult Guest()
        {

            var viewModel = new RegisterAndLoginViewModel
            {
                Register = new RegisterViewModel(),
                User = new User()
            };

            if (Request.Cookies.TryGetValue("UserEmail", out string encodedRememberMeValue))
            {
                string rememberMeValue = Encoding.UTF8.GetString(Convert.FromBase64String(encodedRememberMeValue));

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


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterAndLoginViewModel model)
        {
            var checkMail = IsEmailExist(model.Register.Email);
            // Kiểm tra email trùng lặp
            if (checkMail == true)
            {
                ModelState.AddModelError("Register.Email", "Email already existed. Please try another email.");
                TempData["ShowModal"] = "yes";
                TempData["ShowPanel"] = "register";
                return View("Guest", model);
            }

            if (model.Register.AgreeToTerms == false)
            {
                ModelState.AddModelError("Register.AgreeToTerms", "Please agree to this!");
                TempData["ShowModal"] = "yes";
                TempData["ShowPanel"] = "register";
                return View("Guest", model);
            }

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

                // Thêm customer vào context và lưu thay đổi
                context.Add(customer);
                context.SaveChanges();

                // Hiển thị thông báo đăng ký thành công
                TempData["SuccessMessage"] = "Account created successfully!";
                TempData["ShowModal"] = "no";
                return RedirectToAction("Guest", "Users");
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Return the model to the view if the model state is invalid
            }

            string tokenValue = _tokenGenerator.GenerateToken(32);
            DateTime exTime = _tokenGenerator.GetExpirationTime();

            string email = model.Email;
            int user = _customerContext.getCustomerIdByEmail(email);

            if (user == -1) // not found email
            {
                TempData["FailMessage"] = "Sorry, Your email does not exist in our database!";
            }
            else
            {
                var token = new TokenInfor()
                {
                    Token = tokenValue,
                    UserId = user,
                    ExpirationTime = exTime,
                    IsLocked = false
                };

                context.Add(token);
                context.SaveChanges();

                int? customerId = token.UserId;

                string resetLink = Url.Action("ResetPassword2", "Users", new { customerId = customerId, tokenValue = tokenValue }, Request.Scheme);
                string subject = "Rent-a-car Password Reset";
                _emailService.SendEmail(model.Email, subject, "We have just received a password reset request for" + "<"+ email +">" +" .\r\nPlease click here to reset your password. \r\nFor your security, the link will expire in 24 hours or immediately after you reset your password. \r\n" + resetLink);
                TempData["SuccessMessage"] = "We will send link to reset your password in the email!";
            }

            return View(); // Return the view without a model to reset the form
        }


        public IActionResult ResetPassword2(int customerId, string tokenValue)
        {

            var token = context.TokenInfors.FirstOrDefault(t => t.Token == tokenValue && t.UserId == customerId);

            if (token == null || token.IsLocked == true || token.ExpirationTime < DateTime.Now)
            {
                return View("Fail");
            }

            ResetPassword2ViewModel model = new ResetPassword2ViewModel
            {
                CustomerId = customerId,
            };

            TempData["token"] = token.Token;

            return View(model);
        }

        [HttpPost]
        public IActionResult ResetPassword2(ResetPassword2ViewModel model, string tokenvalue)
        {
            if (ModelState.IsValid)
            {
                var token = context.TokenInfors.FirstOrDefault(t => t.Token == tokenvalue && t.UserId == model.CustomerId);

                var customer = context.Users.FirstOrDefault(t => t.UserId == model.CustomerId);

                var hashPass = HashPassword(model.Password);
                customer.Password = hashPass;
                context.Update(customer);
                context.SaveChanges();

                TempData["SuccessMessage"] = "Your password has been reset";

                token.IsLocked = true;
                context.Update(token);
                context.SaveChanges();

                return View("Guest");
            }
            return View(model);

        }

        public bool IsEmailExist(string email)
        {
            return context.Users.Any(u => u.Email == email);
        }




        public ActionResult Edit(int id)
        {
            var userString = HttpContext.Session.GetString("User");
            User userLogged = null;
            if (!string.IsNullOrEmpty(userString))
            {
                userLogged = JsonConvert.DeserializeObject<User>(userString);
            }
            if (userLogged == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (userLogged.UserId != id)
            {
                return View("ErrorAuthorization");
            }
            var user = context.Users
                .Include(u => u.Address)
                    .ThenInclude(a => a.District)
                .Include(u => u.Address)
                    .ThenInclude(a => a.Ward)
                .SingleOrDefault(u => u.UserId == id);

            var address = user.Address;

            if (address == null)
            {
                ViewBag.Cities = new SelectList(context.Cities.ToList(), "CityId", "CityProvince");
            }
            else
            {
                var city = context.Cities.ToList();
                var district = context.Districts.Where(d => d.CityId == address.CityId).ToList();
                var ward = context.Wards.Where(d => d.DistrictId == address.DistrictId).ToList();

                ViewBag.Cities = new SelectList(city, "CityId", "CityProvince", address.CityId);
                ViewBag.Districts = new SelectList(district, "DistrictId", "DistrictName", address.DistrictId);
                ViewBag.Wards = new SelectList(ward, "WardId", "WardName", address.WardId);
            }
            
            return View(user);
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user, string? NewPassword, string? ConfirmPassword, string? CurrentPassword)
        {
            var address = user.Address;

            if (address == null)
            {
                ViewBag.Cities = new SelectList(context.Cities.ToList(), "CityId", "CityProvince");
            }
            else
            {
                var city = context.Cities.ToList();
                var district = context.Districts.Where(d => d.CityId == address.CityId).ToList();
                var ward = context.Wards.Where(d => d.DistrictId == address.DistrictId).ToList();

                ViewBag.Cities = new SelectList(city, "CityId", "CityProvince", address.CityId);
                ViewBag.Districts = new SelectList(district, "DistrictId", "DistrictName", address.DistrictId);
                ViewBag.Wards = new SelectList(ward, "WardId", "WardName", address.WardId);
            }

            var saveUser = context.Users.SingleOrDefault(u => u.UserId == user.UserId);
            var isPasswordChange = false;

            var passwordPattern = @"^(?=.*[A-Za-z])(?=.*\d).{7,}$";
            var regex = new Regex(passwordPattern);

            if (string.IsNullOrEmpty(CurrentPassword))
            {
                if (!string.IsNullOrEmpty(NewPassword) || !string.IsNullOrEmpty(ConfirmPassword))
                {
                    ModelState.AddModelError("", "Please enter current password before enter new password!");
                    return View(user);
                }
            }
            else
            {
                if (HashPassword(CurrentPassword) == saveUser.Password)
                {
                    if (!string.IsNullOrEmpty(NewPassword) && !string.IsNullOrEmpty(ConfirmPassword))
                    {
                        if (NewPassword == ConfirmPassword && regex.IsMatch(NewPassword))
                        {
                            saveUser.Password = HashPassword(NewPassword);
                            isPasswordChange = true;
                        }
                        else
                        {
                            ModelState.AddModelError("", "New password must be matched with Confirm new password and contains at least seven characters long include at least one letter and one number!");
                            return View(user);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Please enter both new password and confirm new password!");
                        return View(user);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Current password is not correct!");
                    return View(user);
                }
            }

            
            bool emailExists = context.Users.Any(u => u.Email == user.Email && u.UserId != user.UserId);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email already existed. Please try another email.");
                return View(user);
            }

            saveUser.Email = user.Email;
            saveUser.RememberMe = user.RememberMe;
            saveUser.Role = user.Role;
            saveUser.Name = user.Name;
            saveUser.Dob = user.Dob;
            saveUser.NationalId = user.NationalId;
            saveUser.Phone = user.Phone;
            saveUser.AddressId = user.AddressId;
            saveUser.DrivingLicense = user.DrivingLicense;
            saveUser.Wallet = user.Wallet;
            saveUser.Address = user.Address;
            saveUser.Bookings = user.Bookings;
            saveUser.Cars = user.Cars;

            context.SaveChanges();

            var currentUser = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("User"));
            currentUser.Name = user.Name;
            HttpContext.Session.SetString("User", JsonConvert.SerializeObject(currentUser));

            if (isPasswordChange)
            {
                return RedirectToAction("Logout", "Users");
            }
            if (currentUser.Role == false)
            {
                return RedirectToAction("LoginCus", "Users");
            }
            else
            {
                return RedirectToAction("LoginOwn", "Users");
            }
        }



        [HttpGet]
        public JsonResult GetDistricts(int cityId)
        {
            var districts = context.Districts.Where(d => d.CityId == cityId);
            return Json(districts);
        }

        [HttpGet]
        public JsonResult GetWards(int districtId)
        {
            var wards = context.Wards.Where(w => w.DistrictId == districtId);
            return Json(wards);
        }

        [HttpPost]
        public IActionResult UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    return Json(new { success = false, message = "File already exists" });
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Invalid file" });
        }
    }
}