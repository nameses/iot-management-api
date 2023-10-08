using iot_management_api.Entities.common;

namespace iot_management_api.Models.common
{
    public class SubjectDataModel
    {
        public required string Name { get; set; }
        public required SubjectType Type { get; set; }
        public required string TeacherFullName { get; set; }

    }
}
