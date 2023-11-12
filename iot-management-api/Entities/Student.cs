using iot_management_api.Entities.common;

namespace iot_management_api.Entities
{
    public class Student : User
    {
        public int? GroupId { get; set; }
        public Group? Group { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Student student&&
                   Id==student.Id&&
                   Name==student.Name&&
                   Surname==student.Surname&&
                   Email==student.Email&&
                   Password==student.Password&&
                   CreatedAt==student.CreatedAt&&
                   GroupId==student.GroupId&&
                   EqualityComparer<Group?>.Default.Equals(Group, student.Group);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Surname, Email, Password, CreatedAt, GroupId, Group);
        }
    }
}