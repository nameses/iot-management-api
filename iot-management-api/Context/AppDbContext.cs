using iot_management_api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace iot_management_api.Context
{
    public class TimeOnlyConverter : ValueConverter<TimeOnly, TimeSpan>
    {
        public TimeOnlyConverter() : base(
            timeOnly => timeOnly.ToTimeSpan(),
            timeSpan => TimeOnly.FromTimeSpan(timeSpan))
        { }
    }

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Group>()
                .HasIndex(u => u.GroupCode)
                .IsUnique();

            builder.Entity<Room>()
                .HasIndex(u => u.Number)
                .IsUnique();
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            base.ConfigureConventions(builder);

            builder.Properties<TimeOnly>()
                .HaveConversion<TimeOnlyConverter>();
        }

        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceInfo> DeviceInfos { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Teacher> Teachers { get; set; }

    }
}
