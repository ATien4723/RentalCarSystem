using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class CarModel
{
    public int ModelId { get; set; }

    public int? BrandId { get; set; }

    public string? ModelName { get; set; }

    public virtual CarBrand? Brand { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
