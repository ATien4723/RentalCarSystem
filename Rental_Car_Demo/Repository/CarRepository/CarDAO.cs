using Microsoft.EntityFrameworkCore;
using Rental_Car_Demo.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
namespace Rental_Car_Demo.Repository.CarRepository
{
    [ExcludeFromCodeCoverage]
    public class CarDAO
    {
        private static CarDAO instance;
        public static readonly object instanceLock = new object ();
        private readonly RentCarDbContext context;
        public CarDAO(RentCarDbContext _context)
        {
            context = _context;
        }
        public static CarDAO Instance
        {
            get {
                lock ( instanceLock ) {
                    if ( instance == null ) {
                        var context = new RentCarDbContext ();
                        instance = new CarDAO (context);
                    }
                    return instance;
                }
            }
        }

        private RentCarDbContext CreateContext()
        {
            return new RentCarDbContext ();
        }


        public void CreateCar(Car car)
        {
            // Perform validation
            var validationContext = new ValidationContext (car);
            var validationResults = new List<ValidationResult> ();

            bool isValid = Validator.TryValidateObject (car, validationContext, validationResults, true);

            if ( !isValid ) {
                var errors = string.Join ("; ", validationResults.Select (vr => vr.ErrorMessage));
                throw new ValidationException ($"Car model validation failed: {errors}");
            }

            // Proceed to add car if valid
            try {
                context.Cars.Add (car);
                context.SaveChanges ();
            } catch ( Exception ex ) {
                throw new Exception ($"An error occurred while saving the car: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Car>> GetAllCars( string address,string[] brandNames,int[] seats, bool[] transmissionTypes,bool[] fuelTypes,decimal? minPrice,decimal? maxPrice)
        {
            try {
                var carsQuery = context.Cars
                    .AsNoTracking ()
                    .Where (c => c.Status != 2 && c.Status != 3)
                    .AsQueryable ();

                if ( brandNames != null && brandNames.Length > 0 ) {
                    carsQuery = carsQuery.Where (c => brandNames.Contains (c.Brand.BrandName));
                }

                if ( seats != null && seats.Length > 0 ) {
                    carsQuery = carsQuery.Where (c => seats.Contains (c.Seats));
                }

                if ( transmissionTypes != null && transmissionTypes.Length > 0 ) {
                    carsQuery = carsQuery.Where (c => transmissionTypes.Contains (c.TransmissionType));
                }

                if ( fuelTypes != null && fuelTypes.Length > 0 ) {
                    carsQuery = carsQuery.Where (c => fuelTypes.Contains (c.FuelType));
                }

                if ( minPrice.HasValue ) {
                    carsQuery = carsQuery.Where (c => c.BasePrice >= minPrice.Value);
                }

                if ( maxPrice.HasValue ) {
                    carsQuery = carsQuery.Where (c => c.BasePrice <= maxPrice.Value);
                }

                if ( !string.IsNullOrEmpty (address) ) {
                    address = address.Trim ().ToLower ();
                    carsQuery = carsQuery.Where (c =>
                        ( c.Address.City.CityProvince.Trim ().ToLower () == address ||
                         c.Address.District.DistrictName.Trim ().ToLower () == address ||
                         c.Address.Ward.WardName.Trim ().ToLower () == address ));
                }

                // Include related entities
                return await carsQuery
                    .Include (c => c.Brand)
                    .Include (c => c.Address)
                        .ThenInclude (a => a.City)
                    .Include (c => c.Address)
                        .ThenInclude (a => a.District)
                    .Include (c => c.Address)
                        .ThenInclude (a => a.Ward)
                    .ToListAsync ();
            } catch ( Exception ex ) {
                throw new Exception (ex.Message);
            }
        }


        public IEnumerable<Car> GetAllCars()
        {
            try {
                //using (var context = new RentCarDbContext())
                //{
                var cars = context.Cars
                    .Include (c => c.Brand)
                    .Include (c => c.Address)
                        .ThenInclude (a => a.City)
                    .Include (c => c.Address)
                        .ThenInclude (a => a.District)
                    .Include (c => c.Address)
                        .ThenInclude (a => a.Ward)
                    .Where (c => c.Status != 2)
                    .AsQueryable ();

                int count = cars.ToList ().Count;
                return cars.ToList ();

                //}
            } catch ( Exception ex ) {
                throw new Exception (ex.Message);
            }
        }

        public async Task<IEnumerable<Car>> SearchCars(string[] brandNames, int[] seats, bool[] transmissionTypes, bool[] fuelTypes, decimal? minPrice, decimal? maxPrice, string address)
        {
            try {
                // Use a new DbContext instance for this operation
                using ( var context = CreateContext () ) {
                    var carsQuery = context.Cars
                        .AsNoTracking ()
                        .Where (c => c.Status != 2)
                        .AsQueryable ();

                    // Apply filters sequentially
                    if ( brandNames != null && brandNames.Length > 0 ) {
                        carsQuery = carsQuery.Where (c => brandNames.Contains (c.Brand.BrandName));
                    }

                    if ( seats != null && seats.Length > 0 ) {
                        carsQuery = carsQuery.Where (c => seats.Contains (c.Seats));
                    }

                    if ( transmissionTypes != null && transmissionTypes.Length > 0 ) {
                        carsQuery = carsQuery.Where (c => transmissionTypes.Contains (c.TransmissionType));
                    }

                    if ( fuelTypes != null && fuelTypes.Length > 0 ) {
                        carsQuery = carsQuery.Where (c => fuelTypes.Contains (c.FuelType));
                    }

                    if ( minPrice.HasValue ) {
                        carsQuery = carsQuery.Where (c => c.BasePrice >= minPrice.Value);
                    }

                    if ( maxPrice.HasValue ) {
                        carsQuery = carsQuery.Where (c => c.BasePrice <= maxPrice.Value);
                    }

                    if ( !string.IsNullOrEmpty (address) ) {
                        carsQuery = carsQuery.Where (c => ( c.Address.HouseNumberStreet + ", " +
                                                           c.Address.Ward.WardName + ", " +
                                                           c.Address.District.DistrictName + ", " +
                                                           c.Address.City.CityProvince ).Contains (address));
                    }

                    // Include related entities and execute query
                    return await carsQuery
                        .Include (c => c.Brand)
                        .Include (c => c.Address)
                            .ThenInclude (a => a.City)
                        .Include (c => c.Address)
                            .ThenInclude (a => a.District)
                        .ToListAsync ();
                }
            } catch ( Exception ex ) {
                // Consider logging the exception before rethrowing it
                throw new Exception (ex.Message);
            }
        }

    }
}

