using iot_management_api.Context;
using iot_management_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace iot_management_api.Services
{
    public interface IGroupService
    {
        Task<Group?> GetByGroupCode(string? groupCode);
        Task<Group?> GetById(int? id);
        Task<int?> CreateAsync(Group group);
    }
    public class GroupService : IGroupService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<GroupService> _logger;

        public GroupService(AppDbContext context,
            ILogger<GroupService> logger)
        {
            _context=context;
            _logger=logger;
        }
        public async Task<Group?> GetByGroupCode(string? groupCode)
        {
            var group = await _context.Groups
                .Include(x => x.Students)
                .Include(x => x.Schedules)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.GroupCode == groupCode);

            if (group==null)
            {
                _logger.LogInformation($"Group(groupCode={groupCode}) not found");
                return null;
            }

            _logger.LogInformation($"Group by groupCode={groupCode} successfully found");
            return group;
        }

        public async Task<Group?> GetById(int? id)
        {
            if (id==null)
            {
                _logger.LogInformation($"GroupId can not be null");
                return null;
            }

            var group = await _context.Groups
                .Include(x => x.Students)
                .Include(x => x.Schedules)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (group==null)
            {
                _logger.LogInformation($"Group(id={id}) not found");
                return null;
            }

            _logger.LogInformation($"Group(id={id}) successfully found");
            return group;
        }

        public async Task<int?> CreateAsync(Group group)
        {
            if (group==null)
            {
                _logger.LogInformation($"Group for creation is not valid");
                return null;
            }

            await _context.Groups.AddAsync(group);

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Created group with ID {group.Id}");

            return group.Id;
        }
    }
}
