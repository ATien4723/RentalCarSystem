using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Rental_Car_Demo.Context;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.UserRepository;
using Rental_Car_Demo.Validation;
using Rental_Car_Demo.ViewModel;
using System.Globalization;
using System.Text;
using System.Security.Cryptography;



namespace Rental_Car_Demo.Controllers
{
    public class UsersController : Controller
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

                    return (bool)user.Role ? RedirectToAction("LoginOwn", "Users") : RedirectToAction("LoginCus", "Users");
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
            return RedirectToAction("Guest", "Users");
        }
        public IActionResult Guest()
        {
            return View();
        }






        RentCarDbContext context = new RentCarDbContext();
        CustomerContext customerContext = new CustomerContext();
        TokenGenerator tokenGenerator = new TokenGenerator();

        private readonly IEmailService _emailService;

        UserDAO userDAO;

        public UsersController(IEmailService emailService)
        {
            this._emailService = emailService;
            this.userDAO = new UserDAO();
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
                ModelState.AddModelError("Register.Email", "Email already existed. Please try another email.");
            }

            // Kiểm tra tính hợp lệ của ModelState
           // if (ModelState.IsValid)
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
                    return RedirectToAction("Login", "Users");
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi nếu xảy ra
                    ModelState.AddModelError("", "An error occurred while creating the account: " + ex.Message);
                }
           // }

            // Nếu có lỗi, hiển thị lại form đăng ký với thông báo lỗi
            return View("Login", model);
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

            string resetLink = Url.Action("ResetPassword2", "Users", new { customerId = customerId, tokenValue = tokenValue }, Request.Scheme);
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


                return View("Login");
            }
            return View("Fail");

        }



        public bool IsEmailExist(string email)
        {
            return context.Users.Any(u => u.Email == email);
        }

        // GET: UsersController
        public ActionResult Index()
        {
            var userList = userDAO.GetUserList();
            return View(userList);
        }

        // GET: UsersController/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = userDAO.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // GET: UsersController/Create
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    userDAO.Create(user);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(user);
            }
        }

        // GET: UsersController/Edit/5
        public ActionResult Edit(int id)
        {
            var user = userDAO.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            var address = userDAO.GetAddressById(user.AddressId);

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

            return View(user);
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, User user)
        {
            try
            {
                if (id != user.UserId)
                {
                    return NotFound();
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
                else if (ModelState.IsValid)
                {
                    userDAO.Edit(user);
                    return View();
                }
                return View(user);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(user);
            }
        }

        // GET: UsersController/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = userDAO.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                userDAO.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
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