using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iot_management_api.Entities
{
    public class DayMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SubjectNumber { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is DayMapping mapping&&
                   Id==mapping.Id&&
                   SubjectNumber==mapping.SubjectNumber&&
                   StartTime.Equals(mapping.StartTime)&&
                   EndTime.Equals(mapping.EndTime);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, SubjectNumber, StartTime, EndTime);
        }
    }
}
