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
        //public required List<Schedule>? Schedules { get; set; }
        //public required List<Device>? Devices { get; set; }
    }
}
