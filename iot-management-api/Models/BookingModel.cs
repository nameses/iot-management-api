namespace iot_management_api.Models
{
    public class BookingModel
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        //public BookingStatus Status { get; set; }
        public DeviceModel? Device { get; set; }
        public StudentModel? Student { get; set; }
        //public ScheduleModel? Schedule { get; set; }
    }
}
