using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class Booking
{
    public int BookingNo { get; set; }

    public int? UserId { get; set; }

    public int? CarId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? PaymentMethod { get; set; }

    public int? Status { get; set; }

    public int? BookingInfoId { get; set; }

    public virtual BookingInfo? BookingInfo { get; set; }

    public virtual Car? Car { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual User? User { get; set; }

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}
