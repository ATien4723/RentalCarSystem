using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class TokenInfor
{
    public int Id { get; set; }

    public string? Token { get; set; }

    public int? UserId { get; set; }

    public DateTime? ExpirationTime { get; set; }

    public bool? IsLocked { get; set; }
}
