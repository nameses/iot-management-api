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
        public int? DeviceId { get; set; }
        public required Device Device { get; set; }
        public int? StudentId { get; set; }
        public required Student Student { get; set; }
        public int? ScheduleId { get; set; }
        public required Schedule Schedule { get; set; }

    }
}
