using AutoMapper;
using iot_management_api.Context;
using iot_management_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace iot_management_api.Services
{
    public interface IDeviceInfoService
    {
        Task<DeviceInfo?> GetByDeviceInfo(DeviceInfo entity);
        Task<DeviceInfo?> GetById(int? id);
        Task<int?> CreateAsync(DeviceInfo entity);
        Task<bool> UpdateAsync(int id, DeviceInfo entity);
        Task<bool> DeleteAsync(int id);
    }
    public class DeviceInfoService : IDeviceInfoService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DeviceInfoService> _logger;

        public DeviceInfoService(AppDbContext context,
            IMapper mapper,
            ILogger<DeviceInfoService> logger)
        {
            _context=context;
            _mapper=mapper;
            _logger=logger;
        }
        public async Task<DeviceInfo?> GetByDeviceInfo(DeviceInfo entity)
        {
            var dbEntity = await _context.DeviceInfos
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Name == entity.Name
                    && x.Type == entity.Type
                    && x.Model == entity.Model);

            if (dbEntity==null)
            {
                _logger.LogInformation($"DeviceInfo not found");
                return null;
            }

            return entity;
        }

        public async Task<DeviceInfo?> GetById(int? id)
        {
            if (id==null)
            {
                _logger.LogInformation($"DeviceInfoId can not be null");
                return null;
            }

            var entity = await _context.DeviceInfos
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity==null)
            {
                _logger.LogInformation($"DeviceInfo(id={id}) not found");
                return null;
            }

            _logger.LogInformation($"DeviceInfo(id={id}) successfully found");
            return entity;
        }

        public async Task<int?> CreateAsync(DeviceInfo entity)
        {
            if (entity==null)
            {
                _logger.LogInformation($"DeviceInfo for creation is not valid");
                return null;
            }

            await _context.DeviceInfos.AddAsync(entity);

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Created DeviceInfo with ID {entity.Id}");

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(int id, DeviceInfo entity)
        {
            var dbEntity = await _context.DeviceInfos.FirstOrDefaultAsync(x => x.Id == id);
            if (dbEntity==null)
            {
                _logger.LogInformation($"DeviceInfo with ID {id} not found db");
                return false;
            }

            dbEntity.Type = entity.Type;
            dbEntity.Name = entity.Name;
            dbEntity.Model = entity.Model;
            dbEntity.Description = entity.Description;


            _context.DeviceInfos.Update(dbEntity);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dbEntity = await _context.DeviceInfos.FirstOrDefaultAsync(x => x.Id == id);

            if (dbEntity==null)
            {
                _logger.LogWarning($"DeviceInfo with ID {id} not found db");
                return false;
            }

            _context.DeviceInfos.Remove(dbEntity);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
