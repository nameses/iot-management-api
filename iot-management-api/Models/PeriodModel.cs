namespace iot_management_api.Models
{
    public class PeriodModel
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? Lable { get; set; }
    }
}
