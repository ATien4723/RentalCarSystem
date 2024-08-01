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
using System.ComponentModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Rental_Car_Demo.Services;
using Microsoft.CodeAnalysis;

namespace Rental_Car_Demo.Controllers
{
    public class BookingController : Controller
    {
        private readonly IEmailService _emailService;
        BookingDAO bookingDAO = null;
        CarDAO carDAO = null;
        UserDAO userDAO = null;
        RentCarDbContext _db = new RentCarDbContext();




        public IActionResult skipRating(DateTime? startDate, DateTime? endDate, int carId, int bookingNo)
        {

            Feedback feedback = new Feedback();
            feedback.BookingNo = bookingNo;
            feedback.Ratings = -1;
            feedback.Date = DateTime.Now;
            _db.Feedbacks.Add(feedback);
            _db.SaveChanges();
            ViewBag.checkFbExisted = true;
            return RedirectToAction("EditBookingDetail", new { startDate = startDate, endDate = endDate, carId = carId, bookingNo = bookingNo });
        }

        [HttpPost]
        public IActionResult giveRating(DateTime? startDate, DateTime? endDate, int carId, int bookingNo, string content, int ratings)
        {

            Feedback feedback = new Feedback();
            feedback.Content = content;
            feedback.Ratings = ratings;
            feedback.BookingNo = bookingNo;
            feedback.Date = DateTime.Now;
            _db.Feedbacks.Add(feedback);
            _db.SaveChanges();
            ViewBag.checkFbExisted = true;
            return RedirectToAction("EditBookingDetail", new { startDate = startDate, endDate = endDate, carId = carId, bookingNo = bookingNo });
        }

        public BookingController(IEmailService emailService)
        {
            this._emailService = emailService;
            bookingDAO = new BookingDAO();
            carDAO = new CarDAO();
            userDAO = new UserDAO();
        }

