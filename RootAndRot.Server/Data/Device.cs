namespace RootAndRot.Server.Data;

public class Device
{
    public Guid DeviceId { get; set; }

    public string Macaddress { get; set; } = null!;

    public float Temperature { get; set; }

    public float TempThreshold { get; set; }

    public float Humidity { get; set; }

    public float HumThreshold { get; set; }

    public int Methane { get; set; }

    public int CO2 { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
    
    public virtual ICollection<DeviceData> DeviceDataSet { get; set; } = new List<DeviceData>();
}