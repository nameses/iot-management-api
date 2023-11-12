using iot_management_api.Models.common;

namespace iot_management_api.Models
{
    public class ScheduleModel
    {
        public required int Id { get; set; }
        public required SubjectDataModel Subject { get; set; }
        public required PeriodModel Period { get; set; }
        public required RoomModel Room { get; set; }
        public required List<string> GroupCodes { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ScheduleModel model&&
                   Id==model.Id&&
                   EqualityComparer<SubjectDataModel>.Default.Equals(Subject, model.Subject)&&
                   EqualityComparer<PeriodModel>.Default.Equals(Period, model.Period)&&
                   EqualityComparer<RoomModel>.Default.Equals(Room, model.Room)&&
                   EqualityComparer<List<string>>.Default.Equals(GroupCodes, model.GroupCodes);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Subject, Period, Room, GroupCodes);
        }
    }
}
