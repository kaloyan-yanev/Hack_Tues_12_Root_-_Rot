using RootAndRot.Server.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RootAndRot.Server.Data;

public class RefreshToken
{
    [Key]
    public Guid RefreshTokenId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Token { get; set; } = null!;

    [Required]
    public DateTime ExpiresAt { get; set; }

    [Required]
    public bool Consumed { get; set; } = false;

    // Navigation property
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
}