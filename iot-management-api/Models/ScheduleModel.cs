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

    }
}
