using System;
using System.Collections.Generic;

namespace Rental_Car_Demo.Models;

public partial class AdditionalFunction
{
    public int FucntionId { get; set; }

    public bool? Bluetooth { get; set; }

    public bool? Gps { get; set; }

    public bool? Camera { get; set; }

    public bool? SunRoof { get; set; }

    public bool? ChildLock { get; set; }

    public bool? ChildSeat { get; set; }

    public bool? Dvd { get; set; }

    public bool? Usb { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
