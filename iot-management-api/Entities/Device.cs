using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iot_management_api.Entities
{
    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required int Amount { get; set; }
        public int? DeviceInfoId { get; set; }
        public DeviceInfo? DeviceInfo { get; set; }
        public int? RoomId { get; set; }
        public Room? Room { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Device device&&
                   Id==device.Id&&
                   Amount==device.Amount&&
                   DeviceInfoId==device.DeviceInfoId&&
                   EqualityComparer<DeviceInfo?>.Default.Equals(DeviceInfo, device.DeviceInfo)&&
                   RoomId==device.RoomId&&
                   EqualityComparer<Room?>.Default.Equals(Room, device.Room);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Amount, DeviceInfoId, DeviceInfo, RoomId, Room);
        }
    }
}
