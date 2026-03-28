using System.ComponentModel.DataAnnotations;

namespace RootAndRot.Server.Models
{
    public class StirTorDTO
    {
        [Required]
        public Guid DeviceId { get; set; }
    }
}
