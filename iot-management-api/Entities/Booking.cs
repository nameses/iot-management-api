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
    }
}
