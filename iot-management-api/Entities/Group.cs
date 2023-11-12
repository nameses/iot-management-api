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
        public List<GroupSchedule> GroupSchedules { get; set; } = new List<GroupSchedule>();

        public override bool Equals(object? obj)
        {
            return obj is Group group&&
                   Id==group.Id&&
                   GroupCode==group.GroupCode&&
                   Term==group.Term&&
                   EqualityComparer<List<Student>>.Default.Equals(Students, group.Students)&&
                   EqualityComparer<List<Schedule>>.Default.Equals(Schedules, group.Schedules)&&
                   EqualityComparer<List<GroupSchedule>>.Default.Equals(GroupSchedules, group.GroupSchedules);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, GroupCode, Term, Students, Schedules, GroupSchedules);
        }
    }
}
