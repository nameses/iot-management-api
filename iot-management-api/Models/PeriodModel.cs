namespace iot_management_api.Models
{
    public class PeriodModel
    {
        public int SubjectNumber { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        /// <summary>
        /// Schedule description
        /// </summary>
        public string? Lable { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is PeriodModel model&&
                   SubjectNumber==model.SubjectNumber&&
                   StartTime.Equals(model.StartTime)&&
                   EndTime.Equals(model.EndTime)&&
                   Lable==model.Lable;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SubjectNumber, StartTime, EndTime, Lable);
        }
    }
}
