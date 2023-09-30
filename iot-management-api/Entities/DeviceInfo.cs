using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iot_management_api.Entities
{
    public class DeviceInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Type { get; set; }
        public required string Name { get; set; }
        public required string Model { get; set; }
        public required string Description { get; set; }

    }
}
