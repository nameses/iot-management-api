using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iot_management_api.Entities
{
    public class GroupSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int GroupId { get; set; }
        public Group? Group { get; set; }
        public int ScheduleId { get; set; }
        public Schedule? Schedule { get; set; }

    }
}
