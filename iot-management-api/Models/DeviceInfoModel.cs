namespace iot_management_api.Models
{
    public class DeviceInfoModel
    {
        public required string Type { get; set; }
        public required string Name { get; set; }
        public required string Model { get; set; }
        public required string Description { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is DeviceInfoModel model&&
                   Type==model.Type&&
                   Name==model.Name&&
                   Model==model.Model&&
                   Description==model.Description;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Name, Model, Description);
        }
    }
}
