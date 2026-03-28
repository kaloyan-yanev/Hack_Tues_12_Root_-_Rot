using System.ComponentModel.DataAnnotations;

namespace RootAndRot.Server.Models
{
    public class AddingDeviceDTO
    {
        public string MACAddress { get; set; } = null!;
    }
}
