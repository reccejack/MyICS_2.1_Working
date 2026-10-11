using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyICSv2._1.Models;

namespace MyICSv2._1.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Device> Devices => Set<Device>();
        public DbSet<IndustryProcess> Processes => Set<IndustryProcess>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("Devices");
                entity.HasKey(i => i.DeviceId);
                entity.Property(i => i.DeviceName)
                      .IsRequired()
                      .HasMaxLength(200);
                entity.Property(i => i.SubProcess)
                      .HasMaxLength(12);
                entity.Property(i => i.ReadingType)
                      .HasMaxLength(5);
                entity.Property(i => i.DeviceAverage)
                      .HasMaxLength(5);
                entity.Property(i => i.DeviceAddressIPv4)
                      .HasMaxLength(15);
                entity.Property(i => i.DeviceAddressIPv6)
                      .HasMaxLength(39);
                entity.Property(i => i.IsActive);
                entity.Property(i => i.DeviceStatus)
                      .HasMaxLength(20);
                entity.Property(i => i.DeviceSecurity)
                      .HasMaxLength(20);
                entity.Property(i => i.DateTime);
            });

            modelBuilder.Entity<IndustryProcess>(entity =>
            {
                entity.ToTable("Processes");
                entity.HasKey(i => i.ProcessId);
                entity.Property(i => i.ProcessStatus)
                      .HasMaxLength(200);
                entity.Property(i => i.Connection)
                      .HasMaxLength(12);
                entity.Property(i => i.IsAlarm);
                entity.Property(i => i.ProcessReport);
            });

            // Configure many-to-many relationship between Processes and Devices
            modelBuilder.Entity<IndustryProcess>()
                .HasMany(p => p.Devices)
                .WithMany(d => d.Processes)
                .UsingEntity<Dictionary<string, object>>(
                    "DeviceProcess",
                    j => j.HasOne<Device>().WithMany().HasForeignKey("DeviceId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<IndustryProcess>().WithMany().HasForeignKey("ProcessId").OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("DeviceId", "ProcessId");
                        j.ToTable("DeviceProcess");
                    });
        }
    }
}
