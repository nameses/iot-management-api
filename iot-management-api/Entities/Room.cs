using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iot_management_api.Entities
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Number { get; set; }
        public int Floor { get; set; }
        public string? Lable { get; set; }
        public required List<Schedule> Schedules { get; set; } = new List<Schedule>();
        public required List<Device> Devices { get; set; } = new List<Device>();
    }
}
