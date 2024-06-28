using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rental_Car_Demo.Models;
using Rental_Car_Demo.Repository.CarRepository;

namespace Rental_Car_Demo.Controllers
{
    public class CarController : Controller
    {
        ICarRepository carRepository = null;

        RentCarDbContext _db = new RentCarDbContext();
        public CarController() => carRepository = new CarRepository();


        public IActionResult GetModelsByBrand(int brandId)
        {
            var context = new RentCarDbContext();
            var models = context.CarModels.Where(m => m.BrandId == brandId).ToList();
            return Json(models);
        }
        public IActionResult AddACar()
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
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddACarAsync(Car car, IFormFile registration, IFormFile certificate, IFormFile insurance,
            IFormFile front, IFormFile back, IFormFile left, IFormFile right,
            bool Bluetooth, bool GPS, bool Camera, bool Sunroof, bool Childlock, bool Childseat, bool DVD, bool USB,
            bool smoking, bool food, bool pet, string specify,int city,int district,int ward,string street)
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
            address.HouseNumberStreet=street;

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
                car.FrontbackImage = fileNameFront;
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
            car.FucntionId=additionalFunction.FucntionId;

            car.Status = 1;

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

            car.Name = _db.CarBrands.FirstOrDefault( x=>x.BrandId==car.BrandId).BrandName + " " + _db.CarModels.FirstOrDefault(x => x.ModelId == car.ModelId).ModelName  + " " + car.ProductionYear;
            if (car != null)
            {
                _db.Cars.Add(car);
                _db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            else return RedirectToAction("Privacy", "Home");
        }
    }
}
