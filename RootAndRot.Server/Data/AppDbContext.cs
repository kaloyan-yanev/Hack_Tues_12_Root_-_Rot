using Microsoft.EntityFrameworkCore;

namespace RootAndRot.Server.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Device> Devices { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql(
            "server=localhost;database=hacktues;uid=root;pwd=1234",
            ServerVersion.Parse("10.11.16-mariadb")
        );

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.DeviceId);

            entity.Property(e => e.DeviceId)
                .HasColumnType("char(36)")
                .HasColumnName("DeviceID")
                .HasDefaultValueSql("(UUID())");

            entity.Property(e => e.Macaddress)
                .HasMaxLength(50)
                .HasColumnName("MACAddress");

            entity.Property(e => e.TempThreshold)
                .HasColumnName("Temp_Threshold")
                .HasDefaultValueSql("30");

            entity.Property(e => e.Methane)
                .HasColumnType("int");

            entity.Property(e => e.CO2)
                .HasColumnType("int");
        });
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.UserId)
                .HasColumnType("char(36)")
                .HasColumnName("UserID")
                .HasDefaultValueSql("(UUID())");

            entity.Property(e => e.Name)
                .HasMaxLength(50);

            entity.Property(e => e.Password)
                .HasMaxLength(100);
            entity.HasMany(u => u.Devices)
                .WithMany(d => d.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersDevices",
                    r => r.HasOne<Device>()
                        .WithMany()
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade),

                    l => l.HasOne<User>()
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade),

                    j =>
                    {
                        j.HasKey("UserId", "DeviceId");

                        j.ToTable("UsersDevices");

                        j.IndexerProperty<Guid>("UserId")
                            .HasColumnType("char(36)")
                            .HasColumnName("UserID");

                        j.IndexerProperty<Guid>("DeviceId")
                            .HasColumnType("char(36)")
                            .HasColumnName("DeviceID");
                    });
        });
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.RefreshTokenId);

            entity.Property(rt => rt.RefreshTokenId)
                .HasMaxLength(100);

            entity.Property(rt => rt.UserId)
                .IsRequired();

            entity.Property(rt => rt.ExpiresAt)
                .IsRequired();
            entity.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}