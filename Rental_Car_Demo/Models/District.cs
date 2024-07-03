using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class District
{
    public int DistrictId { get; set; }

    public int? CityId { get; set; }

    public string? DistrictName { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual City? City { get; set; }

    public virtual ICollection<Ward> Wards { get; set; } = new List<Ward>();
}
