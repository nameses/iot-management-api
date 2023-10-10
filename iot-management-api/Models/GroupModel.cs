namespace iot_management_api.Models
{
    public class GroupModel
    {
        //public int Id { get; set; }
        public required string GroupCode { get; set; }
        public required int Term { get; set; }
        //public List<StudentModel>? Students { get; set; }
        //public List<ScheduleModel> Schedules { get; set; }
    }
}
