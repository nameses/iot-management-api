namespace iot_management_api.Models
{
    public class RoomModel
    {
        public int Number { get; set; }
        /// <summary>
        /// Floor number, first digit of Room Number
        /// </summary>
        public int Floor { get; set; }
        public string? Lable { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is RoomModel model&&
                   Number==model.Number&&
                   Floor==model.Floor&&
                   Lable==model.Lable;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Number, Floor, Lable);
        }
        //public required List<Schedule>? Schedules { get; set; }
        //public required List<Device>? Devices { get; set; }
    }
}
