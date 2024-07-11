using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rental_Car_Demo.Models;

public partial class Car
{
    public int CarId { get; set; }

    public int? UserId { get; set; }

    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "LicensePlate is not empty!")]
    [RegularExpression(@"^(1[1-9]|[2-9][0-9])[A-Z](-\d{3}\.\d{2}|-\d{4})$", ErrorMessage = "Must follow format, e.g., 50F-567.89 or 50F-5678")]
    public string LicensePlate { get; set; } = null!;
    
    public int BrandId { get; set; }

    public int ModelId { get; set; }
    [Range(1, 49, ErrorMessage = "Seats must be a integer in range 1-49")]

    public int Seats { get; set; }

    public int ColorId { get; set; }

    public string FrontImage { get; set; } = null!;

    public string BackImage { get; set; } = null!;

    public string LeftImage { get; set; } = null!;

    public string RightImage { get; set; } = null!;

    public int ProductionYear { get; set; }

    public bool TransmissionType { get; set; }

    public bool FuelType { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Mileage can not less than 0")]
    public double Mileage { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "FuelConsumption must be greater than 0!")]
    public double FuelConsumption { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "BasePrice must be greater than 0.")]
    public decimal BasePrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Deposit must be a positive number.")]
    public decimal Deposit { get; set; }

    public int AddressId { get; set; }

    [Required(ErrorMessage = "Description is not empty!")]
    public string? Description { get; set; }

    public int DocumentId { get; set; }

    public int TermId { get; set; }

    public int FucntionId { get; set; }

    public int Status { get; set; }

    public int NoOfRide { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual CarBrand Brand { get; set; } = null!;

    public virtual CarColor Color { get; set; } = null!;

    public virtual CarDocument Document { get; set; } = null!;

    public virtual AdditionalFunction Fucntion { get; set; } = null!;

    public virtual CarModel Model { get; set; } = null!;

    public virtual TermOfUse Term { get; set; } = null!;

    public virtual User? User { get; set; }
}
