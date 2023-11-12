using iot_management_api.Entities.common;

namespace iot_management_api.Entities
{
    public class Teacher : User
    {

        public List<Subject> Subjects { get; set; } = new List<Subject>();

        public override bool Equals(object? obj)
        {
            return obj is Teacher teacher&&
                   Id==teacher.Id&&
                   Name==teacher.Name&&
                   Surname==teacher.Surname&&
                   Email==teacher.Email&&
                   Password==teacher.Password&&
                   CreatedAt==teacher.CreatedAt&&
                   EqualityComparer<List<Subject>>.Default.Equals(Subjects, teacher.Subjects);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Surname, Email, Password, CreatedAt, Subjects);
        }
    }
}
