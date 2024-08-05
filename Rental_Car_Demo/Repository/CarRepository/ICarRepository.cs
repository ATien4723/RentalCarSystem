using Rental_Car_Demo.Models;
using System.Data.SqlTypes;
namespace Rental_Car_Demo.Repository.CarRepository
{
    public interface ICarRepository
    {
        public void AddCar(Car car);
        IEnumerable<Car> GetAllCars(string address);

        IEnumerable<Car> GetAllCars();
        IEnumerable<Car> SearchCars(string brandName, int? seats, bool? transmissionType, bool? fuelType, string brandLogo, decimal? minPrice, decimal? maxPrice, string address);


    }
}
