using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.BookingRepository;
using Rental_Car_Demo.Repository.CarRepository;
using Rental_Car_Demo.Validation;

namespace Rental_Car_Demo.Controllers
{
    public class CarController : Controller
    {
        ICarRepository carRepository = null;

        RentCarDbContext _db = new RentCarDbContext();
        public CarController(ICarRepository carRepository, RentCarDbContext db, IEmailService emailService)
        {
            this.carRepository = carRepository;
            _db = db;
            _emailService = emailService;
        }

        public IActionResult tet1(int CarId, string? location, DateTime? startDate, DateTime? endDate)
        {
            ViewBag.location = location;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            var car = _db.Cars.FirstOrDefault(x => x.CarId == CarId);
            ViewBag.CarOwner = _db.Users.FirstOrDefault(x => x.UserId == car.UserId);
            var userJson = HttpContext.Session.GetString("User");
            bool checkRent = false;
            if (!string.IsNullOrEmpty(userJson))
            {
                var user = JsonConvert.DeserializeObject<User>(userJson);
                List<Booking> lBook = _db.Bookings.Where(x => x.CarId == CarId && x.UserId == user.UserId).ToList();
                foreach (Booking booking in lBook)
                {
                    if (booking.Status == 3 || booking.Status == 4)
                    {
                        checkRent = true;
                        break;
                    }
                }

            }
            var matchedFeedback = (from feedback in _db.Feedbacks
                                   join booking in _db.Bookings on feedback.BookingNo equals booking.BookingNo
                                   join user in _db.Users on booking.UserId equals user.UserId
                                   where booking.CarId == CarId && feedback.Ratings > 0
                                   select new
                                   {
                                       feedback.FeedbackId,
                                       feedback.BookingNo,
                                       feedback.Ratings,
                                       feedback.Content,
                                       feedback.Date,
                                       user.Name,
                                   }).ToList();

            ViewBag.matchedFeedback = matchedFeedback.OrderByDescending(x => x.Date).ToList();



            double rating = 0, nor = 0;
            foreach (var o in matchedFeedback)
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

            ViewBag.matchedFeedback = matchedFeedback.OrderByDescending(x=> x.Date);
            ViewBag.Rating = rating;
            var brand = _db.CarBrands.FirstOrDefault(x => x.BrandId == car.BrandId);
            var model = _db.CarModels.FirstOrDefault(x => x.ModelId == car.ModelId);
            var document = _db.CarDocuments.FirstOrDefault(x => x.DocumentId == car.DocumentId);
            var color = _db.CarColors.FirstOrDefault(x => x.ColorId == car.ColorId);
            var address = _db.Addresses.FirstOrDefault(x => x.AddressId == car.AddressId);
            var ward = _db.Wards.FirstOrDefault(x => x.WardId == address.WardId);
            var district = _db.Districts.FirstOrDefault(x => x.DistrictId == address.DistrictId);
            var city = _db.Cities.FirstOrDefault(x => x.CityId == address.CityId);
            var term = _db.TermOfUses.FirstOrDefault(x => x.TermId == car.TermId);
            var function = _db.AdditionalFunctions.FirstOrDefault(x => x.FucntionId == car.FucntionId);
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
            return View();
        }
        public ActionResult AddACar()
        {
            var context = new RentCarDbContext();
            List<int> year = new List<int>();
            for (int i = 1990; i <= 2030; i++)
            {
                year.Add(i);
            }
            ViewBag.LPList = context.Cars.Select(x => x.LicensePlate).ToList();
            ViewBag.Color = context.CarColors.ToList();
            ViewBag.Brand = context.CarBrands.ToList();
            ViewBag.Model = context.CarModels.ToList();
            ViewBag.City = context.Cities.ToList();
            ViewBag.District = context.Districts.ToList();
            ViewBag.Ward = context.Wards.ToList();
            ViewBag.Year = year;


            //get user to block customer access this view
            var userString = HttpContext.Session.GetString("User");
            User user = null;
            if (!string.IsNullOrEmpty(userString))
            {
                user = JsonConvert.DeserializeObject<User>(userString);
            }
            if(user.Role == false)
            {
                return View("ErrorAuthorization");
            }


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddACarAsync(Car car, IFormFile registration, IFormFile certificate, IFormFile insurance,
            IFormFile front, IFormFile back, IFormFile left, IFormFile right,
            bool Bluetooth, bool GPS, bool Camera, bool Sunroof, bool Childlock, bool Childseat, bool DVD, bool USB,
            bool smoking, bool food, bool pet, string specify, int city, int district, int ward, string street)
        {

            var document = new CarDocument();

            if (registration != null)
            {
                var fileNameRegistration = Path.GetFileName(registration.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileNameRegistration);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await registration.CopyToAsync(stream);
                }
                document.Registration = fileNameRegistration;
            }
            if (certificate != null)
            {
                var fileNameCertificate = Path.GetFileName(certificate.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileNameCertificate);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await certificate.CopyToAsync(stream);
                }
                document.Certificate = fileNameCertificate;
            }

