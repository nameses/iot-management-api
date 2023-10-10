using AutoMapper;
using iot_management_api.Context;
using iot_management_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace iot_management_api.Services
{
    public interface IRoomService
    {
        Task<Room?> GetByNumberAsync(int? number);
        Task<Room?> GetByIdAsync(int? id);
        Task<int?> CreateAsync(Room entity);
        Task<bool> UpdateAsync(int id, Room entity);
        Task<bool> DeleteAsync(int id);
    }
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomService> _logger;

        public RoomService(AppDbContext context,
            IMapper mapper,
            ILogger<RoomService> logger)
        {
            _context=context;
            _mapper=mapper;
            _logger=logger;
        }
        public async Task<Room?> GetByNumberAsync(int? number)
        {
            var entity = await _context.Rooms
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Number == number);

            if (entity==null)
            {
                _logger.LogInformation($"Room(number={number}) not found");
                return null;
            }

            _logger.LogInformation($"Room by number={number} successfully found");
            return entity;
        }

        public async Task<Room?> GetByIdAsync(int? id)
        {
            if (id==null)
            {
                _logger.LogInformation($"RoomId can not be null");
                return null;
            }

            var entity = await _context.Rooms
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity==null)
            {
                _logger.LogInformation($"Room(id={id}) not found");
                return null;
            }

            _logger.LogInformation($"Room(id={id}) successfully found");
            return entity;
        }

        public async Task<int?> CreateAsync(Room entity)
        {
            if (entity==null)
            {
                _logger.LogInformation($"Room for creation is not valid");
                return null;
            }

            await _context.Rooms.AddAsync(entity);

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Created Room with ID {entity.Id}");

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(int id, Room entity)
        {
            var dbEntity = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == id);
            if (dbEntity==null)
            {
                _logger.LogInformation($"Room with ID {id} not found db");
                return false;
            }

            dbEntity.Number = entity.Number;
            dbEntity.Floor = entity.Floor;
            dbEntity.Lable = entity.Lable;


            _context.Rooms.Update(dbEntity);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dbEntity = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == id);

            if (dbEntity==null)
            {
                _logger.LogWarning($"Room with ID {id} not found db");
                return false;
            }

            _context.Rooms.Remove(dbEntity);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
