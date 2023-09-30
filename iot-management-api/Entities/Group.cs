using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iot_management_api.Entities
{
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string GroupCode { get; set; }
        public required int Term { get; set; }
        public List<Student> Students { get; set; } = new List<Student>();
        public List<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
