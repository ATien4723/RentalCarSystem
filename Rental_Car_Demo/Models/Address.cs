using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public int? CityId { get; set; }

    public int? DistrictId { get; set; }

    public int? WardId { get; set; }

    public string? HouseNumberStreet { get; set; }

    public virtual ICollection<BookingInfo> BookingInfoDriverAddresses { get; set; } = new List<BookingInfo>();

    public virtual ICollection<BookingInfo> BookingInfoRenterAddresses { get; set; } = new List<BookingInfo>();

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual City? City { get; set; }

    public virtual District? District { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual Ward? Ward { get; set; }
}
