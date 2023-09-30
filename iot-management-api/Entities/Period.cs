using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iot_management_api.Entities
{
    public class Period
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DayEnum Day { get; set; }
        public WeekEnum Week { get; set; }
        public SemesterEnum Semester { get; set; }
        public int Year { get; set; }
        public string? Lable { get; set; }

        public enum DayEnum
        {
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
            Sunday
        }

        public enum WeekEnum
        {
            First,
            Second
        }

        public enum SemesterEnum
        {
            First,
            Second
        }
    }


}
