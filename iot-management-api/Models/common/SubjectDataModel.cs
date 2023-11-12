using iot_management_api.Entities.common;

namespace iot_management_api.Models.common
{
    public class SubjectDataModel
    {
        public required string Name { get; set; }
        public required SubjectType Type { get; set; }
        public required string TeacherFullName { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is SubjectDataModel model&&
                   Name==model.Name&&
                   Type==model.Type&&
                   TeacherFullName==model.TeacherFullName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Type, TeacherFullName);
        }
    }
}
