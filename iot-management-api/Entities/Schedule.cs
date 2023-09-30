using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iot_management_api.Entities
{
    public class Schedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public List<Group> Groups { get; set; } = new List<Group>();
        public required Subject Subject { get; set; }
        public required Period Period { get; set; }
        public required Room Room { get; set; }
    }
}
