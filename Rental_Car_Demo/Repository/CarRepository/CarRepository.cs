using Rental_Car_Demo.Models;

namespace Rental_Car_Demo.Repository.CarRepository
{
    public class CarRepository : ICarRepository
    {

        public void AddCar(Car car) => CarDAO.Instance.CreateCar (car);

        public IEnumerable<Car> GetAllCars(string address) => CarDAO.Instance.GetAllCars(address);

        public IEnumerable<Car> GetAllCars() => CarDAO.Instance.GetAllCars();

        public IEnumerable<Car> SearchCars(string[] brandNames, int[] seats, bool[] transmissionTypes, bool[] fuelTypes, decimal? minPrice, decimal? maxPrice, string address) =>
            CarDAO.Instance.SearchCars(brandNames, seats, transmissionTypes, fuelTypes, minPrice, maxPrice, address);
    }
}
