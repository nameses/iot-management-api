namespace iot_management_api.Models
{
    public class GroupModel
    {
        //public int Id { get; set; }
        public required string GroupCode { get; set; }
        public required int Term { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is GroupModel model&&
                   GroupCode==model.GroupCode&&
                   Term==model.Term;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GroupCode, Term);
        }
        //public List<StudentModel>? Students { get; set; }
        //public List<ScheduleModel> Schedules { get; set; }
    }
}
