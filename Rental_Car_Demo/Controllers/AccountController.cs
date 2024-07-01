using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Rental_Car_Demo.Context;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Validation;
using Rental_Car_Demo.ViewModel;
using System.Security.Cryptography;
using System.Text;

namespace Rental_Car_Demo.Controllers
{
    public class AccountController : Controller
    {
        RentCarDbContext context = new RentCarDbContext();
        CustomerContext customerContext = new CustomerContext();
        TokenGenerator tokenGenerator = new TokenGenerator();

        private readonly IEmailService _emailService;

        public AccountController(IEmailService emailService)
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
        public IActionResult Register(RegisterViewModel model)
        {
            // Kiểm tra email trùng lặp
            if (IsEmailExist(model.Email))
            {
                ModelState.AddModelError("Email", "Email already existed. Please try another email.");
            }

            // Kiểm tra tính hợp lệ của ModelState
            if (ModelState.IsValid)
            {
                // Hash mật khẩu
                var hashedPassword = HashPassword(model.Password);
                bool isCarOwner = model.Role == "carOwner";
                var customer = new User
                {
                    Email = model.Email,
                    Password = hashedPassword,
                    Name = model.Name,
                    Phone = model.Phone,
                    Role = isCarOwner
                };

                try
                {
                    // Thêm customer vào context và lưu thay đổi
                    context.Add(customer);
                    context.SaveChanges();
                    

                    // Hiển thị thông báo đăng ký thành công
                    TempData["SuccessMessage"] = "Account created successfully!";
                    return RedirectToAction("Register", "Account");
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi nếu xảy ra
                    ModelState.AddModelError("", "An error occurred while creating the account: " + ex.Message);
                }
            }

            // Nếu có lỗi, hiển thị lại form đăng ký với thông báo lỗi
            return View("Register");
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

            var token = new TokenInfor()
            {
                Token = tokenValue,
                UserId = customerContext.getCustomerIdByEmail(model.Email),
                ExpirationTime = exTime,
                IsLocked = false
            };

             context.Add(token);
             context.SaveChanges();

            int? customerId = token.UserId;

            string resetLink = Url.Action("ResetPassword2", "Account", new { customerId = customerId, tokenValue = tokenValue }, Request.Scheme);
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

                if (token ==null || token.IsLocked == true || token.ExpirationTime < DateTime.Now)
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


            private string HashPassword(string password)
            {
                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                    return hash;
                }

            }

            public bool IsEmailExist(string email)
            {
                return context.Users.Any(u => u.Email == email);
            }

        
    } 
}
