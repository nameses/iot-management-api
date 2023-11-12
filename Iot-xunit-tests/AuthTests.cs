using iot_management_api.Controllers;
using iot_management_api.Entities;
using iot_management_api.Models;
using Iot_xunit_tests.SharedSetup;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using static iot_management_api.Controllers.AuthController;

namespace Iot_xunit_tests
{
    public class AuthTests : IClassFixture<AuthSharedSetup>
    {
        private readonly AuthSharedSetup _sharedSetup;

        public AuthTests(AuthSharedSetup fixture)
        {
            _sharedSetup = fixture;
        }

        [Fact]
        public async Task GetProfile_WithValidTeacherRole_ShouldReturnTeacherModel()
        {
            //Arrange
            var controller = new AuthController(
                _sharedSetup.TeacherService.Object,
                _sharedSetup.StudentService.Object,
                _sharedSetup.AuthLogger.Object,
                _sharedSetup.JwtGenerator,
                _sharedSetup.Mapper.Object
            );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(_sharedSetup.Claims, "TestAuthentication"))
                }
            };

            // Act
            var result = await controller.GetProfile();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<TeacherModel>(okResult.Value);
            Assert.Equal(okResult.Value, _sharedSetup.TeacherModel);
        }

        [Fact]
        public async Task SignUpStudent_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var studentSignUpRequest = new StudentSignUpRequest
            {
                User = new UserSignUp
                {
                    Name = _sharedSetup.Name,
                    Surname = _sharedSetup.Surname,
                    Email = _sharedSetup.UserEmail,
                    Password = _sharedSetup.UserEmail,
                },
                GroupCode = _sharedSetup.GroupCode,
            };
            var mockedStudentService = _sharedSetup.StudentService;
            mockedStudentService.Setup(service => service.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Student)null);
            mockedStudentService.Setup(service => service.CreateAsync(It.IsAny<Student>(), It.IsAny<string>())).ReturnsAsync(1);

            var controller = new AuthController(
                _sharedSetup.TeacherService.Object,
                mockedStudentService.Object,
                _sharedSetup.AuthLogger.Object,
                _sharedSetup.JwtGenerator,
                _sharedSetup.Mapper.Object
            );


            // Act
            var result = await controller.SignUpStudent(studentSignUpRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var signUpResponse = Assert.IsType<SignUpResponse>(okResult.Value);
            Assert.Equal(1, signUpResponse.CreatedId);
        }

        [Fact]
        public async Task SignUpTeacher_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var teacherSignUpRequest = new TeacherSignUpRequest
            {
                User = new UserSignUp
                {
                    Name = _sharedSetup.Name,
                    Surname = _sharedSetup.Surname,
                    Email = _sharedSetup.UserEmail,
                    Password = _sharedSetup.UserEmail,
                }
            };

            var mockedTeacherService = _sharedSetup.TeacherService;
            mockedTeacherService.Setup(service => service.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Teacher)null);
            mockedTeacherService.Setup(service => service.CreateAsync(It.IsAny<Teacher>())).ReturnsAsync(2);

            var controller = new AuthController(
                _sharedSetup.TeacherService.Object,
                _sharedSetup.StudentService.Object,
                _sharedSetup.AuthLogger.Object,
                _sharedSetup.JwtGenerator,
                _sharedSetup.Mapper.Object
            );

            // Act
            var result = await controller.SignUpTeacher(teacherSignUpRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var signUpResponse = Assert.IsType<SignUpResponse>(okResult.Value);
            Assert.Equal(2, signUpResponse.CreatedId);
        }

        [Fact]
        public async Task SignInStudent_WithUnvalidCredentials_ShouldReturnBadRequest()
        {
            // Arrange
            var signInRequest = new SignInRequest
            {
                User = new SignInRequest.SignInUser
                {
                    Email = _sharedSetup.UserEmail,
                    Password = "unvalid password"
                }
            };
            var student = _sharedSetup.Student;

            var mockedStudentService = _sharedSetup.StudentService;
            mockedStudentService.Setup(service => service.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(student);

            var controller = new AuthController(
                _sharedSetup.TeacherService.Object,
                mockedStudentService.Object,
                _sharedSetup.AuthLogger.Object,
                _sharedSetup.JwtGenerator,
                _sharedSetup.Mapper.Object
            );

            // Act
            var result = await controller.SignInStudent(signInRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        //[Fact]
        //public async Task SignInTeacher_WithValidCredentials_ShouldReturnOk()
        //{
        //    // Arrange
        //    var signInRequest = new SignInRequest
        //    {
        //        User = new SignInRequest.SignInUser
        //        {
        //            Email = "jane.smith@example.com",
        //            Password = "password456"
        //        }
        //    };

        //var controller = new AuthController(
        //    _sharedSetup.TeacherService.Object,
        //    _sharedSetup.StudentService.Object,
        //    _sharedSetup.AuthLogger.Object,
        //    _sharedSetup.JwtGenerator,
        //    _sharedSetup.Mapper.Object
        //);

        //    var teacher = new Teacher
        //    {
        //        Id = 2,
        //        Name = "Jane",
        //        Surname = "Smith",
        //        Email = "jane.smith@example.com",
        //        Password = _mockEncrypter.Encrypt("jane.smith@example.com"), // Mocking encryption
        //        CreatedAt = DateTime.Now
        //    };

        //    _mockTeacherService.Setup(service => service.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(teacher);
        //    _mockEncrypter.Setup(encrypter => encrypter.Encrypt(It.IsAny<string>())).Returns("hashed_password");

        //    // Act
        //    var result = await controller.SignInTeacher(signInRequest);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var signInResponse = Assert.IsType<object>(okResult.Value); // Adjust the type based on your actual response structure
        //                                                                // Add assertions based on your actual response structure
        //}
    }
}