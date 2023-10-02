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

    public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyConverter() : base(
            dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
            dateTime => DateOnly.FromDateTime(dateTime))
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

            builder.Entity<Booking>()
                .HasOne(b => b.Student)
                .WithMany()
                .HasForeignKey(b => b.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Booking>()
                .HasOne(b => b.Schedule)
                .WithMany()
                .HasForeignKey(b => b.ScheduleId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Booking>()
                .HasOne(b => b.Device)
                .WithMany()
                .HasForeignKey(b => b.DeviceId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            base.ConfigureConventions(builder);

            builder.Properties<TimeOnly>()
                .HaveConversion<TimeOnlyConverter>();

            builder.Properties<DateOnly>()
                .HaveConversion<DateOnlyConverter>();
        }

        public DbSet<Booking> Bookings { get; set; }
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
