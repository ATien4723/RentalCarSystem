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
        public IEnumerable<Car> GetAllCars()
        {
            try {
                using ( var context = new RentCarDbContext () ) {

                    return context.Cars
                           .Include (c => c.Brand)
                           .Include (c => c.Model)
                           .Include (c => c.Color)
                           .Include (c => c.Address)
                           .Include (c => c.Document)
                           .Include (c => c.Fucntion)  // Note: Check spelling of Fucntion, it might be Function
                           .Include (c => c.Term)
                           .Include (c => c.User)
                           .Include (c => c.Bookings)
                           .ToList ();
                }
            
            } catch ( Exception ex ) {
                throw new Exception (ex.Message);
            }
        }

        }
}
