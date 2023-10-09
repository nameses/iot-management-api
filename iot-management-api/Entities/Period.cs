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
    }
}
