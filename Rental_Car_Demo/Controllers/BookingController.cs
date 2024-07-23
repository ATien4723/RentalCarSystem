using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.BookingRepository;
using System.Security.Cryptography;
using System.Text;
using Rental_Car_Demo.Repository.CarRepository;
using Microsoft.EntityFrameworkCore;
using Rental_Car_Demo.Repository.UserRepository;

namespace Rental_Car_Demo.Controllers
{
    public class BookingController : Controller
    {
        BookingDAO bookingDAO = null;
        CarDAO carDAO = null;
        UserDAO userDAO = null;
        public BookingController()
        {
            bookingDAO = new BookingDAO();
            carDAO = new CarDAO();
            userDAO = new UserDAO();
        }

        // GET: BookingController
        public ActionResult Index()
        {
            var bookingList = bookingDAO.GetBookingList();
            return View();
        }

        // GET: BookingController/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = bookingDAO.GetBookingById(id);
            if (user == null)
            {
                return NotFound();
            }
            return View();
        }

        // GET: BookingController/Create
        public ActionResult BookACar(string location, DateTime startDate, DateTime endDate, int CarId)
        {
            try
            {
                //var Booking = new Booking();
                //var BookingInfo = new BookingInfo();

                //ViewBag.BookingInfo = BookingInfo;

                var userString = HttpContext.Session.GetString("User");
                User user = null;
                if (!string.IsNullOrEmpty(userString))
                {
                    user = JsonConvert.DeserializeObject<User>(userString);
                }

                using var context = new RentCarDbContext();
                var car = context.Cars.FirstOrDefault(x => x.CarId == CarId);

                ViewBag.location = location;
                ViewBag.startDate = startDate;
                ViewBag.endDate = endDate;
                ViewBag.carId = CarId;

                var numberOfDays = (endDate - startDate).TotalDays;
                ViewBag.NumberOfDays = endDate - startDate;
                ViewBag.BasePrice = car.BasePrice;
                ViewBag.Total = (decimal)numberOfDays * car.BasePrice;
                ViewBag.Deposit = car.Deposit;


                //List<Booking> lBook = context.Bookings.Where(x => x.CarId == CarId && x.UserId == user.UserId).ToList();
                bool checkRent = false;

                //foreach (Booking booking in lBook)
                //{
                //    if (booking.Status == 3 || booking.Status == 4)
                //    {
                //        checkRent = true;
                //        break;
                //    }
                //}
                //var lBooking = context.Bookings.Where(x => x.CarId == CarId).ToList();
                //float rating = 0;
                var brand = context.CarBrands.FirstOrDefault(x => x.BrandId == car.BrandId);
                var model = context.CarModels.FirstOrDefault(x => x.ModelId == car.ModelId);
                var document = context.CarDocuments.FirstOrDefault(x => x.DocumentId == car.DocumentId);
                var color = context.CarColors.FirstOrDefault(x => x.ColorId == car.ColorId);
                var address = context.Addresses.FirstOrDefault(x => x.AddressId == car.AddressId);
                var ward = context.Wards.FirstOrDefault(x => x.WardId == address.WardId);
                var district = context.Districts.FirstOrDefault(x => x.DistrictId == address.DistrictId);
                var city = context.Cities.FirstOrDefault(x => x.CityId == address.CityId);
                var term = context.TermOfUses.FirstOrDefault(x => x.TermId == car.TermId);
                var function = context.AdditionalFunctions.FirstOrDefault(x => x.FucntionId == car.FucntionId);
                ViewBag.car = car;
                ViewBag.brand = brand;
                ViewBag.model = model;
                ViewBag.document = document;
                ViewBag.color = color;
                ViewBag.address = address;
                ViewBag.ward = ward;
                ViewBag.district = district;
                ViewBag.city = city;
                ViewBag.term = term;
                ViewBag.function = function;
                ViewBag.checkRent = checkRent;

                ViewBag.user = user;
                ViewBag.userId = user.UserId;
                ViewBag.wallet = user.Wallet;
                ViewBag.dob = user.Dob?.ToString("yyyy-MM-dd");
                ViewBag.DrivingLience = user.DrivingLicense;
                var addressP = bookingDAO.GetAddressById(user.AddressId);


                if (addressP == null)
                {
                    ViewBag.Cities = new SelectList(bookingDAO.GetCityList(), "CityId", "CityProvince");
                    //ViewBag.Districts = new SelectList(bookingDAO.GetDistrictList(), "DistrictId", "DistrictName");
                    //ViewBag.Wards = new SelectList(bookingDAO.GetWardList(), "WardId", "WardName");
                }
                else
                {
                    var cityP = bookingDAO.GetCityList();
                    var districtP = bookingDAO.GetDistrictListByCity(addressP.CityId);
                    var wardP = bookingDAO.GetWardListByDistrict(addressP.DistrictId);

                    ViewBag.Cities = new SelectList(cityP, "CityId", "CityProvince", addressP.CityId);
                    ViewBag.Districts = new SelectList(districtP, "DistrictId", "DistrictName", addressP.DistrictId);
                    ViewBag.Wards = new SelectList(wardP, "WardId", "WardName", addressP.WardId);
                    ViewBag.houseNumberStreet = addressP.HouseNumberStreet;
                }

                ViewBag.UserId = new SelectList(userDAO.GetUserList(), "UserId", "Name", user.UserId);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

       //POST: BookingController/BookACar
       [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult BookACar(Booking viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using var _context = new RentCarDbContext();

                    var renterAddress = new Address
                    {
                        CityId = viewModel.BookingInfo.RenterAddress.CityId,
                        DistrictId = viewModel.BookingInfo.RenterAddress.DistrictId,
                        WardId = viewModel.BookingInfo.RenterAddress.WardId,
                        HouseNumberStreet = viewModel.BookingInfo.RenterAddress.HouseNumberStreet
                    };

                    _context.Addresses.Add(renterAddress);
                    _context.SaveChanges();

                    var DriverAddress = new Address
                    {
                        CityId = viewModel.BookingInfo.DriverAddress.CityId,
                        DistrictId = viewModel.BookingInfo.DriverAddress.DistrictId,
                        WardId = viewModel.BookingInfo.DriverAddress.WardId,
                        HouseNumberStreet = viewModel.BookingInfo.DriverAddress.HouseNumberStreet
                    };

                    _context.Addresses.Add(DriverAddress);
                    _context.SaveChanges();

                    var bookingInfo = new BookingInfo
                    {
                        RenterEmail = viewModel.BookingInfo.RenterEmail,
                        RenterName = viewModel.BookingInfo.RenterName,
                        RenterDob = viewModel.BookingInfo.RenterDob,
                        RenterNationalId = viewModel.BookingInfo.RenterNationalId,
                        RenterPhone = viewModel.BookingInfo.RenterPhone,
                        RenterAddressId = renterAddress.AddressId,
                        RenterDrivingLicense = viewModel.BookingInfo.RenterDrivingLicense,
                        IsDifferent = viewModel.BookingInfo.IsDifferent,
                        DriverEmail = viewModel.BookingInfo.DriverEmail,
                        DriverName = viewModel.BookingInfo.DriverName,
                        DriverDob = viewModel.BookingInfo.DriverDob,
                        DriverNationalId = viewModel.BookingInfo.DriverNationalId,
                        DriverPhone = viewModel.BookingInfo.DriverPhone,
                        DriverAddressId = DriverAddress.AddressId,
                        DriverDrivingLicense = viewModel.BookingInfo.DriverDrivingLicense
                    };

                    _context.BookingInfos.Add(bookingInfo);
                    _context.SaveChanges();

                    var booking = new Booking
                    {
                        UserId = viewModel.UserId,
                        CarId = viewModel.CarId,
                        StartDate = viewModel.StartDate,
                        EndDate = viewModel.EndDate,
                        PaymentMethod = viewModel.PaymentMethod,
                        Status = viewModel.Status,
                        BookingInfoId = bookingInfo.BookingInfoId
                    };
                    
                    
                    _context.Bookings.Add(booking);
                    _context.SaveChanges();

                    var userString = HttpContext.Session.GetString("User");
                    User user = null;
                    if (!string.IsNullOrEmpty(userString))
                    {
                        user = JsonConvert.DeserializeObject<User>(userString);
                    }

                    var car = _context.Cars.FirstOrDefault(x => x.CarId == viewModel.CarId);

                    if (viewModel.PaymentMethod == 1)
                    {
                        user.Wallet -= car.Deposit;
                        var _user = _context.Users.FirstOrDefault(x => x.UserId == user.UserId);
                        _user.Wallet -= car.Deposit;
                        _context.SaveChanges();
                    }

                    car.Status = 2;
                    _context.SaveChanges();
                    
                    var currentUser = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("User"));
                    currentUser = JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(user));
                    HttpContext.Session.SetString("User", JsonConvert.SerializeObject(currentUser));

                    TempData["CarId"] = car.CarId;
                    TempData["CarName"] = car.Name;
                    TempData["StartDate"] = viewModel.StartDate;
                    TempData["EndDate"] = viewModel.EndDate;
                    TempData["BookingNo"] = booking.BookingNo;

                    return RedirectToAction("BookACarFinish");
                }
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(viewModel);
            }
        }

        // GET: BookingController/Create
        public ActionResult BookACarFinish()
        {
            ViewBag.CarId = TempData["CarId"]?.ToString();
            ViewBag.CarName = TempData["CarName"]?.ToString();
            ViewBag.StartDate = TempData["StartDate"];
            ViewBag.EndDate = TempData["EndDate"];
            ViewBag.BookingNo = TempData["BookingNo"];

            return View();
        }


        public ActionResult EditBookingDetail(DateTime startDate, DateTime endDate, int carId, int bookingNo)
        {
            try
            {

                var userString = HttpContext.Session.GetString("User");
                User user = null;
                if (!string.IsNullOrEmpty(userString))
                {
                    user = JsonConvert.DeserializeObject<User>(userString);
                }

                using var context = new RentCarDbContext();
                var car = context.Cars.FirstOrDefault(x => x.CarId == carId);

                var bookingDetail = context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);


                ViewBag.RenterName = bookingDetail?.BookingInfo?.RenterName;


                //ViewBag.location = location;
                ViewBag.startDate = startDate;
                ViewBag.endDate = endDate;
                ViewBag.carId = carId;

                var numberOfDays = (endDate - startDate).TotalDays;
                ViewBag.NumberOfDays = endDate - startDate;
                ViewBag.BasePrice = car.BasePrice;
                ViewBag.Total = (decimal)numberOfDays * car.BasePrice;
                ViewBag.Deposit = car.Deposit;


                //List<Booking> lBook = context.Bookings.Where(x => x.CarId == CarId && x.UserId == user.UserId).ToList();
                bool checkRent = false;

                //foreach (Booking booking in lBook)
                //{
                //    if (booking.Status == 3 || booking.Status == 4)
                //    {
                //        checkRent = true;
                //        break;
                //    }
                //}
                //var lBooking = context.Bookings.Where(x => x.CarId == CarId).ToList();
                //float rating = 0;
                var brand = context.CarBrands.FirstOrDefault(x => x.BrandId == car.BrandId);
                var model = context.CarModels.FirstOrDefault(x => x.ModelId == car.ModelId);
                var document = context.CarDocuments.FirstOrDefault(x => x.DocumentId == car.DocumentId);
                var color = context.CarColors.FirstOrDefault(x => x.ColorId == car.ColorId);
                var address = context.Addresses.FirstOrDefault(x => x.AddressId == car.AddressId);
                var ward = context.Wards.FirstOrDefault(x => x.WardId == address.WardId);
                var district = context.Districts.FirstOrDefault(x => x.DistrictId == address.DistrictId);
                var city = context.Cities.FirstOrDefault(x => x.CityId == address.CityId);
                var term = context.TermOfUses.FirstOrDefault(x => x.TermId == car.TermId);
                var function = context.AdditionalFunctions.FirstOrDefault(x => x.FucntionId == car.FucntionId);
                ViewBag.car = car;
                ViewBag.brand = brand;
                ViewBag.model = model;
                ViewBag.document = document;
                ViewBag.color = color;
                ViewBag.address = address;
                ViewBag.ward = ward;
                ViewBag.district = district;
                ViewBag.city = city;
                ViewBag.term = term;
                ViewBag.function = function;
                ViewBag.checkRent = checkRent;

                ViewBag.user = user;
                ViewBag.userId = user.UserId;
                ViewBag.wallet = user.Wallet;
                ViewBag.dob = user.Dob?.ToString("yyyy-MM-dd");
                ViewBag.DrivingLience = user.DrivingLicense;
                var addressR = bookingDetail?.BookingInfo?.RenterAddress;

                if (addressR == null)
                {
                    ViewBag.CitiesR = new SelectList(bookingDAO.GetCityList(), "CityId", "CityProvince");
                    //ViewBag.Districts = new SelectList(bookingDAO.GetDistrictList(), "DistrictId", "DistrictName");
                    //ViewBag.Wards = new SelectList(bookingDAO.GetWardList(), "WardId", "WardName");
                }
                else
                {
                    var cityR = bookingDAO.GetCityList();
                    var districtR = bookingDAO.GetDistrictListByCity(addressR.CityId);
                    var wardR = bookingDAO.GetWardListByDistrict(addressR.DistrictId);

                    ViewBag.CitiesR = new SelectList(cityR, "CityId", "CityProvince", addressR.CityId);
                    ViewBag.DistrictsR = new SelectList(districtR, "DistrictId", "DistrictName", addressR.DistrictId);
                    ViewBag.WardsR = new SelectList(wardR, "WardId", "WardName", addressR.WardId);
                    ViewBag.houseNumberStreetR = addressR.HouseNumberStreet;
                }
                
                var addressD = bookingDetail?.BookingInfo?.DriverAddress;

                if (addressD == null)
                {
                    ViewBag.CitiesD = new SelectList(bookingDAO.GetCityList(), "CityId", "CityProvince");
                    //ViewBag.Districts = new SelectList(bookingDAO.GetDistrictList(), "DistrictId", "DistrictName");
                    //ViewBag.Wards = new SelectList(bookingDAO.GetWardList(), "WardId", "WardName");
                }
                else
                {
                    var cityD = bookingDAO.GetCityList();
                    var districtD = bookingDAO.GetDistrictListByCity(addressD.CityId);
                    var wardD = bookingDAO.GetWardListByDistrict(addressD.DistrictId);

                    ViewBag.CitiesD = new SelectList(cityD, "CityId", "CityProvince", addressD.CityId);
                    ViewBag.DistrictsD = new SelectList(districtD, "DistrictId", "DistrictName", addressD.DistrictId);
                    ViewBag.WardsD = new SelectList(wardD, "WardId", "WardName", addressD.WardId);
                    ViewBag.houseNumberStreetD = addressD.HouseNumberStreet;
                }

                ViewBag.UserId = new SelectList(userDAO.GetUserList(), "UserId", "Name", user.UserId);
                return View(bookingDetail);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        //POST: BookingController/BookACar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBookingDetail(Booking viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using var _context = new RentCarDbContext();

                    var renterAddress = new Address
                    {
                        CityId = viewModel.BookingInfo.RenterAddress.CityId,
                        DistrictId = viewModel.BookingInfo.RenterAddress.DistrictId,
                        WardId = viewModel.BookingInfo.RenterAddress.WardId,
                        HouseNumberStreet = viewModel.BookingInfo.RenterAddress.HouseNumberStreet
                    };

                    _context.Addresses.Add(renterAddress);
                    _context.SaveChanges();

                    var DriverAddress = new Address
                    {
                        CityId = viewModel.BookingInfo.DriverAddress.CityId,
                        DistrictId = viewModel.BookingInfo.DriverAddress.DistrictId,
                        WardId = viewModel.BookingInfo.DriverAddress.WardId,
                        HouseNumberStreet = viewModel.BookingInfo.DriverAddress.HouseNumberStreet
                    };

                    _context.Addresses.Add(DriverAddress);
                    _context.SaveChanges();

                    var bookingInfo = new BookingInfo
                    {
                        RenterEmail = viewModel.BookingInfo.RenterEmail,
                        RenterName = viewModel.BookingInfo.RenterName,
                        RenterDob = viewModel.BookingInfo.RenterDob,
                        RenterNationalId = viewModel.BookingInfo.RenterNationalId,
                        RenterPhone = viewModel.BookingInfo.RenterPhone,
                        RenterAddressId = renterAddress.AddressId,
                        RenterDrivingLicense = viewModel.BookingInfo.RenterDrivingLicense,
                        IsDifferent = viewModel.BookingInfo.IsDifferent,
                        DriverEmail = viewModel.BookingInfo.DriverEmail,
                        DriverName = viewModel.BookingInfo.DriverName,
                        DriverDob = viewModel.BookingInfo.DriverDob,
                        DriverNationalId = viewModel.BookingInfo.DriverNationalId,
                        DriverPhone = viewModel.BookingInfo.DriverPhone,
                        DriverAddressId = DriverAddress.AddressId,
                        DriverDrivingLicense = viewModel.BookingInfo.DriverDrivingLicense
                    };

                    _context.BookingInfos.Add(bookingInfo);
                    _context.SaveChanges();

                    var booking = new Booking
                    {
                        UserId = viewModel.UserId,
                        CarId = viewModel.CarId,
                        StartDate = viewModel.StartDate,
                        EndDate = viewModel.EndDate,
                        PaymentMethod = viewModel.PaymentMethod,
                        Status = viewModel.Status,
                        BookingInfoId = bookingInfo.BookingInfoId
                    };


                    _context.Bookings.Add(booking);
                    _context.SaveChanges();

                    var userString = HttpContext.Session.GetString("User");
                    User user = null;
                    if (!string.IsNullOrEmpty(userString))
                    {
                        user = JsonConvert.DeserializeObject<User>(userString);
                    }

                    var car = _context.Cars.FirstOrDefault(x => x.CarId == viewModel.CarId);

                    if (viewModel.PaymentMethod == 1)
                    {
                        user.Wallet -= car.Deposit;
                        var _user = _context.Users.FirstOrDefault(x => x.UserId == user.UserId);
                        _user.Wallet -= car.Deposit;
                        _context.SaveChanges();
                    }

                    car.Status = 2;
                    _context.SaveChanges();

                    var currentUser = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("User"));
                    currentUser = JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(user));
                    HttpContext.Session.SetString("User", JsonConvert.SerializeObject(currentUser));

                    TempData["CarName"] = car.Name;
                    TempData["StartDate"] = viewModel.StartDate;
                    TempData["EndDate"] = viewModel.EndDate;
                    TempData["BookingNo"] = booking.BookingNo;

                    return RedirectToAction("BookACarFinish");
                }
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(viewModel);
            }
        }


        // GET: BookingController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BookingController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BookingController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BookingController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public JsonResult GetDistricts(int cityId)
        {
            var districts = bookingDAO.GetDistrictListByCity(cityId);
            return Json(districts);
        }

        [HttpGet]
        public JsonResult GetWards(int districtId)
        {
            var wards = bookingDAO.GetWardListByDistrict(districtId);
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
        [HttpGet]
        public ActionResult ViewBookingList()
        {
            using var context = new RentCarDbContext();
            var userString = HttpContext.Session.GetString("User");
            User user = null;
            if (!string.IsNullOrEmpty(userString))
            {
                user = JsonConvert.DeserializeObject<User>(userString);
            }
            var userId = user.UserId;
            var bookings = context.Bookings
            .Include(b => b.Car) // Include the Car navigation property
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingNo)
            .ToList();

            var bookingCount = context.Bookings
            .Where(b => b.UserId == userId && b.Status != 0 && b.Status != 5)
            .Count();

            ViewBag.Bookings = bookings;
            ViewBag.Count = bookingCount;
            return View();
        }


        [HttpPost]
        public ActionResult ViewBookingList(string sortOrder)
        {
            var context = new RentCarDbContext();
            var userString = HttpContext.Session.GetString("User");
            User user = null;
            if (!string.IsNullOrEmpty(userString))
            {
                user = JsonConvert.DeserializeObject<User>(userString);
            }
            var userId = user.UserId;
            if (sortOrder == "latest")
            {
                ViewBag.Bookings = context.Bookings
            .Include(b => b.Car) // Include the Car navigation property
            .Where(b => b.UserId == userId)
            .ToList();
                ViewBag.SortOrder = "latest";
            }
            else
            {
                ViewBag.Bookings = context.Bookings
            .Include(b => b.Car) // Include the Car navigation property
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingNo)
            .ToList();
                ViewBag.SortOrder = "newest";
            }
            return View();
        }
    }
}
