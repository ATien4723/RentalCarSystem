using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class BookingInfo
{
    public int BookingInfoId { get; set; }

    public string RenterEmail { get; set; } = null!;

    public string RenterName { get; set; } = null!;

    public DateOnly RenterDob { get; set; }

    public int RenterNationalId { get; set; }

    public string RenterPhone { get; set; } = null!;

    public int? RenterAddressId { get; set; }

    public string? RenterDrivingLicense { get; set; }

    public bool? IsDifferent { get; set; }

    public string DriverEmail { get; set; } = null!;

    public string DriverName { get; set; } = null!;

    public DateOnly DriverDob { get; set; }

    public int DriverNationalId { get; set; }

    public string DriverPhone { get; set; } = null!;

    public int? DriverAddressId { get; set; }

    public string? DriverDrivingLicense { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Address? DriverAddress { get; set; }

    public virtual Address? RenterAddress { get; set; }
}
