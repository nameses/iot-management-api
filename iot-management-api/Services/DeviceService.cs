using AutoMapper;
using iot_management_api.Context;
using iot_management_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace iot_management_api.Services
{
    public interface IDeviceService
    {
        Task<Device?> GetById(int? id);
        Task<int?> CreateAsync(Device entity);
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

        public async Task<Device?> GetById(int? id)
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

        public async Task<int?> CreateAsync(Device entity, DeviceInfo deviceInfo, int? roomNumber)
        {
            if (entity==null || deviceInfo==null || roomNumber==null)
            {
                _logger.LogInformation($"Device for creation is not valid");
                return null;
            }
            //get room by roomNumber
            var room = await _roomService.GetByNumber(roomNumber);

            if (room == null)
            {
                _logger.LogWarning($"Room with code={roomNumber} not found");
                return null;
            }
            //get deviceInfo or create new
            var dbDeviceInfo = await _deviceInfoService.GetByDeviceInfo(deviceInfo);
            if (dbDeviceInfo == null)
            {
                var id = await _deviceInfoService.CreateAsync(deviceInfo);
                if (id==null)
                {
                    _logger.LogInformation($"DeviceInfo not found and could not be created");
                    return null;
                }
                dbDeviceInfo = _mapper.Map<DeviceInfo>(deviceInfo);
                dbDeviceInfo.Id = id.Value;
            }

            await _context.Devices.AddAsync(entity);

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Created Device with ID {entity.Id}");

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(int id, Device entity)
        {
            var dbEntity = await _context.Devices.FirstOrDefaultAsync(x => x.Id == id);
            if (dbEntity==null)
            {
                _logger.LogInformation($"Device with ID {id} not found db");
                return false;
            }

            dbEntity.Amount = entity.Amount;

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
