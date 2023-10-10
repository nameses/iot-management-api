using iot_management_api.Entities.common;

namespace iot_management_api.Models
{
    public class SubjectModel
    {
        public required string Name { get; set; }
        public required SubjectType Type { get; set; }
        public required TeacherModel Teacher { get; set; }
        //public List<SubjectDataModel>? Schedules { get; set; }
    }
}
