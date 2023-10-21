using AutoMapper;
using iot_management_api.Entities;
using iot_management_api.Entities.common;
using iot_management_api.Helper;
using iot_management_api.Models;
using iot_management_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iot_management_api.Controllers
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly IStudentService _studentService;
        private readonly ILogger<AuthController> _logger;
        private readonly JwtGenerator _jwtGenerator;
        private readonly IMapper _mapper;

        public AuthController(ITeacherService teacherService,
            IStudentService studentService,
            ILogger<AuthController> logger,
            JwtGenerator jwtGenerator,
            IMapper mapper)
        {
            _teacherService=teacherService;
            _studentService=studentService;
            _logger=logger;
            _jwtGenerator=jwtGenerator;
            _mapper=mapper;
        }

        [HttpGet]
        [Authorize]
        [Route("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(HttpContext.User.Claims?.First(x => x.Type == "id").Value!);
            var userRole = Enum.Parse<UserRole>(HttpContext.User.Claims?.First(x => x.Type == "role").Value!);

            if (userRole==UserRole.Student)
            {
                var user = await _studentService.GetByIdAsync(userId);

                return Ok(_mapper.Map<StudentModel>(user));
            }
            if (userRole==UserRole.Teacher)
            {
                var user = await _teacherService.GetByIdAsync(userId);

                return Ok(_mapper.Map<TeacherModel>(user));
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("signup/student")]
        [ProducesResponseType(typeof(SignUpResponse), 200)]
        public async Task<IActionResult> SignUpStudent([FromBody] StudentSignUpRequest request)
        {
            var user = await _studentService.GetByEmailAsync(request.User.Email);
            if (user!=null)
                return BadRequest("Email already used");

            var createdId = await _studentService.CreateAsync(_mapper.Map<Student>(request.User), request.GroupCode);

            if (createdId == null)
            {
                _logger.LogError($"User was not created");
                return BadRequest("Unknown Error. User was not created");
            }
            //response
            return Ok(new SignUpResponse
            {
                CreatedId = createdId
            });
        }

        [HttpPost]
        [Route("signup/teacher")]
        [ProducesResponseType(typeof(SignUpResponse), 200)]
        public async Task<IActionResult> SignUpTeacher([FromBody] TeacherSignUpRequest request)
        {
            var user = await _teacherService.GetByEmailAsync(request.User.Email);
            if (user!=null)
                return BadRequest("Email already used");

            var createdId = await _teacherService.CreateAsync(_mapper.Map<Teacher>(request.User));

            if (createdId == null)
            {
                _logger.LogError($"User was not created");
                return BadRequest("Unknown Error. User was not created");
            }

            return Ok(new SignUpResponse
            {
                CreatedId = createdId
            });
        }

        [HttpPost]
        [Route("signin/student")]
        [ProducesResponseType(typeof(StudentModel), 200)]
        public async Task<IActionResult> SignInStudent([FromBody] SignInRequest request)
        {
            //user = await _teacherService.GetByEmail(request.User.Email);
            User? user = await _studentService.GetByEmailAsync(request.User.Email);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            //password check
            if (!BCrypt.Net.BCrypt.Verify(request.User.Password, user.Password))
            {
                _logger.LogWarning($"User(Id={user.Id}) password is not correct");
                return BadRequest("Password not correct.");
            }
            _logger.LogInformation($"User(Id={user.Id}) password is correct");

            //token gen
            var token = _jwtGenerator.GenerateToken(user.Id, user.Email, UserRole.Student);
            //HttpContext.Response.Cookies.Append("token", token);

            //response
            return Ok(new
            {
                user = _mapper.Map<StudentModel>(user),
                token
            });
        }

        [HttpPost]
        [Route("signin/teacher")]
        [ProducesResponseType(typeof(TeacherModel), 200)]
        public async Task<IActionResult> SignInTeacher([FromBody] SignInRequest request)
        {
            User? user = await _teacherService.GetByEmailAsync(request.User.Email);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            //password check
            if (!BCrypt.Net.BCrypt.Verify(request.User.Password, user.Password))
            {
                _logger.LogWarning($"User(Id={user.Id}) password is not correct");
                return BadRequest("Password not correct.");
            }
            _logger.LogInformation($"User(Id={user.Id}) password is correct");

            //token gen
            var token = _jwtGenerator.GenerateToken(user.Id, user.Email, UserRole.Teacher);
            //HttpContext.Response.Cookies.Append("token", token);

            //response
            return Ok(new
            {
                user = _mapper.Map<TeacherModel>(user),
                token = token
            });

        }

        [HttpGet]
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            //HttpContext.Response.Cookies.Delete("token");
            return Ok();
        }

        public class UserSignUp
        {
            public required string Name { get; set; }
            public required string Surname { get; set; }
            public required string Email { get; set; }
            public required string Password { get; set; }
        }
        public class StudentSignUpRequest
        {
            public required UserSignUp User { get; set; }
            public string? GroupCode { get; set; }
        }
        public class TeacherSignUpRequest
        {
            public required UserSignUp User { get; set; }
        }
        public class SignUpResponse
        {
            public int? CreatedId { get; set; }
        }

        public class SignInRequest
        {
            public required SignInUser User { get; set; }
            public class SignInUser
            {
                public required string Email { get; set; }
                public required string Password { get; set; }
            }
        }
    }
}
