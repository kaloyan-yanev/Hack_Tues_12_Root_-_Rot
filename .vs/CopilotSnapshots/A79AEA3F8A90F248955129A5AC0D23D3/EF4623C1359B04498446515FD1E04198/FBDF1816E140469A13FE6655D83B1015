using RootAndRot.Server.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RootAndRot.Server.Models
{
    public class DeviceDataDTO
    {
        public Guid DeviceId { get; set; }
        public string Mac { get; set; }
        public float Temp { get; set; }
        public float Humidity { get; set; }
        public int Methane { get; set; }
        public int Co2 { get; set; }
        public float Progress { get; set; }
        public bool DoesntHaveMeatOrDairy { get; set; }
        public bool HasMeat { get; set; }
        public bool HasDairy { get; set; }
        public static DeviceDataDTO FromDevice(Device device)
        {
            return new DeviceDataDTO
            {
                DeviceId = device.DeviceId,
                Mac = device.Macaddress,
                Temp = device.Temperature,
                Humidity = device.Humidity,
                Methane = device.Methane,
                Co2 = device.CO2,
                DoesntHaveMeatOrDairy = device.DoesntHaveMeatOrDairy,
                HasMeat = device.HasMeat,
                HasDairy = device.HasDairy,
                Progress = CalculateProgress(device)
            };
        }

        public static float CalculateProgress(Device device)
        {
            if (device == null)
                return 0f;

            float tempScore = GetTemperatureScore(device.Temperature);
            float humidityScore = GetHumidityScore(device.Humidity);
            float activityScore = GetActivityScore(device.CO2);
            float methanePenalty = GetMethanePenalty(device.Methane);
            float compositionPenalty = GetCompositionPenalty(device);

            // Weighted score (tuned for composting reality)
            float progress =
                (tempScore * 0.35f) +
                (humidityScore * 0.25f) +
                (activityScore * 0.25f) -
                (methanePenalty * 0.10f) -
                (compositionPenalty * 0.05f);

            return Math.Max(0, Math.Min(100, progress));
        }
        private static float GetTemperatureScore(float temp)
        {
            if (temp < 20) return 10;        // too cold
            if (temp < 40) return 40;        // warming up
            if (temp <= 65) return 100;      // 🔥 optimal thermophilic
            if (temp <= 75) return 70;       // too hot but still active
            return 30;                       // overheating = bad bacteria die
        }
        private static float GetHumidityScore(float humidity)
        {
            if (humidity < 30) return 20;   // too dry
            if (humidity < 50) return 60;   // acceptable
            if (humidity <= 70) return 100; // optimal
            if (humidity <= 85) return 70;  // too wet
            return 30;                      // risk of anaerobic
        }
        private static float GetActivityScore(int co2)
        {
            if (co2 < 400) return 10;     // no activity
            if (co2 < 1000) return 50;    // moderate
            if (co2 < 3000) return 100;   // 🔥 strong decomposition
            return 70;                    // too much = imbalance
        }
        private static float GetMethanePenalty(int methane)
        {
            if (methane < 100) return 0;
            if (methane < 500) return 20;
            if (methane < 1000) return 50;
            return 80;
        }
        private static float GetCompositionPenalty(Device device)
        {
            float penalty = 0;

            if (device.HasMeat)
                penalty += 30;

            if (device.HasDairy)
                penalty += 20;

            return penalty;
        }

    }
}
