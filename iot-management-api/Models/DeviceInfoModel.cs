namespace iot_management_api.Models
{
    public class DeviceInfoModel
    {
        public required string Type { get; set; }
        public required string Name { get; set; }
        public required string Model { get; set; }
        public required string Description { get; set; }
    }
}
