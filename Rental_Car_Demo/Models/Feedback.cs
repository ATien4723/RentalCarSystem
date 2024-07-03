using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int? BookingNo { get; set; }

    public int Ratings { get; set; }

    public string? Content { get; set; }

    public DateOnly? Date { get; set; }

    public virtual Booking? BookingNoNavigation { get; set; }
}
