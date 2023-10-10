using System.Text.Json.Serialization;

namespace iot_management_api.Models
{
    public class DeviceModel
    {
        public required int Id { get; set; }
        public required int Amount { get; set; }
        public required DeviceInfoModel DeviceInfo { get; set; }
        [JsonIgnore]
        public RoomModel? Room { get; set; }
    }
}
