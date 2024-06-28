using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class Car
{
    public int CarId { get; set; }

    public int? UserId { get; set; }

    public string Name { get; set; } = null!;

    public string LicensePlate { get; set; } = null!;

    public int BrandId { get; set; }

    public int ModelId { get; set; }

    public int Seats { get; set; }

    public int ColorId { get; set; }

    public string FrontImage { get; set; } = null!;

    public string BackImage { get; set; } = null!;

    public string LeftImage { get; set; } = null!;

    public string RightImage { get; set; } = null!;

    public int ProductionYear { get; set; }

    public bool TransmissionType { get; set; }

    public bool FuelType { get; set; }

    public double Mileage { get; set; }

    public double FuelConsumption { get; set; }

    public decimal BasePrice { get; set; }

    public decimal Deposit { get; set; }

    public int AddressId { get; set; }

    public string? Description { get; set; }

    public int DocumentId { get; set; }

    public int TermId { get; set; }

    public int FucntionId { get; set; }

    public int Status { get; set; }

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
