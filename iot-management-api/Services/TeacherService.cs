using iot_management_api.Context;
using iot_management_api.Entities;
using iot_management_api.Helper;
using Microsoft.EntityFrameworkCore;

namespace iot_management_api.Services
{
    public interface ITeacherService
    {
        Task<Teacher?> GetByEmail(string email);
        Task<Teacher?> GetById(int? id);
        Task<int?> CreateAsync(Teacher teacher);
    }
    public class TeacherService : ITeacherService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TeacherService> _logger;
        private readonly Encrypter _encrypter;

        public TeacherService(AppDbContext context, ILogger<TeacherService> logger, Encrypter encrypter)
        {
            _context=context;
            _logger=logger;
            _encrypter=encrypter;
        }

        public async Task<Teacher?> GetByEmail(string email)
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(x => x.Email == email);

            if (teacher==null)
            {
                _logger.LogInformation($"User(email={email}) not found");
                return null;
            }

            _logger.LogInformation($"User by email={email} successfully found");
            return teacher;
        }

        public async Task<Teacher?> GetById(int? id)
        {
            if (id==null)
            {
                _logger.LogInformation($"TeacherId can not be null");
                return null;
            }

            var teacher = await _context.Teachers.FirstOrDefaultAsync(x => x.Id == id);

            if (teacher==null)
            {
                _logger.LogInformation($"Teacher(id={id}) not found");
                return null;
            }

            _logger.LogInformation($"Teacher(id={id}) successfully found");
            return teacher;
        }

        public async Task<int?> CreateAsync(Teacher teacher)
        {
            if (teacher==null)
            {
                _logger.LogInformation($"Teacher for creation is not valid");
                throw new ArgumentNullException("Teacher for creation is not valid");
            }

            teacher.Password = _encrypter.Encrypt(teacher.Password);
            teacher.CreatedAt = DateTime.Now;

            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created teacher with ID {teacher.Id}");

            return teacher.Id;
        }
    }
}