        // GET: BookingController/Create
        public ActionResult BookACar(string? location, DateTime startDate, DateTime endDate, int CarId)
        {
            try
            {
                var userString = HttpContext.Session.GetString("User");
                User user = null;
                if (!string.IsNullOrEmpty(userString))
                {
                    user = JsonConvert.DeserializeObject<User>(userString);
                }

                if (user == null)
                {
                    return RedirectToAction("Login", "Users");
                }

                //get user to block customer access this view
                if (user.Role == true)
                {
                    return View("ErrorAuthorization");
                }

                using var context = new RentCarDbContext();
                var car = context.Cars.Include(c => c.Address).ThenInclude(c => c.City).ThenInclude(c => c.Districts).ThenInclude(c => c.Wards).FirstOrDefault(x => x.CarId == CarId);

                if (car.Status != 1)
                {
                    return RedirectToAction("LoginCus", "Users");
                }

                if (string.IsNullOrEmpty(location))
                {
                    ViewBag.location = $"{car.Address.HouseNumberStreet}, {car?.Address?.Ward?.WardName}, {car?.Address?.District?.DistrictName}, {car?.Address?.City?.CityProvince}";
                }
                else
                {
                    ViewBag.location = location;
                }

                if (startDate.ToString("yyyy-MM-ddTHH:mm") != "0001-01-01T00:00")
                {
                    ViewBag.startDate = startDate;
                }
                else
                {
                    ViewBag.startDate = DateTime.Now;
                }

                if (endDate.ToString("yyyy-MM-ddTHH:mm") != "0001-01-01T00:00")
                {
                    ViewBag.endDate = endDate;
                }
                else
                {
                    ViewBag.endDate = DateTime.Now.AddDays(1);
                }
                ViewBag.carId = CarId;

                int numberOfDays;
                if (startDate.ToString("yyyy-MM-ddTHH:mm") == "0001-01-01T00:00" || endDate.ToString("yyyy-MM-ddTHH:mm") == "0001-01-01T00:00")
                {
                    numberOfDays = 1;
                }
                else
                {
                    numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);
                }
                ViewBag.NumberOfDays = numberOfDays;
                ViewBag.BasePrice = car.BasePrice;
                ViewBag.Total = numberOfDays * car.BasePrice;
                ViewBag.Deposit = numberOfDays * car.Deposit;

                bool checkRent = false;

                var lBooking = _db.Bookings.Where(x => x.CarId == CarId).ToList();

                var matchedFeedback = (from feedback in _db.Feedbacks.ToList()
                                       join booking in lBooking on feedback.BookingNo equals booking.BookingNo
                                       select feedback).ToList();

                double rating = 0, nor = 0;
                foreach (Feedback o in matchedFeedback)
                {
                    if (o.Ratings < 0)
                    {
                        continue;
                    }
                    rating += o.Ratings;
                    nor += 1;
                }

                if (nor > 0)
                {
                    rating = rating / nor;
                    rating = (Math.Ceiling(rating * 2)) / 2.0;
                }
                else
                {
                    rating = 0;
                }
                ViewBag.Rating = rating;

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

                var userW = userDAO.GetUserById(user.UserId);
                ViewBag.user = userW;
                ViewBag.userId = userW.UserId;
                ViewBag.wallet = userW.Wallet;
                ViewBag.dob = userW.Dob?.ToString("yyyy-MM-dd");
                ViewBag.DrivingLience = userW.DrivingLicense;
                var addressP = bookingDAO.GetAddressById(userW.AddressId);


                if (addressP == null)
                {
                    ViewBag.Cities = new SelectList(bookingDAO.GetCityList(), "CityId", "CityProvince");
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
        public ActionResult BookACar(string? location, DateTime startDate, DateTime endDate, int CarId, Booking viewModel)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
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
                    BookingInfoId = bookingInfo.BookingInfoId,
                };
                if (viewModel.PaymentMethod == 1)
                {
                    booking.Status = 2;
                }
                else
                {
                    booking.Status = 1;
                }

                _context.Bookings.Add(booking);
                _context.SaveChanges();

                var userString = HttpContext.Session.GetString("User");
                User user = null;
                if (!string.IsNullOrEmpty(userString))
                {
                    user = JsonConvert.DeserializeObject<User>(userString);
                }

                var car = _context.Cars.Include(c => c.User).FirstOrDefault(x => x.CarId == viewModel.CarId);
                var carOwner = _context.Users.FirstOrDefault(u => u.UserId == car.UserId);

                int numberOfDays = (int)Math.Ceiling((viewModel.EndDate - viewModel.StartDate).TotalDays);

                if (viewModel.PaymentMethod == 1 && user.Wallet < (car.Deposit * numberOfDays))
                {
                    TempData["AlertMessage"] = "You do not have enough money in your wallet to pay deposit!";
                    return RedirectToAction("BookACar", new { location = location, startDate = startDate, endDate = endDate, carId = CarId });
                }

                if (viewModel.PaymentMethod == 1)
                {
                    user.Wallet -= (numberOfDays * car.Deposit);
                    carOwner.Wallet += (numberOfDays * car.Deposit);
                    var _user = _context.Users.FirstOrDefault(x => x.UserId == user.UserId);
                    _user.Wallet -= (numberOfDays * car.Deposit);

                    var wallet = new Wallet
                    {
                        UserId = user.UserId,
                        Amount = (-(numberOfDays * car.Deposit)).ToString("N2"),
                        Type = "Pay Deposit",
                        CreatedAt = DateTime.Now,
                        BookingNo = booking.BookingNo,
                        CarName = car.Name
                    };
                    var walletCarOwner = new Wallet
                    {
                        UserId = car.User.UserId,
                        Amount = (numberOfDays * car.Deposit).ToString("N2"),
                        Type = "Receive Deposit",
                        CreatedAt = DateTime.Now,
                        BookingNo = booking.BookingNo,
                        CarName = car.Name
                    };
                    _context.Wallets.Add(wallet);
                    _context.Wallets.Add(walletCarOwner);
                    _context.SaveChanges();
                }

                car.Status = 2;
                _context.SaveChanges();

                var currentUser = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("User"));
                currentUser = JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(user));
                HttpContext.Session.SetString("User", JsonConvert.SerializeObject(currentUser));

