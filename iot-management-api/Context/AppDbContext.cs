using iot_management_api.Entities;
using iot_management_api.Helper;
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
        private readonly DataSeeder _seeder;

        public AppDbContext(DbContextOptions<AppDbContext> options, DataSeeder seeder) : base(options)
        {
            _seeder=seeder;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Student>()
                .HasIndex(u => u.Email)
                .IsUnique();
            builder.Entity<Teacher>()
                .HasIndex(u => u.Email)
                .IsUnique();

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

            builder
                .Entity<Schedule>()
                .HasMany(c => c.Groups)
                .WithMany(s => s.Schedules)
                .UsingEntity<GroupSchedule>(
                   j => j
                    .HasOne(pt => pt.Group)
                    .WithMany(t => t.GroupSchedules)
                    .HasForeignKey(pt => pt.GroupId),
                    j => j
                        .HasOne(pt => pt.Schedule)
                        .WithMany(p => p.GroupSchedules)
                        .HasForeignKey(pt => pt.ScheduleId),
                    j =>
                    {
                        j.HasKey(t => new { t.GroupId, t.ScheduleId });
                        j.ToTable("GroupSchedules");
                    });


            //builder.Entity<Schedule>()
            //    .HasMany(c => c.Groups)
            //    .WithMany(s => s.Schedules)
            //    .UsingEntity(j => j.ToTable("GroupSchedules"));

            _seeder.SeedData(builder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            base.ConfigureConventions(builder);

            builder.Properties<TimeOnly>()
                .HaveConversion<TimeOnlyConverter>();

            builder.Properties<DateOnly>()
                .HaveConversion<DateOnlyConverter>();
        }
        //public DbSet<GroupSchedule> GroupSchedules { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<DayMapping> DayMappings { get; set; }
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
