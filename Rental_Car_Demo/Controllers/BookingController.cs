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
using Microsoft.CodeAnalysis;
using Rental_Car_Demo.Services;

namespace Rental_Car_Demo.Controllers
{
    public class BookingController : Controller
    {
        private readonly IEmailService _emailService;
        BookingDAO bookingDAO = null;
        CarDAO carDAO = null;
        UserDAO userDAO = null;
        RentCarDbContext _db = new RentCarDbContext();
        private readonly RentCarDbContext context;

        public IActionResult skipRating(DateTime? startDate, DateTime? endDate, int carId, int bookingNo)
        {
            var carExists = _db.Cars.Any(c => c.CarId == carId);

            if (!carExists)
            {
                return NotFound($"Car with ID {carId} not found.");
            }

            var bookingExists = _db.Bookings.Any(b => b.BookingNo == bookingNo);
            if (!bookingExists)
            {
                return NotFound($"Booking with number {bookingNo} not found.");
            }

            if (startDate == null || endDate == null)
            {
                return NotFound("Need DateTime!");
            }

            if (startDate.HasValue && endDate.HasValue && startDate >= endDate)
            {
                return NotFound("DateTime invalid!");
            }

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
            var carExists = _db.Cars.Any(c => c.CarId == carId);

            if (!carExists)
            {
                return NotFound($"Car with ID {carId} not found.");
            }

            var bookingExists = _db.Bookings.Any(b => b.BookingNo == bookingNo);
            if (!bookingExists)
            {
                return NotFound($"Booking with number {bookingNo} not found.");
            }

            if (startDate == null || endDate == null)
            {
                return NotFound("Need DateTime!");
            }


            if (startDate.HasValue && endDate.HasValue && startDate >= endDate)
            {
                return NotFound("DateTime invalid!");
            }


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

        public BookingController(IEmailService emailService, RentCarDbContext _context)
        {
            this._emailService = emailService;
            bookingDAO = new BookingDAO();
            carDAO = new CarDAO(context);
            userDAO = new UserDAO();
            context = _context;
        }

        // GET: BookingController/Create
        public ActionResult BookACar(string? location, DateTime startDate, DateTime endDate, int CarId)
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

            if (user.Role == true)
            {
                return View("ErrorAuthorization");
            }
            else
            {
                var car = _context.Cars.Include(c => c.Address)
                    .ThenInclude(c => c.City)
                    .ThenInclude(c => c.Districts)
                    .ThenInclude(c => c.Wards).FirstOrDefault(x => x.CarId == CarId);

                if (car == null)
                {
                    return View("ErrorAuthorization");
                }

                if (car.Status != 1)
                {
                    return View("ErrorAuthorization");
                }

                if (string.IsNullOrEmpty(location))
                {
                    ViewBag.location = $"{car?.Address?.District?.DistrictName}, {car?.Address?.City?.CityProvince}";
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
                var totalHours = (int)Math.Ceiling((endDate - startDate).TotalHours);
                ViewBag.TotalHours = totalHours;
                if (totalHours < 12)
                {
                    ViewBag.Total = (decimal) car.BasePrice * 0.5m;
                    ViewBag.Deposit = (decimal) car.Deposit * 0.5m;
                }
                else
                {
                    ViewBag.Total = numberOfDays * car.BasePrice;
                    ViewBag.Deposit = numberOfDays * car.Deposit;
                }
                ViewBag.NumberOfDays = numberOfDays;
                ViewBag.BasePrice = car.BasePrice;

                bool checkRent = false;

                var lBooking = _context.Bookings.Where(x => x.CarId == CarId).ToList();

                var matchedFeedback = (from feedback in _context.Feedbacks.ToList()
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

                var brand = _context.CarBrands.FirstOrDefault(x => x.BrandId == car.BrandId);
                var model = _context.CarModels.FirstOrDefault(x => x.ModelId == car.ModelId);
                var document = _context.CarDocuments.FirstOrDefault(x => x.DocumentId == car.DocumentId);
                var color = _context.CarColors.FirstOrDefault(x => x.ColorId == car.ColorId);
                var address = _context.Addresses.FirstOrDefault(x => x.AddressId == car.AddressId);
                var ward = _context.Wards.FirstOrDefault(x => x.WardId == address.WardId);
                var district = _context.Districts.FirstOrDefault(x => x.DistrictId == address.DistrictId);
                var city = _context.Cities.FirstOrDefault(x => x.CityId == address.CityId);
                var term = _context.TermOfUses.FirstOrDefault(x => x.TermId == car.TermId);
                var function = _context.AdditionalFunctions.FirstOrDefault(x => x.FucntionId == car.FucntionId);
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

                var userW = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
                ViewBag.user = userW;
                ViewBag.userId = userW.UserId;
                ViewBag.wallet = userW.Wallet;
                ViewBag.dob = userW.Dob?.ToString("yyyy-MM-dd");
                ViewBag.DrivingLience = userW.DrivingLicense;

                var addressP = _context.Addresses.FirstOrDefault(a=>a.AddressId==userW.AddressId);


                if (addressP == null)
                {
                    ViewBag.Cities = new SelectList(_context.Cities.ToList(), "CityId", "CityProvince");
                }
                else
                {
                    var cityP = _context.Cities.ToList();
                    var districtP = _context.Districts.Where(a => a.CityId == addressP.CityId).ToList();
                    var wardP = _context.Wards.Where(a => a.DistrictId == addressP.DistrictId).ToList();

                    ViewBag.Cities = new SelectList(cityP, "CityId", "CityProvince", addressP.CityId);
                    ViewBag.Districts = new SelectList(districtP, "DistrictId", "DistrictName", addressP.DistrictId);
                    ViewBag.Wards = new SelectList(wardP, "WardId", "WardName", addressP.WardId);
                    ViewBag.houseNumberStreet = addressP.HouseNumberStreet;
                }
                return View("BookACar");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BookACar(string? location, DateTime startDate, DateTime endDate, int CarId, Booking viewModel)
        {
            var userString = HttpContext.Session.GetString("User");
            User user = null;
            if (!string.IsNullOrEmpty(userString))
            {
                user = JsonConvert.DeserializeObject<User>(userString);
            }

            var userCheckWallet = _context.Users.SingleOrDefault(u => u.UserId == user.UserId);
            var car = _context.Cars.Include(c => c.User).FirstOrDefault(x => x.CarId == viewModel.CarId);
            var carOwner = _context.Users.FirstOrDefault(u => u.UserId == car.UserId);
            int numberOfDays = (int)Math.Ceiling((viewModel.EndDate - viewModel.StartDate).TotalDays);

            var checkTimeRent = (int)Math.Ceiling((endDate - startDate).TotalHours);
            if (checkTimeRent < 12)
            {
                if (viewModel.PaymentMethod == 1 && (userCheckWallet.Wallet * 2) < (car.Deposit * numberOfDays))
                {
                    ModelState.AddModelError("PaymentMethod", "You do not have enough money in your wallet to pay deposit!");
                    //TempData["AlertMessage"] = "You do not have enough money in your wallet to pay deposit!";
                    return RedirectToAction("BookACar", new { location = location, startDate = startDate, endDate = endDate, carId = CarId });
                }
            }
            else
            {
                if (viewModel.PaymentMethod == 1 && userCheckWallet.Wallet < (car.Deposit * numberOfDays))
                {
                    ModelState.AddModelError("PaymentMethod", "You do not have enough money in your wallet to pay deposit!");
                    //TempData["AlertMessage"] = "You do not have enough money in your wallet to pay deposit!";
                    return RedirectToAction("BookACar", new { location = location, startDate = startDate, endDate = endDate, carId = CarId });
                }
            }

            

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


            if (viewModel.PaymentMethod == 1)
            {
                var totalHours = (int)Math.Ceiling((endDate - startDate).TotalHours);
                if (totalHours < 12)
                {
                    user.Wallet -= (0.5m * car.Deposit);
                    carOwner.Wallet += (0.5m * car.Deposit);
                    var _user = _context.Users.FirstOrDefault(x => x.UserId == user.UserId);
                    _user.Wallet -= (0.5m * car.Deposit);

                    var wallet = new Wallet
                    {
                        UserId = user.UserId,
                        Amount = (-(0.5m * car.Deposit)).ToString("N2"),
                        Type = "Pay Deposit",
                        CreatedAt = DateTime.Now,
                        BookingNo = booking.BookingNo,
                        CarName = car.Name
                    };
                    var walletCarOwner = new Wallet
                    {
                        UserId = car.User.UserId,
                        Amount = (0.5m * car.Deposit).ToString("N2"),
                        Type = "Receive Deposit",
                        CreatedAt = DateTime.Now,
                        BookingNo = booking.BookingNo,
                        CarName = car.Name
                    };
                    _context.Wallets.Add(wallet);
                    _context.Wallets.Add(walletCarOwner);
                    _context.SaveChanges();
                }
                else
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
                             $"if the deposit has been paid and go to your cars details page to confirm " +
                             $"the deposit. Thank you!";
            _emailService.SendEmail(email, subject, message);

            return RedirectToAction("BookACarFinish", new { carId = car.CarId, carName = car.Name, startDate = viewModel.StartDate, endDate = viewModel.EndDate, bookingNo = booking.BookingNo });
        }

        // GET: BookingController/Create
        public ActionResult BookACarFinish(int? carId, string? carName, DateTime? startDate, DateTime? endDate, int? bookingNo)
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
            if (user.Role == true)
            {
                return View("ErrorAuthorization");
            }

            ViewBag.CarId = carId;
            ViewBag.CarName = carName;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.BookingNo = bookingNo;
            return View();
        }


        [HttpPost]
        public IActionResult confirmPickupForDetailsPage(DateTime? startDate, DateTime? endDate, int carId, int bookingNo,string sortOrder)
        {

            var context = new RentCarDbContext();
            var userString = HttpContext.Session.GetString("User");
            User user = null;
            if (!string.IsNullOrEmpty(userString))
            {
                user = JsonConvert.DeserializeObject<User>(userString);
            }

            var userId = user.UserId;

            if (user.Role == true)
            {
                return View("ErrorAuthorization");
            }

            Booking booking = _db.Bookings.Find(bookingNo);

            if (booking == null)
            {
                return NotFound($"Cannot find bookingNo {bookingNo} !");
            }

            booking.Status = 3;
            _db.Bookings.Update(booking);
            _db.SaveChanges();
           
            if (!startDate.HasValue || !endDate.HasValue)
            {
               
                if (sortOrder != "latest" && sortOrder != "newest" && sortOrder != "highest" && sortOrder != "lowest")
                {
                    return NotFound($"Can not find sort order {sortOrder} !");
                }


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

            var car = _db.Cars.Find(carId);
            if (car == null)
            {
                return NotFound($"Cannot find car with Id = {carId} !");
            }

            if (startDate.HasValue && endDate.HasValue && startDate >= endDate)
            {
                return NotFound("DateTime invalid!");

            }

            return RedirectToAction("EditBookingDetail", new { startDate = startDate, endDate = endDate, carId = carId, bookingNo = bookingNo });
        }

        public ActionResult EditBookingDetail(DateTime startDate, DateTime endDate, int carId, int bookingNo)
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
            var booking = _context.Bookings
                .Include(b => b.BookingInfo)
                    .ThenInclude(bi => bi.RenterAddress)
                .Include(b => b.BookingInfo)
                    .ThenInclude(bi => bi.DriverAddress)
                .FirstOrDefault(b => b.BookingNo == bookingNo);

            //using var context = new RentCarDbContext();
            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);
            if (user.Role == true || booking.UserId != user.UserId || booking.CarId != carId)
            {
                return View("ErrorAuthorization");
            }
            //

            Boolean checkFbExisted = false;
            Feedback? feedback = _context.Feedbacks.FirstOrDefault(x => x.BookingNo == bookingNo);
            if (feedback != null)
            {
                checkFbExisted = true;
            }
            ViewBag.checkFbExisted = checkFbExisted;

            var bookingDetail = _context.Bookings
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
            var totalHours = (int)Math.Ceiling((endDate - startDate).TotalHours);
            ViewBag.TotalHours = totalHours;

            ViewBag.NumberOfDays = numberOfDays;
            ViewBag.BasePrice = car.BasePrice;

            if (totalHours < 12)
            {
                ViewBag.Total = (decimal)car.BasePrice * 0.5m;
                ViewBag.Deposit = (decimal)car.Deposit * 0.5m;
            }
            else
            {
                ViewBag.Total = numberOfDays * car.BasePrice;
                ViewBag.Deposit = numberOfDays * car.Deposit;
            }

            

            bool checkRent = false; 
            if (bookingDetail.Status == 2 || bookingDetail.Status == 3 || bookingDetail.Status == 4)
            {
                checkRent = true;
            }

            var lBooking = _context.Bookings.Where(x => x.CarId == carId).ToList();

            var matchedFeedback = (from feedbackEdit in _context.Feedbacks.ToList()
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

            var brand = _context.CarBrands.FirstOrDefault(x => x.BrandId == car.BrandId);
            var model = _context.CarModels.FirstOrDefault(x => x.ModelId == car.ModelId);
            var document = _context.CarDocuments.FirstOrDefault(x => x.DocumentId == car.DocumentId);
            var color = _context.CarColors.FirstOrDefault(x => x.ColorId == car.ColorId);
            var address = _context.Addresses.FirstOrDefault(x => x.AddressId == car.AddressId);
            var ward = _context.Wards.FirstOrDefault(x => x.WardId == address.WardId);
            var district = _context.Districts.FirstOrDefault(x => x.DistrictId == address.DistrictId);
            var city = _context.Cities.FirstOrDefault(x => x.CityId == address.CityId);
            var term = _context.TermOfUses.FirstOrDefault(x => x.TermId == car.TermId);
            var function = _context.AdditionalFunctions.FirstOrDefault(x => x.FucntionId == car.FucntionId);
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
                ViewBag.CitiesR = new SelectList(_context.Cities.ToList(), "CityId", "CityProvince");
            }
            else
            {
                var cityR = _context.Cities.ToList();
                var districtR = _context.Districts.Where(d => d.CityId == addressR.CityId).ToList();
                var wardR = _context.Wards.Where(d => d.DistrictId == addressR.DistrictId).ToList();

                ViewBag.CitiesR = new SelectList(cityR, "CityId", "CityProvince", addressR.CityId);
                ViewBag.DistrictsR = new SelectList(districtR, "DistrictId", "DistrictName", addressR.DistrictId);
                ViewBag.WardsR = new SelectList(wardR, "WardId", "WardName", addressR.WardId);
                ViewBag.houseNumberStreetR = addressR.HouseNumberStreet;
            }

            var addressD = bookingDetail?.BookingInfo?.DriverAddress;

            if (addressD == null)
            {
                ViewBag.CitiesD = new SelectList(_context.Cities.ToList(), "CityId", "CityProvince");
            }
            else
            {
                var cityD = _context.Cities.ToList();
                var districtD = _context.Districts.Where(d => d.CityId == addressD.CityId).ToList();
                var wardD = _context.Wards.Where(d => d.DistrictId == addressD.DistrictId).ToList();

                ViewBag.CitiesD = new SelectList(cityD, "CityId", "CityProvince", addressD.CityId);
                ViewBag.DistrictsD = new SelectList(districtD, "DistrictId", "DistrictName", addressD.DistrictId);
                ViewBag.WardsD = new SelectList(wardD, "WardId", "WardName", addressD.WardId);
                ViewBag.houseNumberStreetD = addressD.HouseNumberStreet;
            }
            return View(bookingDetail);
        }

        //POST: BookingController/BookACar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBookingDetail(Booking viewModel)
        {
            var renterAddress = _context.Addresses.FirstOrDefault(a => a.AddressId == viewModel.BookingInfo.RenterAddressId);

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
            booking.BookingInfoId = bookingInfo.BookingInfoId;

            _context.Bookings.Update(booking);
            _context.SaveChanges();

            return RedirectToAction("EditBookingDetail", new { startDate = booking.StartDate, endDate = booking.EndDate, carId = booking.CarId, bookingNo = booking.BookingNo });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelBooking(DateTime? startDate, DateTime? endDate, int? carId, int? BookingNo)
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

            Booking booking = null;
            booking = _context.Bookings.Include(b => b.Car).FirstOrDefault(b => b.BookingNo == BookingNo);

            booking.Status = 0;
            booking.Car.Status = 1;
            _context.Update(booking);
            _context.SaveChanges();

            var car = _context.Cars.FirstOrDefault(x => x.CarId == booking.CarId);
            var carOwner = _context.Users.FirstOrDefault(u => u.UserId == car.UserId);

            int numberOfDays = (int)Math.Ceiling((booking.EndDate - booking.StartDate).TotalDays);

            if (booking.PaymentMethod == 1)
            {

                var totalHours = (int)Math.Ceiling((booking.EndDate - booking.StartDate).TotalHours);
                if (totalHours < 12)
                {
                  carOwner.Wallet -= (0.5m * car.Deposit);
                    var _user = _context.Users.FirstOrDefault(x => x.UserId == user.UserId);
                    _user.Wallet += (0.5m * car.Deposit);

                    var wallet = new Wallet
                    {
                        UserId = user.UserId,
                        Amount = (0.5m * car.Deposit).ToString("N2"),
                        Type = "Refund Deposit",
                        CreatedAt = DateTime.Now,
                        BookingNo = booking.BookingNo,
                        CarName = car.Name
                    };
                    var walletCarOwner = new Wallet
                    {
                        UserId = car.User.UserId,
                        Amount = (-(0.5m * car.Deposit)).ToString("N2"),
                        Type = "Refund Deposit",
                        CreatedAt = DateTime.Now,
                        BookingNo = booking.BookingNo,
                        CarName = car.Name
                    };
                    _context.Wallets.Add(wallet);
                    _context.Wallets.Add(walletCarOwner);
                    _context.SaveChanges();
                }
                else
                {
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
            }
            _context.SaveChanges();

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
            return RedirectToAction("EditBookingDetail", new { startDate = startDate, endDate = booking.EndDate, carId = booking.CarId, bookingNo = BookingNo });
        }



        [HttpGet]
        public JsonResult GetDistricts(int cityId)
        {
            var districts = _context.Districts.Where(d => d.CityId == cityId);
            return Json(districts);
        }

        [HttpGet]
        public JsonResult GetWards(int districtId)
        {
            var wards = _context.Wards.Where(w => w.DistrictId == districtId);
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
            var userString = HttpContext.Session.GetString("User");
            User user = null;
            if (!string.IsNullOrEmpty(userString))
            {
                user = JsonConvert.DeserializeObject<User>(userString);
            }
            else
            {
                return RedirectToAction("Login", "Users");
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
            var userString = HttpContext.Session.GetString("User");
            User user = null;
            if (!string.IsNullOrEmpty(userString))
            {
                user = JsonConvert.DeserializeObject<User>(userString);
            }
            var userId = user.UserId;

            var bookingsQuery = context.Bookings
                .Include(b => b.Car)
                .Where(b => b.UserId == userId)
                .AsQueryable();

            if (sortOrder == "latest")
            {
                ViewBag.SortOrder = "latest";
            }
            else if (sortOrder == "newest")
            {
                bookingsQuery = bookingsQuery.OrderByDescending(b => b.BookingNo);
                ViewBag.SortOrder = "newest";
            }
            else if (sortOrder == "highest")
            {
                bookingsQuery = bookingsQuery
                    .AsEnumerable()
                    .Select(b => new
                    {
                        Booking = b,
                        TotalCost = (b.EndDate - b.StartDate).Days * b.Car.BasePrice
                    })
                    .OrderByDescending(b => b.TotalCost)
                    .Select(b => b.Booking)
                    .AsQueryable();
                ViewBag.SortOrder = "highest";
            }
            else if (sortOrder == "lowest")
            {
                bookingsQuery = bookingsQuery
                    .AsEnumerable()
                    .Select(b => new
                    {
                        Booking = b,
                        TotalCost = (b.EndDate - b.StartDate).Days * b.Car.BasePrice
                    })
                    .OrderBy(b => b.TotalCost)
                    .Select(b => b.Booking)
                    .AsQueryable();
                ViewBag.SortOrder = "lowest";
            }

            var bookings = bookingsQuery.ToList();

            var bookingCount = context.Bookings
                .Where(b => b.UserId == userId && b.Status != 0 && b.Status != 5)
                .Count();

            ViewBag.Bookings = bookings;
            ViewBag.Count = bookingCount;

            return View();
        }

    }
}
