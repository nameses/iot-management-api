using iot_management_api.Models.common;

namespace iot_management_api.Models
{

    public class StudentModel : UserModel
    {
        public required string GroupCode { get; set; }
    }
}
