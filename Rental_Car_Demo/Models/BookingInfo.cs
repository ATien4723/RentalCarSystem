using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rental_Car_Demo.Models;

public partial class BookingInfo
{
    public int BookingInfoId { get; set; }

    [Required(ErrorMessage = "This field is required.")]
    [EmailAddress]
    [MaxLength(50)]
    [RegularExpression(@"^[^@\s]+@gmail\.com$", ErrorMessage = "Email must be a valid gmail.com address.")]
    public string RenterEmail { get; set; } = null!;

    public string RenterName { get; set; } = null!;

    public DateOnly RenterDob { get; set; }

    public int RenterNationalId { get; set; }

    [Required(ErrorMessage = "This field is required.")]
    [RegularExpression(@"^0[35789]\d{8}$", ErrorMessage = "Phone number invalid!")]
    public string RenterPhone { get; set; } = null!;

    public int? RenterAddressId { get; set; }

    public string? RenterDrivingLicense { get; set; }

    public bool IsDifferent { get; set; } = false;

    [Required(ErrorMessage = "This field is required.")]
    [EmailAddress]
    [MaxLength(50)]
    [RegularExpression(@"^[^@\s]+@gmail\.com$", ErrorMessage = "Email must be a valid gmail.com address.")]
    public string DriverEmail { get; set; } = null!;

    public string DriverName { get; set; } = null!;

    public DateOnly DriverDob { get; set; }

    public int DriverNationalId { get; set; }

    [Required(ErrorMessage = "This field is required.")]
    [RegularExpression(@"^0[35789]\d{8}$", ErrorMessage = "Phone number invalid!")]
    public string DriverPhone { get; set; } = null!;

    public int? DriverAddressId { get; set; }

    public string? DriverDrivingLicense { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Address? DriverAddress { get; set; }

    public virtual Address? RenterAddress { get; set; }
}
