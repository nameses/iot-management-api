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
        public List<GroupSchedule> GroupSchedules { get; set; } = new List<GroupSchedule>();
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public int? PeriodId { get; set; }
        public Period? Period { get; set; }
        public int? RoomId { get; set; }
        public Room? Room { get; set; }
    }
}
