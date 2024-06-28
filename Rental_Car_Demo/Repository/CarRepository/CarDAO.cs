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
    }
}
