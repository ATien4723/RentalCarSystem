using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class Booking
{
    public int BookingNo { get; set; }

    public int? UserId { get; set; }

    public int? CarId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int? DriverId { get; set; }

    public int? PaymentMethod { get; set; }

    public int? Status { get; set; }

    public virtual Car? Car { get; set; }

    public virtual Driver? Driver { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual User? User { get; set; }

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}
