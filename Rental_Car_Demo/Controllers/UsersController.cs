using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Rental_Car_Demo.Context;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.UserRepository;
using Rental_Car_Demo.Services;
using Rental_Car_Demo.ViewModel;
using System.Globalization;
using System.Text;
using System.Security.Cryptography;



namespace Rental_Car_Demo.Controllers
{
    public class UsersController : Controller
    {

        private readonly RentCarDbContext context;
        private readonly ICustomerContext _customerContext;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IEmailService _emailService;

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
                var user = context.Users.Where(x => x.Email.Equals(userLogin.User.Email)
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

                    return (bool)user.Role ? RedirectToAction("LoginOwn", "Users") : RedirectToAction("LoginCus", "Users");
                }

                if (user == null && !string.IsNullOrEmpty(userLogin.User.Email) && !string.IsNullOrEmpty(userLogin.User.Password))
                {
                    ViewBag.Error = "Either email address or password is incorrect. Please try again";

                }
            }
            TempData["ShowModal"] = "SignIn"; // Set flag to show sign-in modal
            return View("Guest", userLogin);
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
            var checkMail = IsEmailExist(model.Register.Email);

            // Kiểm tra email trùng lặp
            if (checkMail == true)
            {
                ModelState.AddModelError("Register.Email", "Email already existed. Please try another email.");
                return View("Guest", model);
            }

            if (model.Register.AgreeToTerms == false)
            {
                ModelState.AddModelError("Register.AgreeToTerms", "Please agree to this!");
                return View("Guest", model);
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
                return RedirectToAction("Guest", "Users");
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu xảy ra
                ModelState.AddModelError("", "An error occurred while creating the account: " + ex.Message);
            }
            //}

            // Nếu có lỗi, hiển thị lại form đăng ký với thông báo lỗi
            //TempData["ShowModal"] = "Register";
            return View("Guest", model);
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
                string subject = "Link Reset Password";
                _emailService.SendEmail(model.Email, subject, resetLink);
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

            token.IsLocked = true;
            context.Update(token);
            context.SaveChanges();

            return View(model);
        }

        [HttpPost]
        public IActionResult ResetPassword2(ResetPassword2ViewModel model)
        {
            if (ModelState.IsValid)
            {
                var customer = context.Users.FirstOrDefault(t => t.UserId == model.CustomerId);

                var hashPass = HashPassword(model.Password);
                customer.Password = hashPass;
                context.Update(customer);
                context.SaveChanges();

                TempData["SuccessMessage"] = "Your password has been reset";

                return View("Login");
            }
            return View(model);

        }

        public bool IsEmailExist(string email)
        {
            return context.Users.Any(u => u.Email == email);
        }



        // GET: UsersController/Edit/5
        public ActionResult Edit(int id)
        {
            //get user to block customer access this view
            var userString = HttpContext.Session.GetString("User");
            User userLogged = null;
            if (!string.IsNullOrEmpty(userString))
            {
                userLogged = JsonConvert.DeserializeObject<User>(userString);
            }
            if (userLogged.UserId != id)
            {
                return View("ErrorAuthorization");
            }

            var user = userDAO.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            var address = userDAO.GetAddressById(user.AddressId);

            if (address == null)
            {
                ViewBag.Cities = new SelectList(userDAO.GetCityList(), "CityId", "CityProvince");
                //ViewBag.Districts = new SelectList(userDAO.GetDistrictList(), "DistrictId", "DistrictName");
                //ViewBag.Wards = new SelectList(userDAO.GetWardList(), "WardId", "WardName");
            }
            else
            {
                var city = userDAO.GetCityList();
                var district = userDAO.GetDistrictListByCity(address.CityId);
                var ward = userDAO.GetWardListByDistrict(address.DistrictId);

                ViewBag.Cities = new SelectList(city, "CityId", "CityProvince", address.CityId);
                ViewBag.Districts = new SelectList(district, "DistrictId", "DistrictName", address.DistrictId);
                ViewBag.Wards = new SelectList(ward, "WardId", "WardName", address.WardId);
                ViewBag.Addresses = new SelectList(userDAO.GetAddress(), "AddressId", "HouseNumberStreet", user.AddressId);
            }

            return View(user);
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, User user, string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            try
            {
                if (id != user.UserId)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(NewPassword))
                {
                    user.Password = HashPassword(NewPassword);
                }
                else
                {
                    user.Password = userDAO.GetUserById(user.UserId).Password;
                }

                var userString = HttpContext.Session.GetString("User");
                User _user = null;
                if (!string.IsNullOrEmpty(userString))
                {
                    _user = JsonConvert.DeserializeObject<User>(userString);
                }

                var address = userDAO.GetAddressById(_user.AddressId);

                if (address == null)
                {
                    ViewBag.Cities = new SelectList(userDAO.GetCityList(), "CityId", "CityProvince");
                    ViewBag.Districts = new SelectList(userDAO.GetDistrictList(), "DistrictId", "DistrictName");
                    ViewBag.Wards = new SelectList(userDAO.GetWardList(), "WardId", "WardName");
                }
                else
                {
                    var city = userDAO.GetCityList();
                    var district = userDAO.GetDistrictListByCity(address.CityId);
                    var ward = userDAO.GetWardListByDistrict(address.DistrictId);

                    ViewBag.Cities = new SelectList(city, "CityId", "CityProvince", address.CityId);
                    ViewBag.Districts = new SelectList(district, "DistrictId", "DistrictName", address.DistrictId);
                    ViewBag.Wards = new SelectList(ward, "WardId", "WardName", address.WardId);
                    ViewBag.Addresses = new SelectList(userDAO.GetAddress(), "AddressId", "HouseNumberStreet", user.AddressId);
                }

                string errorMessage = userDAO.Edit(user);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    if (errorMessage.Contains("Email"))
                    {
                        ModelState.AddModelError("Email", errorMessage);
                    }
                    else
                    {
                        ModelState.AddModelError("", errorMessage);
                    }
                    return View(user);
                }

                //if (ModelState.IsValid)
                //{
                //    userDAO.Edit(user);
                //    return RedirectToAction("LoginCus", "Verify");
                //}

                var currentUser = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("User"));
                currentUser.Name = user.Name; // Assuming UserName is the property you want to update
                HttpContext.Session.SetString("User", JsonConvert.SerializeObject(currentUser));
                if (!String.IsNullOrEmpty(NewPassword))
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(user);
            }
        }



        [HttpGet]
        public JsonResult GetDistricts(int cityId)
        {
            var districts = userDAO.GetDistrictListByCity(cityId);
            return Json(districts);
        }

        [HttpGet]
        public JsonResult GetWards(int districtId)
        {
            var wards = userDAO.GetWardListByDistrict(districtId);
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