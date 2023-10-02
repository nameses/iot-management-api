using iot_management_api.Entities.common;

namespace iot_management_api.Entities
{
    public class Student : User
    {
        public required Group Group { get; set; }
    }
}