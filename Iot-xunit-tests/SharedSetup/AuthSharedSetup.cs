using AutoMapper;
using iot_management_api.Configuration;
using iot_management_api.Controllers;
using iot_management_api.Entities;
using iot_management_api.Entities.common;
using iot_management_api.Helper;
using iot_management_api.Models;
using iot_management_api.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;

namespace Iot_xunit_tests.SharedSetup
{
    public class AuthSharedSetup : IDisposable
    {
        private string name = "Name";
        private string surname = "Surname";
        private int userId = 1;
        private string userEmail = "edvin.brown@gmail.com";
        private UserRole userRole = UserRole.Teacher;
        private string groupCode = "TV-12";



        public Encrypter Encrypter;

        public JwtGenerator JwtGenerator { get; }
        public Mock<ILogger<AuthController>> AuthLogger { get; }
        public Mock<IMapper> Mapper;
        public Mock<IStudentService> StudentService;
        public Mock<ITeacherService> TeacherService;

        public Teacher Teacher { get; }
        public TeacherModel TeacherModel { get; }
        public Student Student { get; }
        public StudentModel StudentModel { get; }
        public string Name { get { return name; } }
        public string Surname { get { return surname; } }
        public int UserId { get { return userId; } }
        public string UserEmail { get { return userEmail; } }
        public UserRole UserRole { get { return userRole; } }
        public string GroupCode { get { return groupCode; } }
        public List<Claim> Claims { get; }

        public AuthSharedSetup()
        {
            //logger
            AuthLogger = new Mock<ILogger<AuthController>>();

            //encrypt
            var encryptionConfigMock = new Mock<IOptions<PasswordEncryption>>();
            encryptionConfigMock
                .Setup(config => config.Value)
                .Returns(new PasswordEncryption
                {
                    Key = "$2a$10$iQbE6XllxYBgNt/cktFC0u"
                });

            Encrypter = new Encrypter(encryptionConfigMock.Object);

            //claims
            Claims = new List<Claim>
            {
                new Claim("id", userId.ToString()),
                new Claim("email", userEmail.ToString()),
                new Claim("role", userRole.ToString())
            };

            //teacher and teacher data
            Teacher = CreateTeacherInstance();
            TeacherModel = CreateTeacherModelInstance();
            Student = CreateStudentInstance();
            StudentModel = CreateStudentModelInstance();

            //jwt config
            var jwtConfigMock = new Mock<IOptions<JwtConfig>>();
            jwtConfigMock.Setup(config => config.Value).Returns(new JwtConfig
            {
                Key = "FXPHmFpoXzGZ9NegXlFqoecQz9niMXMc",
                Issuer = "react-iot-management",
                Audience = "webapi-iot-management-api"
            });
            JwtGenerator = new JwtGenerator(jwtConfigMock.Object);

            // mapper mock
            Mapper = new Mock<IMapper>();
            Mapper.Setup(mapper => mapper.Map<TeacherModel>(It.IsAny<Teacher>()))
                .Returns(TeacherModel);

            //teacher/student service
            StudentService = new Mock<IStudentService>();
            TeacherService = new Mock<ITeacherService>();
            TeacherService.Setup(service => service.GetByIdAsync(UserId)).ReturnsAsync(Teacher);
        }

        private Teacher CreateTeacherInstance()
        {
            return new Teacher
            {
                Id = 1,
                Name = Name,
                Surname = Surname,
                Email = UserEmail,
                Password = Encrypter.Encrypt(UserEmail),
                CreatedAt = DateTime.Now
            };
        }
        private TeacherModel CreateTeacherModelInstance()
        {
            return new TeacherModel
            {
                Id = 1,
                Name = Name,
                Surname = Surname,
                Email = UserEmail
            };
        }
        private Student CreateStudentInstance()
        {
            return new Student
            {
                Id = 1,
                Name = Name,
                Surname = Surname,
                Email = UserEmail,
                Password = Encrypter.Encrypt(UserEmail),
                CreatedAt = DateTime.Now
            };
        }
        private StudentModel CreateStudentModelInstance()
        {
            return new StudentModel
            {
                Id = 1,
                Name = Name,
                Surname = Surname,
                Email = UserEmail,
                GroupCode = GroupCode
            };
        }

        public void Dispose() { }
    }
}
