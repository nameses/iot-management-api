using iot_management_api.Entities;

namespace iot_management_api.Models
{
    public class BookingModel
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public BookingStatus Status { get; set; }
        public DeviceModel? Device { get; set; }
        public StudentModel? Student { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is BookingModel model&&
                   Id==model.Id&&
                   Date.Equals(model.Date)&&
                   Status==model.Status&&
                   EqualityComparer<DeviceModel?>.Default.Equals(Device, model.Device)&&
                   EqualityComparer<StudentModel?>.Default.Equals(Student, model.Student);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Date, Status, Device, Student);
        }
        //public ScheduleModel? Schedule { get; set; }
    }
}
