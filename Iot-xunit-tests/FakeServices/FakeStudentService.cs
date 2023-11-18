using iot_management_api.Configuration;
using iot_management_api.Entities;
using iot_management_api.Entities.common;
using iot_management_api.Helper;
using iot_management_api.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace Iot_xunit_tests.FakeServices
{
    public class FakeStudentService : IStudentService
    {
        private string name = "Name";
        private string surname = "Surname";
        private int userId = 1;
        private string userEmail = "edvin.brown@gmail.com";
        private UserRole userRole = UserRole.Teacher;
        private string groupCode = "TV-12";

        public FakeStudentService()//AppDbContext context, IGroupService groupService, ILogger<StudentService> logger, Encrypter encrypter) : base(context, groupService, logger, encrypter)
        {
        }

        public async Task<Student?> GetByEmailAsync(string email)
        {
            var encryptionConfigMock = new Mock<IOptions<PasswordEncryption>>();
            encryptionConfigMock
                .Setup(config => config.Value)
                .Returns(new PasswordEncryption
                {
                    Key = "$2a$10$iQbE6XllxYBgNt/cktFC0u"
                });

            var encrypter = new Encrypter(encryptionConfigMock.Object);

            return new Student
            {
                Id = userId,
                Name = name,
                Surname = surname,
                Email = userEmail,
                Password = encrypter.Encrypt(userEmail),
                CreatedAt = DateTime.Now
            };
        }
        public async Task<Student?> GetByIdAsync(int? id)
        {
            var encryptionConfigMock = new Mock<IOptions<PasswordEncryption>>();
            encryptionConfigMock
                .Setup(config => config.Value)
                .Returns(new PasswordEncryption
                {
                    Key = "$2a$10$iQbE6XllxYBgNt/cktFC0u"
                });

            var encrypter = new Encrypter(encryptionConfigMock.Object);

            return new Student
            {
                Id = userId,
                Name = name,
                Surname = surname,
                Email = userEmail,
                Password = encrypter.Encrypt(userEmail),
                CreatedAt = DateTime.Now
            };
        }

        public async Task<int?> CreateAsync(Student student, string? groupCode)
        {
            return userId;
        }
    }
}
