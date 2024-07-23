using Microsoft.EntityFrameworkCore;
using Rental_Car_Demo.Models;
namespace Rental_Car_Demo.Repository.CarRepository
{
    public class CarDAO
    {
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

        public IEnumerable<Car> GetAllCars (string address)
        {
            try {
                using ( var context = new RentCarDbContext () ) {
                    var cars = context.Cars
                        .Include (c => c.Brand)
                        .Include (c => c.Model)
                        .Include (c => c.Color)
                        .Include (c => c.Address)
                            .ThenInclude (a => a.City)
                        .Include (c => c.Address)
                            .ThenInclude (a => a.District)
                        .Include (c => c.Address)
                            .ThenInclude (a => a.Ward)
                        .Include (c => c.Document)
                        .Include (c => c.Term)
                        .Where (c => c.Status != 2)
                        .Include (c => c.User)
                        .Include (c => c.Bookings)
                        .Where(c => c.Status != 2)
                        .AsQueryable ();

                    if ( !string.IsNullOrEmpty (address) ) {
                        cars = cars.Where (c => ( c.Address.HouseNumberStreet + ", " +
                                                 c.Address.Ward.WardName + ", " +
                                                 c.Address.District.DistrictName + ", " +
                                                 c.Address.City.CityProvince ).Contains (address));
                    }

                    return cars.ToList ();
                }
            } catch ( Exception ex ) {
                throw new Exception (ex.Message);
            }
        }

        public IEnumerable<Car> SearchCars(string brandName, int? seats, bool? transmissionType, string brandLogo, decimal? minPrice, decimal? maxPrice, string address)
        {
            try {
                using ( var context = new RentCarDbContext () ) {
                    var cars = context.Cars
                        .Include (c => c.Brand)
                        .Include (c => c.Model)
                        .Include (c => c.Color)
                        .Include (c => c.Address)
                            .ThenInclude (a => a.City)
                        .Include (c => c.Address)
                            .ThenInclude (a => a.District)
                        .Include (c => c.Address)
                            .ThenInclude (a => a.Ward)
                        .Include (c => c.Document)
                        .Include (c => c.Term)
                        .Include (c => c.User)
                        .Include (c => c.Bookings)
                        .Where(c => c.Status != 2)
                        .AsQueryable ();

                    if ( !string.IsNullOrEmpty (brandName) ) {
                        cars = cars.Where (c => c.Brand.BrandName.Contains (brandName));
                    }

                    if ( seats.HasValue ) {
                        cars = cars.Where (c => c.Seats == seats);
                    }

                    if ( transmissionType.HasValue ) {
                        cars = cars.Where (c => c.TransmissionType == transmissionType.Value);
                    }

                    if ( !string.IsNullOrEmpty (brandLogo) ) {
                        cars = cars.Where (c => c.Brand.BrandLogo == brandLogo);
                    }

                    if ( minPrice.HasValue ) {
                        cars = cars.Where (c => c.BasePrice >= minPrice.Value);
                    }

                    if ( maxPrice.HasValue ) {
                        cars = cars.Where (c => c.BasePrice <= maxPrice.Value);
                    }

                    if ( !string.IsNullOrEmpty (address) ) {
                        cars = cars.Where (c => ( c.Address.HouseNumberStreet + ", " +
                                                 c.Address.Ward.WardName + ", " +
                                                 c.Address.District.DistrictName + ", " +
                                                 c.Address.City.CityProvince ).Contains (address));
                    }

                    return cars.ToList ();
                }
            } catch ( Exception ex ) {
                throw new Exception (ex.Message);
            }
        }


    }
}

