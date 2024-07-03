using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class TermOfUse
{
    public int TermId { get; set; }

    public bool? NoSmoking { get; set; }

    public bool? NoFoodInCar { get; set; }

    public bool? NoPet { get; set; }

    public string? Specify { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
