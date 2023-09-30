using iot_management_api.Context;
using iot_management_api.Entities;
using iot_management_api.Entities.common;
using iot_management_api.Helper;
using Microsoft.EntityFrameworkCore;

namespace iot_management_api.Services
{
    public interface IUserService
    {
        Task<User?> GetByEmail(string email, UserRole userRole);
        Task<User?> GetById(int? id);
        Task<int?> CreateAsync(User user, string? groupCode);
    }
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly Encrypter _encrypter;

        public UserService(AppDbContext context, ILogger<UserService> logger, Encrypter encrypter)
        {
            _context=context;
            _logger=logger;
            _encrypter=encrypter;
        }
        public async Task<User?> GetByEmail(string email, UserRole userRole)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email && x.Role == userRole);

            if (user==null)
            {
                _logger.LogInformation($"User(email={email},role={userRole}) not found");
                return null;
            }

            _logger.LogInformation($"User by email={email} successfully found");
            return user;
        }
        public async Task<User?> GetById(int? id)
        {
            if (id==null)
            {
                _logger.LogInformation($"UserId can not be null");
                return null;
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user==null)
            {
                _logger.LogInformation($"User(id={id}) not found");
                return null;
            }

            _logger.LogInformation($"User(id={id}) successfully found");
            return user;
        }

        public async Task<int?> CreateAsync(User user, string? groupCode = null)
        {
            if (user==null)
            {
                _logger.LogInformation($"User for creation is not valid");
                throw new ArgumentNullException("User for creation is not valid");
            }

            user.Password = _encrypter.Encrypt(user.Password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            //creating teacher/student
            if (user.Role==UserRole.Student && groupCode!=null)
            {
                var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupCode==groupCode);

                if (group == null)
                {
                    _logger.LogWarning($"Group with code={groupCode} not found");
                    return null;
                }

                await _context.Students.AddAsync(new Student
                {
                    User=user,
                    Group = group
                });
            }
            else if (user.Role==UserRole.Teacher)
            {
                await _context.Teachers.AddAsync(new Teacher { User = user });
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Created user with ID {user.Id}");

            return user.Id;
        }

    }
}
