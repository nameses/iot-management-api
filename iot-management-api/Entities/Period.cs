using iot_management_api.Entities.common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iot_management_api.Entities
{
    public class Period
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DayEnum Day { get; set; }
        public WeekEnum Week { get; set; }
        public SemesterEnum Semester { get; set; }
        public int Year { get; set; }
        public string? Lable { get; set; }
        public int? DayMappingId { get; set; }
        public DayMapping? DayMapping { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Period period&&
                   Id==period.Id&&
                   Day==period.Day&&
                   Week==period.Week&&
                   Semester==period.Semester&&
                   Year==period.Year&&
                   Lable==period.Lable&&
                   DayMappingId==period.DayMappingId&&
                   EqualityComparer<DayMapping?>.Default.Equals(DayMapping, period.DayMapping);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Day, Week, Semester, Year, Lable, DayMappingId, DayMapping);
        }
    }
}
