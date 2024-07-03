using Rental_Car_Demo.Models;

namespace Rental_Car_Demo.Repository.CarRepository
{
    public class CarRepository : ICarRepository
    {
        public void AddCar(Car car) => CarDAO.Instance.CreateCar(car);
    }
}
