using Microsoft.EntityFrameworkCore;
using Rental_Car_Demo.Models;
using System.ComponentModel.DataAnnotations;
namespace Rental_Car_Demo.Repository.CarRepository
{
    public class CarDAO
    {
        private static CarDAO instance;
        public static readonly object instanceLock = new object();
        private readonly RentCarDbContext context;
        public CarDAO(RentCarDbContext _context)
        {
            context = _context;
        }
        public static CarDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        var context = new RentCarDbContext();
                        instance = new CarDAO(context);
                    }
                    return instance;
                }
            }
        }
        public void CreateCar(Car car)
        {
            // Perform validation
            var validationContext = new ValidationContext(car);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(car, validationContext, validationResults, true);

            if (!isValid)
            {
                var errors = string.Join("; ", validationResults.Select(vr => vr.ErrorMessage));
                throw new ValidationException($"Car model validation failed: {errors}");
            }

            // Proceed to add car if valid
            try
            {
                context.Cars.Add(car);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while saving the car: {ex.Message}");
            }
        }

        public IEnumerable<Car> GetAllCars (string address)
        {
            try
            {
                //using (var context = new RentCarDbContext())
                //{
                var cars = context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Model)
                    .Include(c => c.Color)
                    .Include(c => c.Address)
                        .ThenInclude(a => a.City)
                    .Include(c => c.Address)
                        .ThenInclude(a => a.District)
                    .Include(c => c.Address)
                        .ThenInclude(a => a.Ward)
                    .Include(c => c.Document)
                    .Include(c => c.Term)
                    .Where(c => c.Status != 2)
                    .Include(c => c.User)
                    .Include(c => c.Bookings)
                    .Where(c => c.Status != 2 && c.Status != 3)
                    .AsQueryable();

                if ( !string.IsNullOrEmpty (address) ) {
                    address = address.Trim ().ToLower ();

                    cars = cars.Where (c => ( c.Address.HouseNumberStreet + ", " +
                                            c.Address.Ward.WardName + ", " +
                                            c.Address.District.DistrictName + ", " +
                                            c.Address.City.CityProvince ).Trim ().ToLower().Contains (address));

                }

                return cars.ToList();
                //}
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Car> GetAllCars()
        {
            try
            {
                //using (var context = new RentCarDbContext())
                //{
                var cars = context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Model)
                    .Include(c => c.Color)
                    .Include(c => c.Address)
                        .ThenInclude(a => a.City)
                    .Include(c => c.Address)
                        .ThenInclude(a => a.District)
                    .Include(c => c.Address)
                        .ThenInclude(a => a.Ward)
                    .Include(c => c.Document)
                    .Include(c => c.Term)
                    .Include(c => c.User)
                    .Include(c => c.Bookings)
                    .Where(c => c.Status != 2)
                    .AsQueryable();

                int count = cars.ToList().Count;
                return cars.ToList();
                
                //}
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Car> SearchCars(string[] brandNames, int[] seats, bool[] transmissionTypes, bool[] fuelTypes, decimal? minPrice, decimal? maxPrice, string address)
        {
            try
            {
                //using (var context = new RentCarDbContext())
                //{
                var cars = context.Cars
                    .Include(c => c.Brand)
                    .Include(c => c.Model)
                    .Include(c => c.Color)
                    .Include(c => c.Address)
                    .ThenInclude(a => a.City)
                    .Include(c => c.Address)
                    .ThenInclude(a => a.District)
                    .Include(c => c.Address)
                    .ThenInclude(a => a.Ward)
                    .Include(c => c.Document)
                    .Include(c => c.Term)
                    .Include(c => c.User)
                    .Include(c => c.Bookings)
                    .Where(c => c.Status != 2)
                    .AsQueryable();

                if (brandNames != null && brandNames.Length > 0)
                {
                    cars = cars.Where(c => brandNames.Contains(c.Brand.BrandName));
                }

                if (seats != null && seats.Length > 0)
                {
                    cars = cars.Where(c => seats.Contains(c.Seats));
                }

                if (transmissionTypes != null && transmissionTypes.Length > 0)
                {
                    cars = cars.Where(c => transmissionTypes.Contains(c.TransmissionType));
                }

                if (fuelTypes != null && fuelTypes.Length > 0)
                {
                    cars = cars.Where(c => fuelTypes.Contains(c.FuelType));
                }

                if (minPrice.HasValue)
                {
                    cars = cars.Where(c => c.BasePrice >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    cars = cars.Where(c => c.BasePrice <= maxPrice.Value);
                }

                if (!string.IsNullOrEmpty(address))
                {
                    cars = cars.Where(c => (c.Address.HouseNumberStreet + ", " +
                                             c.Address.Ward.WardName + ", " +
                                             c.Address.District.DistrictName + ", " +
                                             c.Address.City.CityProvince).Contains(address));
                }

                return cars.ToList();
                //}
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}

