using iot_management_api.Context;
using iot_management_api.Entities;
using iot_management_api.Helper;
using Microsoft.EntityFrameworkCore;

namespace iot_management_api.Services
{
    public interface IStudentService
    {
        Task<Student?> GetByEmail(string email);
        Task<Student?> GetById(int? id);
        Task<int?> CreateAsync(Student student, string? groupCode);
    }
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;
        private readonly IGroupService _groupService;
        private readonly ILogger<StudentService> _logger;
        private readonly Encrypter _encrypter;

        public StudentService(AppDbContext context,
            IGroupService groupService,
            ILogger<StudentService> logger,
            Encrypter encrypter)
        {
            _context=context;
            _groupService=groupService;
            _logger=logger;
            _encrypter=encrypter;
        }
        public async Task<Student?> GetByEmail(string email)
        {
            var student = await _context.Students
                .Include(x => x.Group)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Email == email);

            if (student==null)
            {
                _logger.LogInformation($"Student(email={email}) not found");
                return null;
            }

            _logger.LogInformation($"Student by email={email} successfully found");
            return student;
        }
        public async Task<Student?> GetById(int? id)
        {
            if (id==null)
            {
                _logger.LogInformation($"StudentId can not be null");
                return null;
            }

            var student = await _context.Students
                .Include(x => x.Group)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (student==null)
            {
                _logger.LogInformation($"Student(id={id}) not found");
                return null;
            }

            _logger.LogInformation($"Student(id={id}) successfully found");
            return student;
        }

        public async Task<int?> CreateAsync(Student student, string? groupCode)
        {
            if (student==null)
            {
                _logger.LogInformation($"Student for creation is not valid");
                throw new ArgumentNullException("Student for creation is not valid");
            }

            student.Password = _encrypter.Encrypt(student.Password);
            student.CreatedAt = DateTime.Now;

            var group = await _groupService.GetByGroupCode(groupCode);

            if (group == null)
            {
                _logger.LogWarning($"Group with code={groupCode} not found");
                return null;
            }

            student.Group = group;
            await _context.Students.AddAsync(student);

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Created user with ID {student.Id}");

            return student.Id;
        }
    }
}
