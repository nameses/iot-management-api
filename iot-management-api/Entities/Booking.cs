using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iot_management_api.Entities
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required DateOnly Date { get; set; }
        public required BookingStatus Status { get; set; }
        public int? DeviceId { get; set; }
        public Device? Device { get; set; }
        public int? StudentId { get; set; }
        public Student? Student { get; set; }
        public int? ScheduleId { get; set; }
        public Schedule? Schedule { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Booking booking&&
                   Id==booking.Id&&
                   Date.Equals(booking.Date)&&
                   Status==booking.Status&&
                   DeviceId==booking.DeviceId&&
                   EqualityComparer<Device?>.Default.Equals(Device, booking.Device)&&
                   StudentId==booking.StudentId&&
                   EqualityComparer<Student?>.Default.Equals(Student, booking.Student)&&
                   ScheduleId==booking.ScheduleId&&
                   EqualityComparer<Schedule?>.Default.Equals(Schedule, booking.Schedule);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(Date);
            hash.Add(Status);
            hash.Add(DeviceId);
            hash.Add(Device);
            hash.Add(StudentId);
            hash.Add(Student);
            hash.Add(ScheduleId);
            hash.Add(Schedule);
            return hash.ToHashCode();
        }
    }
}
