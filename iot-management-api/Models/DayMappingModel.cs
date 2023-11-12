namespace iot_management_api.Models
{
    public class DayMappingModel
    {
        public int SubjectNumber { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is DayMappingModel model&&
                   SubjectNumber==model.SubjectNumber&&
                   StartTime.Equals(model.StartTime)&&
                   EndTime.Equals(model.EndTime);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SubjectNumber, StartTime, EndTime);
        }
    }
}
