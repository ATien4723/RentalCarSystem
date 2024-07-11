using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class CarBrand
{
    public int BrandId { get; set; }

    public string? BrandName { get; set; }

    public virtual ICollection<CarModel> CarModels { get; set; } = new List<CarModel>();

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
