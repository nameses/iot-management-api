using iot_management_api.Models.common;

namespace iot_management_api.Models
{

    public class StudentModel : UserModel
    {
        /// <summary>
        /// Group code
        /// </summary>
        /// <example>TR-12,TV-23</example>
        public required string GroupCode { get; set; }
    }
}
