using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.UserRepository;
using System.Security.Cryptography;
using System.Text;

namespace Rental_Car_Demo.Controllers
{
    public class UsersController : Controller
    {
        UserDAO userDAO = null;
        public UsersController() => userDAO = new UserDAO();

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
                    //string hashedCurrentPassword = HashPassword(CurrentPassword);

                    //// Compare hashedCurrentPassword with existingUser.Password
                    //if (hashedCurrentPassword != userDAO.GetUserById(user.UserId).Password)
                    //{
                    //    ModelState.AddModelError("CurrentPasswordError", "The current password is incorrect.");
                    //    return View(user);
                    //}
                    //if (NewPassword != ConfirmPassword)
                    //{
                    //    ModelState.AddModelError("confirmPasswordError", "The new password and confirmation password do not match.");
                    //    return View(user);
                    //}

                    user.Password = HashPassword(NewPassword);
                }
                else
                {
                    user.Password = userDAO.GetUserById(user.UserId).Password;
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
                return RedirectToAction("LoginCus", "Verify");
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
    }
}