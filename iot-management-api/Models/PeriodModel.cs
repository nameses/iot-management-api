namespace iot_management_api.Models
{
    public class PeriodModel
    {
        public int SubjectNumber { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? Lable { get; set; }
    }
}
