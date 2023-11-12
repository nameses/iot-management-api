using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iot_management_api.Entities
{
    public class GroupSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? GroupId { get; set; }
        public Group? Group { get; set; }
        public int? ScheduleId { get; set; }
        public Schedule? Schedule { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is GroupSchedule schedule&&
                   Id==schedule.Id&&
                   GroupId==schedule.GroupId&&
                   EqualityComparer<Group?>.Default.Equals(Group, schedule.Group)&&
                   ScheduleId==schedule.ScheduleId&&
                   EqualityComparer<Schedule?>.Default.Equals(Schedule, schedule.Schedule);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, GroupId, Group, ScheduleId, Schedule);
        }
    }
}
