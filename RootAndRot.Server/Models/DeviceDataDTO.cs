using RootAndRot.Server.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RootAndRot.Server.Models
{
    public class DeviceDataDTO
    {
        public string MAC { get; set; }
        public float Temp  { get; set; }
        public float Humidity { get; set; }
        public int Methane { get; set; }
        public int CO2 { get; set; }
        public float progress { get; set; }

        public static DeviceDataDTO FromDevice(Device device)
        {
            return new DeviceDataDTO
            {
                MAC = device.Macaddress,
                Temp = device.Temperature,
                Humidity = device.Humidity,
                Methane = device.Methane,
                CO2 = device.CO2,
                progress = CalculateProgress()
            };
        }

        public static float CalculateProgress()
        {
            throw new NotImplementedException();
        }
    }
}
