namespace iot_management_api.Models
{
    public class DeviceModel
    {
        public required int Amount { get; set; }
        public required DeviceInfoModel DeviceInfo { get; set; }
        public required RoomModel Room { get; set; }
    }
}
