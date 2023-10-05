using AutoMapper;
using iot_management_api.Context;
using iot_management_api.Entities;
using iot_management_api.Entities.common;
using Microsoft.EntityFrameworkCore;

namespace iot_management_api.Services
{
    public interface ISubjectService
    {
        Task<Subject?> GetByName(string name, SubjectType type, int teacherId);
        Task<Subject?> GetById(int? id);
        Task<int?> CreateAsync(Subject entity, int teacherId);
        Task<bool> UpdateAsync(int id, Subject entity);
        Task<bool> DeleteAsync(int id);
    }
    public class SubjectService : ISubjectService
    {
        private readonly AppDbContext _context;
        private readonly ITeacherService _teacherService;
        private readonly IMapper _mapper;
        private readonly ILogger<SubjectService> _logger;

        public SubjectService(AppDbContext context,
            ITeacherService teacherService,
            IMapper mapper,
            ILogger<SubjectService> logger)
        {
            _context=context;
            _teacherService=teacherService;
            _mapper=mapper;
            _logger=logger;
        }
        public async Task<Subject?> GetByName(string name, SubjectType type, int teacherId)
        {
            var dbEntity = await _context.Subjects
                .Where(x => x.Name == name && x.Type==type && x.TeacherId == teacherId)
                .FirstOrDefaultAsync();

            if (dbEntity==null)
                return null;

            return dbEntity;
        }

        public async Task<Subject?> GetById(int? id)
        {
            if (id==null)
            {
                _logger.LogInformation($"SubjectId can not be null");
                return null;
            }

            var entity = await _context.Subjects
                .Where(x => x.Id==id)
                .Include(x => x.Teacher)
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            if (entity==null)
            {
                _logger.LogInformation($"Subject(id={id}) not found");
                return null;
            }

            _logger.LogInformation($"Subject(id={id}) successfully found");
            return entity;
        }

        public async Task<int?> CreateAsync(Subject entity, int teacherId)
        {
            if (entity==null)
            {
                _logger.LogInformation($"Subject for creation is not valid");
                return null;
            }
            entity.TeacherId = teacherId;

            var teacher = await _teacherService.GetById(teacherId);
            if (teacher==null) return null;
            entity.Teacher = teacher;

            await _context.Subjects.AddAsync(entity);

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Created Subject with ID {entity.Id}");

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(int id, Subject entity)
        {
            var dbEntity = await _context.Subjects.FirstOrDefaultAsync(x => x.Id == id);
            if (dbEntity==null)
            {
                _logger.LogInformation($"Subject with ID {id} not found db");
                return false;
            }

            dbEntity.Name = entity.Name;
            dbEntity.Type = entity.Type;

            _context.Subjects.Update(dbEntity);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dbEntity = await _context.Subjects.FirstOrDefaultAsync(x => x.Id == id);

            if (dbEntity==null)
            {
                _logger.LogWarning($"Subject with ID {id} not found db");
                return false;
            }

            _context.Subjects.Remove(dbEntity);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
