using Rental_Car_Demo.Models;

namespace Rental_Car_Demo.Repository.CarRepository
{
    public class CarRepository : ICarRepository
    {
        public void AddCar(Car car) => CarDAO.Instance.CreateCar(car);

        public IEnumerable<Car> GetAllCars(string address) => CarDAO.Instance.GetAllCars (address);

        public IEnumerable<Car> GetAllCars() => CarDAO.Instance.GetAllCars();

        public IEnumerable<Car> SearchCars(string brandName, int? seats, bool? transmissionType , bool? fuelType, string brandLogo, decimal? minPrice, decimal? maxPrice , string address) =>
          CarDAO.Instance.SearchCars (brandName, seats, transmissionType, fuelType, brandLogo , minPrice , maxPrice , address);
    }
}
