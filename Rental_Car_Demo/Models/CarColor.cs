using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class CarColor
{
    public int ColorId { get; set; }

    public string? ColorName { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
