using iot_management_api.Entities.common;

namespace iot_management_api.Entities
{
    public class Teacher : User
    {

        public List<Subject> Subjects { get; set; } = new List<Subject>();
    }
}
