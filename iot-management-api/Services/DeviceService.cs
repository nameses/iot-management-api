using AutoMapper;
using iot_management_api.Context;
using iot_management_api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace iot_management_api.Services
{
    public interface IDeviceService
    {
        Task<bool> CheckIfDeviceAvailableAsync(int deviceId, DateOnly date, int scheduleId);
        Task<IEnumerable<Device>?> GetAvailableAsync(DateOnly date, int scheduleId);
        Task<Device?> GetByIdAsync(int? id);
        Task<IEnumerable<Device>?> GetByRoomAsync(int? room);
        Task<int?> CreateAsync(Device entity, DeviceInfo deviceInfo, int? roomNumber);
        Task<bool> UpdateAsync(int id, Device entity);
        Task<bool> DeleteAsync(int id);
    }
    public class DeviceService : IDeviceService
    {
        private readonly AppDbContext _context;
        private readonly IDeviceInfoService _deviceInfoService;
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;
        private readonly ILogger<DeviceService> _logger;

        public DeviceService(AppDbContext context,
            IDeviceInfoService deviceInfoService,
            IRoomService roomService,
            IMapper mapper,
            ILogger<DeviceService> logger)
        {
            _context=context;
            _deviceInfoService=deviceInfoService;
            _roomService=roomService;
            _mapper=mapper;
            _logger=logger;
        }

        public async Task<bool> CheckIfDeviceAvailableAsync(int deviceId, DateOnly date, int scheduleId)
        {
            //get schedule entity + room
            var schedule = await _context.Schedules
                .Include(x => x.Room)
                .FirstOrDefaultAsync(x => x.Id==scheduleId);
            if (schedule == null) return false;

            //get device from room
            var device = await _context.Devices
                .Include(x => x.DeviceInfo)
                .FirstOrDefaultAsync(x => x.Id==deviceId);
            if (device==null) return false;

            //check if schedule room and device room same
            if (device.RoomId!=schedule.RoomId) return false;

            //get bookings with devices (search by scheduleID, status and date)
            var bookings = await _context.Bookings
                .Where(x => x.ScheduleId == schedule.Id
                    && x.DeviceId == deviceId
                    && x.Date == date
                    && x.Status==BookingStatus.Approved)
                .ToListAsync();
            //dictionary DeviceId - Count(Booked devices)
            var deviceBookingCounts = bookings
                .GroupBy(b => b.DeviceId)
                .ToDictionary(
                    group => group.Key ?? 0, // Use 0 as the default key if DeviceId is null
                    group => group.Count()
                );
            //count real amount of devices and check if device available
            if (deviceBookingCounts.TryGetValue(deviceId, out int bookedCount))
                if (device.Amount-bookedCount<=0) //device.Amount = 0;
                    return false;

            return true;
        }

        public async Task<IEnumerable<Device>?> GetAvailableAsync(DateOnly date, int scheduleId)
        {
            //get schedule entity + room
            var schedule = await _context.Schedules
                .Include(x => x.Room)
                .FirstOrDefaultAsync(x => x.Id == scheduleId);
            if (schedule == null) return null;

            //get all devices from room
            IEnumerable<Device> devices = await _context.Devices
                .Include(x => x.DeviceInfo)
                .Where(x => x.RoomId==schedule.RoomId)
                .ToListAsync();
            if (devices==null) return null;

            //get bookings with devices (search by scheduleID, status and date)
            var bookings = await _context.Bookings
                .Where(x => x.ScheduleId == schedule.Id && x.Date == date && x.Status==BookingStatus.Approved)
                .ToListAsync();
            //dictionary DeviceId - Count(Booked devices)
            var deviceBookingCounts = bookings
                .GroupBy(b => b.DeviceId)
                .ToDictionary(
                    group => group.Key ?? 0, // Use 0 as the default key if DeviceId is null
                    group => group.Count()
                );
            //count real amount of devices
            foreach (var device in devices)
            {
                if (deviceBookingCounts.TryGetValue(device.Id, out int bookedCount))
                {
                    device.Amount -= bookedCount;
                }
            }

            devices = devices.Where(x => x.Amount>0);

            return devices;
        }

        public async Task<Device?> GetByIdAsync(int? id)
        {
            if (id==null)
            {
                _logger.LogInformation($"DeviceId can not be null");
                return null;
            }

            var entity = await _context.Devices
                .Include(x => x.Room)
                .Include(x => x.DeviceInfo)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity==null)
            {
                _logger.LogInformation($"Device(id={id}) not found");
                return null;
            }

            _logger.LogInformation($"Device(id={id}) successfully found");
            return entity;
        }

        public async Task<IEnumerable<Device>?> GetByRoomAsync(int? roomNumber)
        {
            if (roomNumber==null)
            {
                _logger.LogInformation($"RoomNumber can not be null");
                return null;
            }

            var entities = await _context.Devices
                .Include(x => x.DeviceInfo)
                .Include(x => x.Room)
                .Where(x => x.Room!.Number == roomNumber)
                .Select(x => new Device
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    DeviceInfo = x.DeviceInfo,
                })
                .ToListAsync();

            if (entities.IsNullOrEmpty())
            {
                _logger.LogInformation($"Devices(roomNumber={roomNumber}) not found");
                return null;
            }

            _logger.LogInformation($"Devices(roomNumber={roomNumber}) successfully found");
            return entities;
        }

        public async Task<int?> CreateAsync(Device entity, DeviceInfo deviceInfo, int? roomNumber)
        {
            if (entity==null || deviceInfo==null || roomNumber==null)
            {
                _logger.LogInformation($"Device for creation is not valid");
                return null;
            }
            //get room by roomNumber
            var room = await _roomService.GetByNumberAsync(roomNumber);

            if (room == null)
            {
                _logger.LogWarning($"Room with code={roomNumber} not found");
                return null;
            }

            entity.Room = room;

            //get deviceInfo or create new
            var dbDeviceInfo = await _deviceInfoService.GetByDeviceInfoAsync(deviceInfo);
            if (dbDeviceInfo == null)
            {
                var id = await _deviceInfoService.CreateAsync(deviceInfo);
                entity.DeviceInfoId = id;
            }
            else entity.DeviceInfoId = dbDeviceInfo.Id;

            await _context.Devices.AddAsync(entity);

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Created Device with ID {entity.Id}");

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(int id, Device entity)
        {
            var dbEntity = await _context.Devices
                .Include(x => x.DeviceInfo)
                .Include(x => x.Room)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (dbEntity==null)
            {
                _logger.LogInformation($"Device with ID {id} not found db");
                return false;
            }
            //room update
            if (entity.Room!=null && entity.Room?.Number!=null
                && entity.Room?.Number!=dbEntity.Room?.Number)
            {
                var room = await _roomService.GetByNumberAsync(entity.Room?.Number);
                if (room==null) return false;
                dbEntity.Room = room;
                dbEntity.RoomId = room.Id;
            }
            //device update
            dbEntity.Amount = entity.Amount;
            //deviceInfo update
            await _deviceInfoService.UpdateAsync(id, entity.DeviceInfo!);

            _context.Devices.Update(dbEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dbEntity = await _context.Devices.FirstOrDefaultAsync(x => x.Id == id);

            if (dbEntity==null)
            {
                _logger.LogWarning($"Device with ID {id} not found db");
                return false;
            }

            _context.Devices.Remove(dbEntity);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
