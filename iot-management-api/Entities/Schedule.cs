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

        public override bool Equals(object? obj)
        {
            return obj is Schedule schedule&&
                   Id==schedule.Id&&
                   EqualityComparer<List<Group>>.Default.Equals(Groups, schedule.Groups)&&
                   EqualityComparer<List<GroupSchedule>>.Default.Equals(GroupSchedules, schedule.GroupSchedules)&&
                   SubjectId==schedule.SubjectId&&
                   EqualityComparer<Subject?>.Default.Equals(Subject, schedule.Subject)&&
                   PeriodId==schedule.PeriodId&&
                   EqualityComparer<Period?>.Default.Equals(Period, schedule.Period)&&
                   RoomId==schedule.RoomId&&
                   EqualityComparer<Room?>.Default.Equals(Room, schedule.Room);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(Groups);
            hash.Add(GroupSchedules);
            hash.Add(SubjectId);
            hash.Add(Subject);
            hash.Add(PeriodId);
            hash.Add(Period);
            hash.Add(RoomId);
            hash.Add(Room);
            return hash.ToHashCode();
        }
    }
}
