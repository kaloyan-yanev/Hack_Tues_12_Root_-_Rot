using System;
using System.Collections.Generic;

namespace RootAndRot.Server.Data;

public partial class Device
{
    public Guid DeviceId { get; set; }

    public string Macaddress { get; set; } = null!;

    public string Ipaddress { get; set; } = null!;

    public string Name { get; set; } = null!;

    public float Temperature { get; set; }

    public float TempThreshold { get; set; }

    public float Humidity { get; set; }

    public float HumThreshold { get; set; }

    public int Methane { get; set; }

    public int C02 { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
