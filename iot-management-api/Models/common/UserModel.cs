namespace iot_management_api.Models.common
{
    public class UserModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is UserModel model&&
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
