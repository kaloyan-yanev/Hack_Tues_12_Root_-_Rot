using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=10.245.172.43;database=hacktues;uid=Burgaski_Glarusi;pwd=Dildoto_n@_pepelqshk4", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.16-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.DeviceId).HasName("PRIMARY");

            entity.Property(e => e.DeviceId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("DeviceID");
            entity.Property(e => e.C02).HasColumnType("int(11)");
            entity.Property(e => e.HumThreshold).HasColumnName("Hum_Threshold");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(15)
                .HasColumnName("IPAddress");
            entity.Property(e => e.Macaddress)
                .HasMaxLength(17)
                .IsFixedLength()
                .HasColumnName("MACAddress");
            entity.Property(e => e.Methane).HasColumnType("int(11)");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.TempThreshold).HasColumnName("Temp_Threshold");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.Property(e => e.UserId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("UserID");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(60);

            entity.HasMany(d => d.Devices).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersDevice",
                    r => r.HasOne<Device>().WithMany()
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("UsersDevices_ibfk_2"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("UsersDevices_ibfk_1"),
                    j =>
                    {
                        j.HasKey("UserId", "DeviceId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("UsersDevices");
                        j.HasIndex(new[] { "DeviceId" }, "DeviceID");
                        j.IndexerProperty<ulong>("UserId")
                            .HasColumnType("bigint(20) unsigned")
                            .HasColumnName("UserID");
                        j.IndexerProperty<ulong>("DeviceId")
                            .HasColumnType("bigint(20) unsigned")
                            .HasColumnName("DeviceID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
