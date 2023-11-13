using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iot_management_api.Entities.common
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime CreatedAt { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is User user&&
                   Id==user.Id&&
                   Name==user.Name&&
                   Surname==user.Surname&&
                   Email==user.Email&&
                   Password==user.Password&&
                   CreatedAt==user.CreatedAt;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Surname, Email, Password, CreatedAt);
        }
    }
}
