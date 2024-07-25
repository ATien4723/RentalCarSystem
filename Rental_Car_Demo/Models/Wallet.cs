using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class Wallet
{
    public int WalletId { get; set; }

    public int UserId { get; set; }

    public string Amount { get; set; } = null!;

    public string Type { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int? BookingNo { get; set; }

    public string? CarName { get; set; }

    public string? Note { get; set; }

    public virtual Booking? BookingNoNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
