using iot_management_api.Entities.common;

namespace iot_management_api.Models
{
    public class SubjectModel
    {
        public required string Name { get; set; }
        public required SubjectType Type { get; set; }
        public required TeacherModel Teacher { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is SubjectModel model&&
                   Name==model.Name&&
                   Type==model.Type&&
                   EqualityComparer<TeacherModel>.Default.Equals(Teacher, model.Teacher);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Type, Teacher);
        }
        //public List<SubjectDataModel>? Schedules { get; set; }
    }
}
