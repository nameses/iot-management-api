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

        public override bool Equals(object? obj)
        {
            return obj is StudentModel model&&
                   Id==model.Id&&
                   Name==model.Name&&
                   Surname==model.Surname&&
                   Email==model.Email&&
                   GroupCode==model.GroupCode;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Surname, Email, GroupCode);
        }
    }
}
