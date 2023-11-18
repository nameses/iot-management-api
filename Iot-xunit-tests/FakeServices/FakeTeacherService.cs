using iot_management_api.Configuration;
using iot_management_api.Entities;
using iot_management_api.Entities.common;
using iot_management_api.Helper;
using iot_management_api.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace Iot_xunit_tests.FakeServices
{
    public class FakeTeacherService : ITeacherService
    {
        private string name = "Name";
        private string surname = "Surname";
        private int userId = 1;
        private string userEmail = "edvin.brown@gmail.com";
        private UserRole userRole = UserRole.Teacher;
        private string groupCode = "TV-12";

        public FakeTeacherService() { }

        public async Task<int?> CreateAsync(Teacher teacher)
        {
            return 1;
        }

        public async Task<Teacher?> GetByEmailAsync(string email)
        {
            var encryptionConfigMock = new Mock<IOptions<PasswordEncryption>>();
            encryptionConfigMock
                .Setup(config => config.Value)
                .Returns(new PasswordEncryption
                {
                    Key = "$2a$10$iQbE6XllxYBgNt/cktFC0u"
                });

            var encrypter = new Encrypter(encryptionConfigMock.Object);

            return new Teacher()
            {
                Id = userId,
                Name = name,
                Surname = surname,
                Email = userEmail,
                Password = encrypter.Encrypt(userEmail),
                CreatedAt = DateTime.Now
            };
        }

        public async Task<Teacher?> GetByIdAsync(int? id)
        {
            var encryptionConfigMock = new Mock<IOptions<PasswordEncryption>>();
            encryptionConfigMock
                .Setup(config => config.Value)
                .Returns(new PasswordEncryption
                {
                    Key = "$2a$10$iQbE6XllxYBgNt/cktFC0u"
                });

            var encrypter = new Encrypter(encryptionConfigMock.Object);

            return new Teacher()
            {
                Id = userId,
                Name = name,
                Surname = surname,
                Email = userEmail,
                Password = encrypter.Encrypt(userEmail),
                CreatedAt = DateTime.Now
            };
        }
    }
}
