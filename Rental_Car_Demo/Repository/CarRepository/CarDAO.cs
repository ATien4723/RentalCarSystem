using Microsoft.EntityFrameworkCore;
using Rental_Car_Demo.Models;
namespace Rental_Car_Demo.Repository.CarRepository
{
    public class CarDAO
    {
        private readonly RentCarDbContext _context;
        private static CarDAO instance;
        public static readonly object instanceLock = new object();
        public static CarDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CarDAO();
                    }
                    return instance;
                }
            }
        }
        public void CreateCar(Car car)
        {
            try
            {
                var context = new RentCarDbContext();
                context.Cars.Add(car);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Car> GetAllCars(string address)
        {
            try
            {
                using (var context = new RentCarDbContext())
                {
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

                    if (!string.IsNullOrEmpty(address))
                    {
                        cars = cars.Where(c => (c.Address.HouseNumberStreet + ", " +
                                                 c.Address.Ward.WardName + ", " +
                                                 c.Address.District.DistrictName + ", " +
                                                 c.Address.City.CityProvince).Contains(address));
                    }

                    return cars.ToList();
                }
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
                using (var context = new RentCarDbContext())
                {
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
                        .Where(c => c.Status != 2)
                        .AsQueryable();


                    return cars.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Car> SearchCars(string[] brandNames, int[] seats, bool[] transmissionTypes, bool[] fuelTypes, string[] brandLogos, decimal? minPrice, decimal? maxPrice, string address)
        {
            try
            {
                using (var context = new RentCarDbContext())
                {
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

                    if (brandLogos != null && brandLogos.Length > 0)
                    {
                        cars = cars.Where(c => brandLogos.Any(logo => c.Brand.BrandLogo.Contains(logo)));
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
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}

