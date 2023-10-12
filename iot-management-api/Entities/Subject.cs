using iot_management_api.Entities.common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iot_management_api.Entities
{
    public class Subject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required SubjectType Type { get; set; }
        public int? TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public List<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