                string email = car.User.Email;
                string subject = "Your car has been booked";
                string message = $"Congratulations! Your car {car.Name} has been booked at " +
                 $"{DateTime.Now:dd/MM/yyyy HH:mm}. Please go to your wallet to check " +
                 $"if the deposit has been paid and go to your car’s details page to confirm " +
                 $"the deposit. Thank you!";
                _emailService.SendEmail(email, subject, message);
                return RedirectToAction("BookACarFinish", new { carId = car.CarId, carName = car.Name, startDate = viewModel.StartDate, endDate = viewModel.EndDate, bookingNo = booking.BookingNo });
                //}
                //return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(viewModel);
            }
        }

        // GET: BookingController/Create
        public ActionResult BookACarFinish(int? carId, string? carName, DateTime? startDate, DateTime? endDate, int? bookingNo)
        {
            ViewBag.CarId = carId;
            ViewBag.CarName = carName;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.BookingNo = bookingNo;

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


        [HttpPost]
        public IActionResult confirmPickupForDetailsPage(DateTime? startDate, DateTime? endDate, int carId, int bookingNo, string sortOrder)
        {
            Booking booking = _db.Bookings.Find(bookingNo);
            booking.Status = 3;
            _db.Bookings.Update(booking);
            _db.SaveChanges();
            if (!startDate.HasValue || !endDate.HasValue)
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
                    .Include(b => b.Car)
                    .Where(b => b.UserId == userId)
                    .ToList();
                    ViewBag.SortOrder = "latest";
                }
                else if (sortOrder == "newest")
                {
                    ViewBag.Bookings = context.Bookings
                    .Include(b => b.Car)
                    .Where(b => b.UserId == userId)
                    .OrderByDescending(b => b.BookingNo)
                    .ToList();
                    ViewBag.SortOrder = "newest";
                }
                else if (sortOrder == "highest")
                {
                    ViewBag.Bookings = context.Bookings
                    .Include(b => b.Car)
                    .Where(b => b.UserId == userId)
                    .Select(b => new
                    {
                        Booking = b,
                        Car = b.Car,
                        CarId = b.Car.CarId,
                        StartDate = b.StartDate,
                        EndDate = b.EndDate,
                        BookingNo = b.BookingNo,
                        Status = b.Status,
                        Name = b.Car.Name,
                        BasePrice = b.Car.BasePrice,

                        TotalCost = EF.Functions.DateDiffDay(b.StartDate, b.EndDate) * b.Car.BasePrice
                    })
                    .OrderByDescending(b => b.TotalCost)
                    .ToList();
                    ViewBag.SortOrder = "highest";
                }
                else if (sortOrder == "lowest")
                {
                    ViewBag.Bookings = context.Bookings
                    .Include(b => b.Car)
                    .Where(b => b.UserId == userId)
                    .Select(b => new
                    {
                        Booking = b,
                        Car = b.Car,
                        CarId = b.Car.CarId,
                        StartDate = b.StartDate,
                        EndDate = b.EndDate,
                        BookingNo = b.BookingNo,
                        Status = b.Status,
                        Name = b.Car.Name,
                        BasePrice = b.Car.BasePrice,

                        TotalCost = EF.Functions.DateDiffDay(b.StartDate, b.EndDate) * b.Car.BasePrice
                    })
                    .OrderBy(b => b.TotalCost)
                    .ToList();
                    ViewBag.SortOrder = "lowest";
                }
                var bookingCount = context.Bookings
                .Where(b => b.UserId == userId && b.Status != 0 && b.Status != 5)
                .Count();

                ViewBag.Count = bookingCount;
                return View("ViewBookingList");
            }

            return RedirectToAction("EditBookingDetail", new { startDate = startDate, endDate = endDate, carId = carId, bookingNo = bookingNo });
        }

        public ActionResult EditBookingDetail(DateTime startDate, DateTime endDate, int carId, int bookingNo)
        {
            Boolean checkFbExisted = false;
            Feedback? feedback = _db.Feedbacks.FirstOrDefault(x => x.BookingNo == bookingNo);
            if (feedback != null)
            {
                checkFbExisted = true;
            }
            ViewBag.checkFbExisted = checkFbExisted;

            try
            {
                var booking = _db.Bookings.FirstOrDefault(b => b.BookingNo == bookingNo);

                var userString = HttpContext.Session.GetString("User");
                User user = null;
                if (!string.IsNullOrEmpty(userString))
                {
                    user = JsonConvert.DeserializeObject<User>(userString);
                }

                //get user to block customer access this view

                if (user.Role == true || booking.UserId != user.UserId)
                {
                    return View("ErrorAuthorization");
                }
                //
                using var context = new RentCarDbContext();
                var car = context.Cars.FirstOrDefault(x => x.CarId == carId);

                var bookingDetail = context.Bookings
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.RenterAddress)
                    .Include(b => b.BookingInfo)
                        .ThenInclude(bi => bi.DriverAddress)
                    .FirstOrDefault(b => b.BookingNo == bookingNo);


                ViewBag.RenterName = bookingDetail?.BookingInfo?.RenterName;

                ViewBag.bookingNo = bookingNo;
                //ViewBag.location = location;
                ViewBag.startDate = startDate;
                ViewBag.endDate = endDate;
                ViewBag.carId = carId;

                int numberOfDays = (int)Math.Ceiling((endDate - startDate).TotalDays);
                ViewBag.NumberOfDays = numberOfDays;
                ViewBag.BasePrice = car.BasePrice;
                ViewBag.Total = numberOfDays * car.BasePrice;
                ViewBag.Deposit = numberOfDays * car.Deposit;

                bool checkRent = false;
                if (bookingDetail.Status == 2 || bookingDetail.Status == 3 || bookingDetail.Status == 4)
                {
                    checkRent = true;
                }

                var lBooking = _db.Bookings.Where(x => x.CarId == carId).ToList();

                var matchedFeedback = (from feedbackEdit in _db.Feedbacks.ToList()
                                       join booking2 in lBooking on feedbackEdit.BookingNo equals booking.BookingNo
                                       select feedbackEdit).ToList();

                double rating = 0, nor = 0;
                foreach (Feedback o in matchedFeedback)
                {
                    if (o.Ratings < 0)
                    {
                        continue;
                    }
                    rating += o.Ratings;
                    nor += 1;
                }

                if (nor > 0)
                {
                    rating = rating / nor;
                    rating = (Math.Ceiling(rating * 2)) / 2.0;
                }
                else
                {
                    rating = 0;
                }
                ViewBag.Rating = rating;

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

                    if (viewModel != null)
                    {
                        var renterAddress = _context.Addresses.FirstOrDefault(a => a.AddressId == viewModel.BookingInfo.RenterAddressId);
                        //if (renterAddress == null)
                        //{
                        //    return RedirectToAction("EditBookingDetail");

                        //}
                        renterAddress.CityId = viewModel.BookingInfo.RenterAddress.CityId;
                        renterAddress.DistrictId = viewModel.BookingInfo.RenterAddress.DistrictId;
                        renterAddress.WardId = viewModel.BookingInfo.RenterAddress.WardId;
                        renterAddress.HouseNumberStreet = viewModel.BookingInfo.RenterAddress.HouseNumberStreet;

                        _context.Addresses.Update(renterAddress);
                        _context.SaveChanges();

                        var DriverAddress = _context.Addresses.FirstOrDefault(a => a.AddressId == viewModel.BookingInfo.DriverAddressId);
                        DriverAddress.CityId = viewModel.BookingInfo.DriverAddress.CityId;
                        DriverAddress.DistrictId = viewModel.BookingInfo.DriverAddress.DistrictId;
                        DriverAddress.WardId = viewModel.BookingInfo.DriverAddress.WardId;
                        DriverAddress.HouseNumberStreet = viewModel.BookingInfo.DriverAddress.HouseNumberStreet;

                        _context.Addresses.Update(DriverAddress);
                        _context.SaveChanges();

                        var bookingInfo = _context.BookingInfos.FirstOrDefault(b => b.BookingInfoId == viewModel.BookingInfoId);
                        bookingInfo.RenterEmail = viewModel.BookingInfo.RenterEmail;
                        bookingInfo.RenterName = viewModel.BookingInfo.RenterName;
                        bookingInfo.RenterDob = viewModel.BookingInfo.RenterDob;
                        bookingInfo.RenterNationalId = viewModel.BookingInfo.RenterNationalId;
                        bookingInfo.RenterPhone = viewModel.BookingInfo.RenterPhone;
                        bookingInfo.RenterAddressId = viewModel.BookingInfo.RenterAddressId;
                        bookingInfo.RenterDrivingLicense = viewModel.BookingInfo.RenterDrivingLicense;
                        bookingInfo.IsDifferent = viewModel.BookingInfo.IsDifferent;
                        bookingInfo.DriverEmail = viewModel.BookingInfo.DriverEmail;
                        bookingInfo.DriverName = viewModel.BookingInfo.DriverName;
                        bookingInfo.DriverDob = viewModel.BookingInfo.DriverDob;
                        bookingInfo.DriverNationalId = viewModel.BookingInfo.DriverNationalId;
                        bookingInfo.DriverPhone = viewModel.BookingInfo.DriverPhone;
                        bookingInfo.DriverAddressId = viewModel.BookingInfo.DriverAddressId;
                        bookingInfo.DriverDrivingLicense = viewModel.BookingInfo.DriverDrivingLicense;

                        _context.BookingInfos.Update(bookingInfo);
                        _context.SaveChanges();

                        var booking = _context.Bookings.FirstOrDefault(b => b.BookingNo == viewModel.BookingNo);
                        booking.UserId = viewModel.UserId;
                        booking.CarId = viewModel.CarId;
                        //booking.StartDate = viewModel.StartDate;
                        //booking.EndDate = viewModel.EndDate;
                        //booking.PaymentMethod = viewModel.PaymentMethod;
                        //booking.Status = viewModel.Status;
                        booking.BookingInfoId = bookingInfo.BookingInfoId;


                        _context.Bookings.Update(booking);
                        _context.SaveChanges();

                        return RedirectToAction("LoginCus", "Users");
                    }
                    return View(viewModel);
                }
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(viewModel);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelBooking(DateTime? startDate, DateTime? endDate, int? carId, int? BookingNo)
        {
            try
            {
                using RentCarDbContext _context = new RentCarDbContext();
                var booking = _context.Bookings.Include(b => b.Car).FirstOrDefault(b => b.BookingNo == BookingNo);
                if (booking != null)
                {
                    booking.Status = 0;
                    booking.Car.Status = 1;
                    _context.Update(booking);
                    _context.SaveChanges();

                    var userString = HttpContext.Session.GetString("User");
                    User user = null;
                    if (!string.IsNullOrEmpty(userString))
                    {
                        user = JsonConvert.DeserializeObject<User>(userString);
                    }

                    var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);
                    var carOwner = _context.Users.FirstOrDefault(u => u.UserId == car.UserId);

                    int numberOfDays = (int)Math.Ceiling((booking.EndDate - booking.StartDate).TotalDays);

                    if (booking.PaymentMethod == 1)
                    {
                        user.Wallet += (numberOfDays * car.Deposit);
                        carOwner.Wallet -= (numberOfDays * car.Deposit);
                        var _user = _context.Users.FirstOrDefault(x => x.UserId == user.UserId);
                        _user.Wallet += (numberOfDays * car.Deposit);

                        var wallet = new Wallet
                        {
                            UserId = user.UserId,
                            Amount = (numberOfDays * car.Deposit).ToString("N2"),
                            Type = "Refund Deposit",
                            CreatedAt = DateTime.Now,
                            BookingNo = booking.BookingNo,
                            CarName = car.Name
                        };
                        var walletCarOwner = new Wallet
                        {
                            UserId = car.User.UserId,
                            Amount = (-(numberOfDays * car.Deposit)).ToString("N2"),
                            Type = "Refund Deposit",
                            CreatedAt = DateTime.Now,
                            BookingNo = booking.BookingNo,
                            CarName = car.Name
                        };
                        _context.Wallets.Add(wallet);
                        _context.Wallets.Add(walletCarOwner);
                        _context.SaveChanges();
                    }
                    _context.SaveChanges();

                    var currentUser = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("User"));
                    currentUser = JsonConvert.DeserializeObject<User>(JsonConvert.SerializeObject(user));
                    HttpContext.Session.SetString("User", JsonConvert.SerializeObject(currentUser));

                    string email = car.User.Email;
                    string subject = "A booking with your car has been cancelled";
                    string message = $"Please be informed that a booking with your car {car.Name} has been cancelled at " +
                     $"{DateTime.Now:dd/MM/yyyy HH:mm}. The deposit will be returned to the customer's wallet " +
                     $"if the deposit has been paid and go to your car’s details page to confirm";
                    _emailService.SendEmail(email, subject, message);
                    if (endDate == null)
                    {
                        return RedirectToAction("ViewBookingList");
                    }
                    return RedirectToAction("EditBookingDetail", new { startDate = startDate, endDate = booking.EndDate, carId = carId, bookingNo = BookingNo });
                }

                return RedirectToAction("LoginCus", "Users");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("LoginCus", "Users");
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

            //get user to block customer access this view
            if (user.Role == true)
            {
                return View("ErrorAuthorization");
            }
            //

            var userId = user.UserId;

            var bookings = context.Bookings
            .Include(b => b.Car)
            .Include(b => b.User)
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
                .Include(b => b.Car)
                .Where(b => b.UserId == userId)
                .ToList();
                ViewBag.SortOrder = "latest";
            }
            else if (sortOrder == "newest")
            {
                ViewBag.Bookings = context.Bookings
                .Include(b => b.Car)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingNo)
                .ToList();
                ViewBag.SortOrder = "newest";
            }
            else if (sortOrder == "highest")
            {
                ViewBag.Bookings = context.Bookings
                .Include(b => b.Car)
                .Where(b => b.UserId == userId)
                .Select(b => new
                {
                    Booking = b,
                    Car = b.Car,
                    CarId = b.Car.CarId,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    BookingNo = b.BookingNo,
                    Status = b.Status,
                    Name = b.Car.Name,
                    BasePrice = b.Car.BasePrice,

                    TotalCost = EF.Functions.DateDiffDay(b.StartDate, b.EndDate) * b.Car.BasePrice
                })
                .OrderByDescending(b => b.TotalCost)
                .ToList();
                ViewBag.SortOrder = "highest";
            }
            else if (sortOrder == "lowest")
            {
                ViewBag.Bookings = context.Bookings
                .Include(b => b.Car)
                .Where(b => b.UserId == userId)
                .Select(b => new
                {
                    Booking = b,
                    Car = b.Car,
                    CarId = b.Car.CarId,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    BookingNo = b.BookingNo,
                    Status = b.Status,
                    Name = b.Car.Name,
                    BasePrice = b.Car.BasePrice,

                    TotalCost = EF.Functions.DateDiffDay(b.StartDate, b.EndDate) * b.Car.BasePrice
                })
                .OrderBy(b => b.TotalCost)
                .ToList();
                ViewBag.SortOrder = "lowest";
            }
            var bookingCount = context.Bookings
            .Where(b => b.UserId == userId && b.Status != 0 && b.Status != 5)
            .Count();

            ViewBag.Count = bookingCount;
            return View();
        }
    }
}
