using RootAndRot.Server.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RootAndRot.Server.Data;

public class DeviceData
{
    [Key]
    public Guid DataId { get; set; }

    [Required]
    public Guid DeviceId { get; set; }

    [Required]
    public DateTime DateTime { get; set; }

    [Required]
    public int Temperature { get; set; }

    [Required]
    public int Humidity { get; set; } 
    
    [Required]
    public int CO2 { get; set; }
    
    // Navigation property
    [ForeignKey(nameof(DeviceId))]
    public virtual Device Device { get; set; } = null!;
}