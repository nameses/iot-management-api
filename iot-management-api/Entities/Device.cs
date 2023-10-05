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
    }
}
