using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; }

    public bool? Role { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly? Dob { get; set; }

    public string? NationalId { get; set; }

    public string Phone { get; set; } = null!;

    public int? AddressId { get; set; }

    public string? DrivingLicense { get; set; }

    public decimal? Wallet { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
