using iot_management_api.Entities;

namespace iot_management_api.Models
{
    public class ScheduleModel
    {
        public List<GroupModel> Groups { get; set; } = new List<GroupModel>();
        public required SubjectModel Subject { get; set; }
        public required Period Period { get; set; }
        public required RoomModel Room { get; set; }
    }
}
