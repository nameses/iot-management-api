using System.Text.Json.Serialization;

namespace iot_management_api.Models
{
    public class DeviceModel
    {
        public required int Id { get; set; }
        public required int Amount { get; set; }
        public  DeviceInfoModel DeviceInfo { get; set; }
        [JsonIgnore]
        public RoomModel? Room { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is DeviceModel model&&
                   Id==model.Id&&
                   Amount==model.Amount&&
                   EqualityComparer<DeviceInfoModel>.Default.Equals(DeviceInfo, model.DeviceInfo)&&
                   EqualityComparer<RoomModel?>.Default.Equals(Room, model.Room);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Amount, DeviceInfo, Room);
        }
    }
}
