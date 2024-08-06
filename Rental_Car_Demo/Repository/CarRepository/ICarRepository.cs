using Rental_Car_Demo.Models;
using System.Data.SqlTypes;
namespace Rental_Car_Demo.Repository.CarRepository
{
    public interface ICarRepository
    {
        public void AddCar(Car car);
        IEnumerable<Car> GetAllCars(string address);

        IEnumerable<Car> GetAllCars();
        IEnumerable<Car> SearchCars(string[] brandNames, int[] seats, bool[] transmissionTypes, bool[] fuelTypes, string[] brandLogos, decimal? minPrice, decimal? maxPrice, string address);


    }
}
