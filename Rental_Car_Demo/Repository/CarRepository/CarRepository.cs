using Rental_Car_Demo.Models;

namespace Rental_Car_Demo.Repository.CarRepository
{
    public class CarRepository : ICarRepository
    {
        private readonly CarDAO _carDao;

        public CarRepository(CarDAO carDao)
        {
            _carDao = carDao;
        }

        public void AddCar(Car car) => _carDao.CreateCar(car);

        public IEnumerable<Car> GetAllCars(string address) => _carDao.GetAllCars(address);

        public IEnumerable<Car> GetAllCars() => _carDao.GetAllCars();

        public IEnumerable<Car> SearchCars(string[] brandNames, int[] seats, bool[] transmissionTypes, bool[] fuelTypes, decimal? minPrice, decimal? maxPrice, string address) =>
            _carDao.SearchCars(brandNames, seats, transmissionTypes, fuelTypes, minPrice, maxPrice, address);
    }
}
