using iot_management_api.Models.common;

namespace iot_management_api.Models
{
    public class TeacherModel : UserModel
    {
        public override bool Equals(object? obj)
        {
            return obj is TeacherModel model&&
                   Id==model.Id&&
                   Name==model.Name&&
                   Surname==model.Surname&&
                   Email==model.Email;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Surname, Email);
        }
    }
}
