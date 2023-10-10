using AutoMapper;
using iot_management_api.Context;
using iot_management_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace iot_management_api.Services
{
    public interface IDayMappingService
    {
        Task<IEnumerable<DayMapping>?> GetAllAsync();
    }
    public class DayMappingService : IDayMappingService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DayMappingService> _logger;

        public DayMappingService(AppDbContext context,
            IMapper mapper,
            ILogger<DayMappingService> logger)
        {
            _context=context;
            _mapper=mapper;
            _logger=logger;
        }

        public async Task<IEnumerable<DayMapping>?> GetAllAsync()
        {
            var entities = await _context.DayMappings.ToListAsync();
            if (entities==null)
            {
                _logger.LogWarning("No Day Mappings found");
                return null;
            }

            return entities;
        }
    }
}
