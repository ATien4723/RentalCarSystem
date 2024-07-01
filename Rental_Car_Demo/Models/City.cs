using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class City
{
    public int CityId { get; set; }

    public string? CityProvince { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}