            if (insurance != null)
            {
                var fileNameInsurance = Path.GetFileName(insurance.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileNameInsurance);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await insurance.CopyToAsync(stream);
                }
                document.Insurance = fileNameInsurance;
            }

            _db.CarDocuments.Add(document);
            _db.SaveChanges();

            car.DocumentId = document.DocumentId;

            var address = new Address();
            address.CityId = city;
            address.DistrictId = district;
            address.WardId = ward;
            address.HouseNumberStreet = street;

            _db.Addresses.Add(address);
            _db.SaveChanges();
            car.AddressId = address.AddressId;

            if (front != null)
            {
                var fileNameFront = Path.GetFileName(front.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileNameFront);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await front.CopyToAsync(stream);
                }
                car.FrontImage = fileNameFront;
            }



            if (back != null)
            {
                var fileNameBack = Path.GetFileName(back.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileNameBack);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await back.CopyToAsync(stream);
                }
                car.BackImage = fileNameBack;
            }

            if (left != null)
            {
                var fileNameLeft = Path.GetFileName(left.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileNameLeft);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await left.CopyToAsync(stream);
                }
                car.LeftImage = fileNameLeft;
            }

            if (right != null)
            {
                var fileNameRight = Path.GetFileName(right.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileNameRight);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await right.CopyToAsync(stream);
                }
                car.RightImage = fileNameRight;
            }

            var additionalFunction = new AdditionalFunction();

            additionalFunction.Bluetooth = Bluetooth;
            additionalFunction.Gps = GPS;
            additionalFunction.Camera = Camera;
            additionalFunction.SunRoof = Sunroof;
            additionalFunction.ChildLock = Childlock;
            additionalFunction.ChildSeat = Childseat;
            additionalFunction.Dvd = DVD;
            additionalFunction.Usb = USB;

            _db.AdditionalFunctions.Add(additionalFunction);
            _db.SaveChanges();
            car.FucntionId = additionalFunction.FucntionId;

            car.Status = 1;
            car.NoOfRide = 0;
            var termsOfUse = new TermOfUse();

            termsOfUse.NoSmoking = smoking;
            termsOfUse.NoFoodInCar = food;
            termsOfUse.NoPet = pet;
            if (specify != null)
            {
                termsOfUse.Specify = specify;
            }
            _db.TermOfUses.Add(termsOfUse);
            _db.SaveChanges();
            car.TermId = termsOfUse.TermId;

            var userString = HttpContext.Session.GetString("User");
            User user = null;
            if (!string.IsNullOrEmpty(userString))
            {
                user = JsonConvert.DeserializeObject<User>(userString);
            }
            car.UserId = user.UserId;

            car.Name = _db.CarBrands.FirstOrDefault(x => x.BrandId == car.BrandId).BrandName + " " + _db.CarModels.FirstOrDefault(x => x.ModelId == car.ModelId).ModelName + " " + car.ProductionYear;
            if (car != null)
            {
                _db.Cars.Add(car);
                _db.SaveChanges();
                return RedirectToAction("LoginOwn", "Users");
            }
            else return RedirectToAction("Fail", "Users");
        }


        [HttpGet]
        public ActionResult ViewMyCars()
        {
            var context = new RentCarDbContext();
            var userString = HttpContext.Session.GetString("User");
            User user = null;
            if (!string.IsNullOrEmpty(userString))
            {
                user = JsonConvert.DeserializeObject<User>(userString);
            }
            var userId = user.UserId;
            ViewBag.Cars = context.Cars
                .Where(c => c.UserId == userId)
                                    .OrderByDescending(c => c.CarId)
                                    .Include(c => c.Address)
                                        .ThenInclude(a => a.District)
                                    .Include(c => c.Address)
                                        .ThenInclude(a => a.City)
                                    .Include(c => c.Bookings)
                                    .ToList();
            ViewBag.SortOrder = "newest";

            ViewBag.Bookings = context.Bookings
           .Include(b => b.Car) // Include the Car navigation property
           .ToList();

            //get user to block customer access this view
            if (user.Role == false)
            {
                return View("ErrorAuthorization");
            }
            return View();

        }

        [HttpPost]
        public ActionResult ViewMyCars(string sortOrder)
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
                ViewBag.Cars = context.Cars
                    .Where(c => c.UserId == userId)
                                    .Include(c => c.Address)
                                        .ThenInclude(a => a.District)
                                    .Include(c => c.Address)
                                        .ThenInclude(a => a.City)
                                    .ToList();
                ViewBag.SortOrder = "latest";
            }
            else if(sortOrder == "newest")
            {
                ViewBag.Cars = context.Cars
                    .Where(c => c.UserId == userId)
                                    .OrderByDescending(c => c.CarId)
                                    .Include(c => c.Address)
                                        .ThenInclude(a => a.District)
                                    .Include(c => c.Address)
                                        .ThenInclude(a => a.City)
                                    .ToList();
                ViewBag.SortOrder = "newest";
            }
            else if (sortOrder == "highest")
            {
                ViewBag.Cars = context.Cars
                    .Where(c => c.UserId == userId)
                                    .OrderByDescending(c => c.BasePrice)
                                    .Include(c => c.Address)
                                        .ThenInclude(a => a.District)
                                    .Include(c => c.Address)
                                        .ThenInclude(a => a.City)
                                    .ToList();
                ViewBag.SortOrder = "highest";
            }
            else if (sortOrder == "lowest")
            {
                ViewBag.Cars = context.Cars
                    .Where(c => c.UserId == userId)
                                    .OrderBy(c => c.BasePrice)
                                    .Include(c => c.Address)
                                        .ThenInclude(a => a.District)
                                    .Include(c => c.Address)
                                        .ThenInclude(a => a.City)
                                    .ToList();
                ViewBag.SortOrder = "lowest";
            }
            return View();
        }
        public IActionResult ViewCarDetailsByCustomer(int CarId, string? location, DateTime? startDate, DateTime? endDate)
        {
            ViewBag.location = location;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            var car = _db.Cars.FirstOrDefault(x => x.CarId == CarId);
            var userJson = HttpContext.Session.GetString("User");
            bool checkRent = false;
            if (!string.IsNullOrEmpty(userJson))
            {
                var user = JsonConvert.DeserializeObject<User>(userJson);
                //authority
                if (user.Role == true)
                {
                    return View("ErrorAuthorization");
                }
                List<Booking> lBook = _db.Bookings.Where(x => x.CarId == CarId && x.UserId == user.UserId).ToList();
                foreach (Booking booking in lBook)
                {
                    if (booking.Status == 3|| booking.Status == 4)
                    {
                        checkRent = true;
                        break;
                    }
                }

            }

            
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
                rating = (Math.Ceiling(rating * 2) )/ 2.0;
            }
            else
            {
                rating = 0;
            }
            ViewBag.Rating = rating;
            var brand = _db.CarBrands.FirstOrDefault(x => x.BrandId == car.BrandId);
            var model = _db.CarModels.FirstOrDefault(x => x.ModelId == car.ModelId);
            var document = _db.CarDocuments.FirstOrDefault(x => x.DocumentId == car.DocumentId);
            var color = _db.CarColors.FirstOrDefault(x => x.ColorId == car.ColorId);
            var address = _db.Addresses.FirstOrDefault(x => x.AddressId == car.AddressId);
            var ward = _db.Wards.FirstOrDefault(x => x.WardId == address.WardId);
            var district = _db.Districts.FirstOrDefault(x => x.DistrictId == address.DistrictId);
            var city = _db.Cities.FirstOrDefault(x => x.CityId == address.CityId);
            var term = _db.TermOfUses.FirstOrDefault(x => x.TermId == car.TermId);
            var function = _db.AdditionalFunctions.FirstOrDefault(x => x.FucntionId == car.FucntionId);
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
            return View();
        }

        public IActionResult ChangeCarDetailsByOwner(int CarId)
        {
            var car = _db.Cars.FirstOrDefault(x => x.CarId == CarId);
            var userJson = HttpContext.Session.GetString("User");
            bool checkRent = false;
            if (!string.IsNullOrEmpty(userJson))
            {
                var user = JsonConvert.DeserializeObject<User>(userJson);

                if (user.Role == false)
                {
                    return View("ErrorAuthorization");
                }

                List<Booking> lBooking = _db.Bookings.Where(x => x.CarId == CarId && x.UserId == user.UserId).ToList();
                foreach (Booking booking in lBooking)
                {
                    if (booking.Status == 3)
                    {
                        checkRent = true;
                        break;
                    }
                }
            }

            var brand = _db.CarBrands.FirstOrDefault(x => x.BrandId == car.BrandId);
            var model = _db.CarModels.FirstOrDefault(x => x.ModelId == car.ModelId);
            var document = _db.CarDocuments.FirstOrDefault(x => x.DocumentId == car.DocumentId);
            var color = _db.CarColors.FirstOrDefault(x => x.ColorId == car.ColorId);
            var address = _db.Addresses.FirstOrDefault(x => x.AddressId == car.AddressId);
            var ward = _db.Wards.FirstOrDefault(x => x.WardId == address.WardId);
            var district = _db.Districts.FirstOrDefault(x => x.DistrictId == address.DistrictId);
            var city = _db.Cities.FirstOrDefault(x => x.CityId == address.CityId);
            var term = _db.TermOfUses.FirstOrDefault(x => x.TermId == car.TermId);
            var function = _db.AdditionalFunctions.FirstOrDefault(x => x.FucntionId == car.FucntionId);
            var listCity = _db.Cities.ToList();
            var listDistrict = _db.Districts.ToList();
            var listWard = _db.Wards.ToList();


            var bookingg = _db.Bookings.FirstOrDefault(x => x.CarId == CarId && x.Status == 1);
            ViewBag.booking = bookingg;



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
            ViewBag.listCity = listCity;
            ViewBag.listDistrict = listDistrict;
            ViewBag.listWard = listWard;

            return View(car);
        }


        [HttpPost]
        public IActionResult ChangeCarDetailsByOwner(Car car,
            IFormFile front, IFormFile back, IFormFile left, IFormFile right,
            bool Bluetooth, bool GPS, bool Camera, bool Sunroof, bool Childlock, bool Childseat, bool DVD, bool USB,
            int city, int district, int ward, string street)
        {
            var carId = car.CarId;
            var carrrr = _db.Cars.FirstOrDefault(car => car.CarId == carId);

            var address = _db.Addresses.FirstOrDefault(a => a.AddressId == carrrr.AddressId);

            address.CityId = city;
            address.DistrictId = district;
            address.WardId = ward;
            address.HouseNumberStreet = street;

            _db.Addresses.Update(address);
            _db.SaveChanges();

            carrrr.Mileage = car.Mileage;
            carrrr.FuelConsumption = car.FuelConsumption;
            carrrr.Description = car.Description;
            _db.Cars.Update(carrrr);
            _db.SaveChanges();



            if (front != null)
            {
                var fileNameFront = Path.GetFileName(front.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileNameFront);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    front.CopyToAsync(stream);
                }
                carrrr.FrontImage = fileNameFront;
            }



            if (back != null)
            {
                var fileNameBack = Path.GetFileName(back.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileNameBack);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    back.CopyToAsync(stream);
                }
                carrrr.BackImage = fileNameBack;
            }

            if (left != null)
            {
                var fileNameLeft = Path.GetFileName(left.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileNameLeft);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    left.CopyToAsync(stream);
                }
                carrrr.LeftImage = fileNameLeft;
            }

            if (right != null)
            {
                var fileNameRight = Path.GetFileName(right.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileNameRight);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    right.CopyToAsync(stream);
                }
                carrrr.RightImage = fileNameRight;
            }

            _db.Cars.Update(carrrr);
            _db.SaveChanges();

            var additionalFunction = _db.AdditionalFunctions.FirstOrDefault(fnc => fnc.FucntionId == carrrr.FucntionId);

            additionalFunction.Bluetooth = Bluetooth;
            additionalFunction.Gps = GPS;
            additionalFunction.Camera = Camera;
            additionalFunction.SunRoof = Sunroof;
            additionalFunction.ChildLock = Childlock;
            additionalFunction.ChildSeat = Childseat;
            additionalFunction.Dvd = DVD;
            additionalFunction.Usb = USB;

            _db.AdditionalFunctions.Update(additionalFunction);
            _db.SaveChanges();

            return RedirectToAction("ChangeCarDetailsByOwner", new { CarId = car.CarId });
        }



        [HttpPost]
        public IActionResult ChangeCarTermsByOwner(Car car,
           bool smoking, bool food, bool pet, string specify)
        {
            var carId = car.CarId;
            var carrrr = _db.Cars.FirstOrDefault(car => car.CarId == carId);

       
            carrrr.BasePrice = car.BasePrice;
            carrrr.Deposit = car.Deposit;
            _db.Update(carrrr);
            _db.SaveChanges();


            var termsOfUse = _db.TermOfUses.FirstOrDefault(terms => terms.TermId == carrrr.TermId);

            termsOfUse.NoSmoking = smoking;
            termsOfUse.NoFoodInCar = food;
            termsOfUse.NoPet = pet;
            if (specify != null)
            {
                termsOfUse.Specify = specify;
            }

            _db.Update(termsOfUse);
            _db.SaveChanges();


            return RedirectToAction("ChangeCarDetailsByOwner", new { CarId = car.CarId});
        }

        [HttpPost]
        public IActionResult ChangeCarStatus(Car car)
        {
            var carId = car.CarId;
            var carrrr = _db.Cars.FirstOrDefault(car => car.CarId == carId);

            carrrr.Status = car.Status;
            _db.Update(carrrr);
            _db.SaveChanges();
            TempData["SuccessMessage"] = "Your car status changed!";
            return RedirectToAction("ChangeCarDetailsByOwner", new { CarId = car.CarId });

        }
        private readonly IEmailService _emailService;

        [HttpPost] 
        public IActionResult ReturnCar(int carId, int userId, decimal amount)
        {
            var booking = _db.Bookings.SingleOrDefault(b => b.CarId == carId && (b.Status==3 || b.Status == 4));
            var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            var car = _db.Cars.FirstOrDefault(c => c.CarId == carId);
            var carOwner = _db.Users.FirstOrDefault(u => u.UserId == car.UserId);
            if (user != null)
            {
                if((-amount) > user.Wallet)
                {
                    TempData["ErrorMessage"] = "Your wallet doesn’t have enough balance. Please top-up your wallet and try again";
                    booking.Status = 4;
                    _db.Update(booking);
                    _db.SaveChanges();
                    return RedirectToAction("ViewBookingList", "Booking");
                }
                user.Wallet += amount; 
                var transactionUser = new Wallet
                {
                    UserId = userId,
                    Amount = string.Format("{0:+#,##0.00;-#,##0.00;0.00}", amount),
                    Type = "Offset final payment",
                    CreatedAt = DateTime.Now,
                    BookingNo = booking.BookingNo,
                    CarName = car.Name
                };
                carOwner.Wallet -= amount;
                var transactionCarOwnerType = amount > 0 ? "Return Transaction" : "Receive Transaction";
                var transactionCarOwner = new Wallet
                {
                    UserId = carOwner.UserId,
                    Amount = string.Format("{0:+#,##0.00;-#,##0.00;0.00}", -amount),
                    Type = transactionCarOwnerType,
                    CreatedAt = DateTime.Now,
                    BookingNo = booking.BookingNo,
                    CarName = car.Name
                };
                _db.Wallets.Add(transactionUser);
                _db.Wallets.Add(transactionCarOwner);
                booking.Status = 5;
                car.NoOfRide += 1;
                _db.Update(car);
                _db.Update(booking);
                _db.SaveChanges();
                _emailService.SendReturnEmail(carOwner.Email, car.Name, DateTime.Now);
            }
            return RedirectToAction("ViewBookingList", "Booking");
        }
        [HttpPost]
        public IActionResult ReturnCarInDetail(int carId, int userId, decimal amount)
        {
            var booking = _db.Bookings.SingleOrDefault(b => b.CarId == carId && (b.Status == 3|| b.Status==4));
            var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            var car = _db.Cars.FirstOrDefault(c => c.CarId == carId);
            var carOwner = _db.Users.FirstOrDefault(u => u.UserId == car.UserId);
            if (user != null)
            {
                if ((-amount) > user.Wallet)
                {
                    TempData["ErrorMessage"] = "Your wallet doesn’t have enough balance. Please top-up your wallet and try again";
                    booking.Status = 4;
                    _db.Update(booking);
                    _db.SaveChanges();
                    return RedirectToAction("EditBookingDetail","Booking", new { startDate = booking.StartDate, endDate = booking.EndDate, carId = carId, bookingNo = booking.BookingNo });
                }
                user.Wallet += amount;
                var transactionUser = new Wallet
                {
                    UserId = userId,
                    Amount = string.Format("{0:+#,##0.00;-#,##0.00;0.00}", amount),
                    Type = "Offset final payment",
                    CreatedAt = DateTime.Now,
                    BookingNo = booking.BookingNo,
                    CarName = car.Name
                };
                carOwner.Wallet -= amount;
                var transactionCarOwnerType = amount > 0 ? "Return Transaction" : "Receive Transaction";
                var transactionCarOwner = new Wallet
                {
                    UserId = carOwner.UserId,
                    Amount = string.Format("{0:+#,##0.00;-#,##0.00;0.00}", -amount),
                    Type = transactionCarOwnerType,
                    CreatedAt = DateTime.Now,
                    BookingNo = booking.BookingNo,
                    CarName = car.Name
                };
                _db.Wallets.Add(transactionUser);
                _db.Wallets.Add(transactionCarOwner);
                booking.Status = 5;
                car.NoOfRide += 1;
                _db.Update(car);
                _db.Update(booking);
                _db.SaveChanges();
                _emailService.SendReturnEmail(carOwner.Email, car.Name, DateTime.Now);
            }
            return RedirectToAction("EditBookingDetail", "Booking", new { startDate = booking.StartDate, endDate = booking.EndDate, carId = carId, bookingNo = booking.BookingNo });
        }

        [HttpPost]
        public IActionResult ConfirmDeposit(Car car)
        {
            
            var booking = _db.Bookings.FirstOrDefault(b => b.CarId == car.CarId && b.Status == 1);
            if (booking != null)
            {

                booking.Status = 2;
                _db.Update(booking);
                _db.SaveChanges();
                
            }
            TempData["SuccessMessage"] = "You confirmed deposit!";
            return RedirectToAction("ChangeCarDetailsByOwner", new { CarId = car.CarId });
        }
    }
}
