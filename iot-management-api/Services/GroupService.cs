using AutoMapper;
using iot_management_api.Context;
using iot_management_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace iot_management_api.Services
{
    public interface IGroupService
    {
        Task<Group?> GetByGroupCode(string? groupCode);
        Task<Group?> GetById(int? id);
        Task<int?> CreateAsync(Group entity);
        Task<bool> UpdateAsync(int id, Group entity);
        Task<bool> DeleteAsync(int id);
    }
    public class GroupService : IGroupService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GroupService> _logger;

        public GroupService(AppDbContext context,
            IMapper mapper,
            ILogger<GroupService> logger)
        {
            _context=context;
            _mapper=mapper;
            _logger=logger;
        }
        public async Task<Group?> GetByGroupCode(string? groupCode)
        {
            var entity = await _context.Groups
                .Include(x => x.Students)
                .Include(x => x.Schedules)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.GroupCode == groupCode);

            if (entity==null)
            {
                _logger.LogInformation($"Group(groupCode={groupCode}) not found");
                return null;
            }

            _logger.LogInformation($"Group by groupCode={groupCode} successfully found");
            return entity;
        }

        public async Task<Group?> GetById(int? id)
        {
            if (id==null)
            {
                _logger.LogInformation($"GroupId can not be null");
                return null;
            }

            var entity = await _context.Groups
                .Include(x => x.Students)
                .Include(x => x.Schedules)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity==null)
            {
                _logger.LogInformation($"Group(id={id}) not found");
                return null;
            }

            _logger.LogInformation($"Group(id={id}) successfully found");
            return entity;
        }

        public async Task<int?> CreateAsync(Group entity)
        {
            if (entity==null)
            {
                _logger.LogInformation($"Group for creation is not valid");
                return null;
            }

            await _context.Groups.AddAsync(entity);

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Created group with ID {entity.Id}");

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(int id, Group entity)
        {
            var dbEntity = await _context.Groups.FirstOrDefaultAsync(x => x.Id == id);
            if (dbEntity==null)
            {
                _logger.LogInformation($"Group with ID {id} not found db");
                return false;
            }

            dbEntity.GroupCode = entity.GroupCode;
            dbEntity.Term = entity.Term;

            _context.Groups.Update(dbEntity);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dbEntity = await _context.Groups.FirstOrDefaultAsync(x => x.Id == id);

            if (dbEntity==null)
            {
                _logger.LogWarning($"Group with ID {id} not found db");
                return false;
            }

            _context.Groups.Remove(dbEntity);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
