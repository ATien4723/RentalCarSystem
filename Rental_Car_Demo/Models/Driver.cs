using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class Driver
{
    public int DriverId { get; set; }

    public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateOnly? Dob { get; set; }

    public int? NationalId { get; set; }

    public string Phone { get; set; } = null!;

    public int? AddressId { get; set; }

    public string? DrivingLicense { get; set; }

    public bool? IsDifferent { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
