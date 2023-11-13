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
        public List<Schedule> Schedules { get; set; } = new List<Schedule>();
        public List<Device> Devices { get; set; } = new List<Device>();

        public override bool Equals(object? obj)
        {
            return obj is Room room&&
                   Id==room.Id&&
                   Number==room.Number&&
                   Floor==room.Floor&&
                   Lable==room.Lable&&
                   EqualityComparer<List<Schedule>>.Default.Equals(Schedules, room.Schedules)&&
                   EqualityComparer<List<Device>>.Default.Equals(Devices, room.Devices);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Number, Floor, Lable, Schedules, Devices);
        }
    }
}
