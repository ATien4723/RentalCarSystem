using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class CarDocument
{
    public int DocumentId { get; set; }

    public string Registration { get; set; } = null!;

    public string Certificate { get; set; } = null!;

    public string? Insurance { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
