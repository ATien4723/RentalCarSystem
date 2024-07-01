using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class Ward
{
    public int WardId { get; set; }

    public int? DistrictId { get; set; }

    public string? WardName { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual District? District { get; set; }
}
