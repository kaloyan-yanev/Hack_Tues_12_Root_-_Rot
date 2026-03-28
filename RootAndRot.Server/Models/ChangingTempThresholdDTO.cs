using System.ComponentModel.DataAnnotations;

namespace RootAndRot.Server.Models
{
    public class ChangingTempThresholdDTO
    {
        [Required]
        public Guid DeviceId { get; set; }

        [Required]
        public bool HasVegetables { get; set; }

        [Required]
        public bool HasMeat { get; set; }

        [Required]
        public bool HasDairy { get; set; }
    }
}
