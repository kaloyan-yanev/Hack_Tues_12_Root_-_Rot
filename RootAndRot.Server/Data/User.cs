using System;
using System.Collections.Generic;

namespace RootAndRot.Server.Data;

public partial class User
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}
