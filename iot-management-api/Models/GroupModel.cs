namespace iot_management_api.Models
{
    public class GroupModel
    {
        public int Id { get; set; }
        public required string GroupCode { get; set; }
        public required int Term { get; set; }
        public List<StudentModel> Students { get; set; } = new List<StudentModel>();
        //public List<ScheduleModel> Schedules { get; set; }
    }
}
