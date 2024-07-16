using Rental_Car_Demo.Models;

namespace Rental_Car_Demo.Repository.CarRepository
{
    public class CarRepository : ICarRepository
    {
        public void AddCar(Car car) => CarDAO.Instance.CreateCar(car);

        public IEnumerable<Car> GetAllCars(string address) => CarDAO.Instance.GetAllCars (address);

        public IEnumerable<Car> SearchCars(string brandName, int? seats, bool? transmissionType , string brandLogo, decimal? minPrice, decimal? maxPrice , string address) =>
          CarDAO.Instance.SearchCars (brandName, seats, transmissionType, brandLogo , minPrice , maxPrice , address);
    }
}
