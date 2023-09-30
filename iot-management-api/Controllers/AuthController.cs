using AutoMapper;
using iot_management_api.Entities;
using iot_management_api.Entities.common;
using iot_management_api.Helper;
using iot_management_api.Models;
using iot_management_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace iot_management_api.Controllers
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;
        private readonly JwtGenerator _jwtGenerator;
        private readonly IMapper _mapper;

        public AuthController(IUserService userService,
            ILogger<AuthController> logger,
            JwtGenerator jwtGenerator,
            IMapper mapper)
        {
            _userService = userService;
            _logger=logger;
            _jwtGenerator=jwtGenerator;
            _mapper=mapper;
        }

        public class RegRequest
        {
            public required RegUser User { get; set; }
            public string? GroupCode { get; set; }
            public class RegUser
            {
                public required string Name { get; set; }
                public required string Surname { get; set; }
                public required string Email { get; set; }
                public required string Password { get; set; }
                public required string Role { get; set; }
            }
        }
        public class RegResponse
        {
            public int? UserId { get; set; }
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(typeof(RegResponse), 200)]
        public async Task<IActionResult> Register([FromBody] RegRequest request)
        {
            var createdId = await _userService.CreateAsync(_mapper.Map<User>(request.User), request.GroupCode);

            if (createdId==null)
            {
                _logger.LogError($"User was not created");
                return BadRequest("Unknown Error. User was not created");
            }

            return Ok(new RegResponse
            {
                UserId = createdId
            });
        }

        /// <summary>
        ///     Login method, on Output gives Auth Token in Body and in Cookies("auth_token")
        /// </summary>
        /// <param name="user">User Entity</param>
        /// <returns>User Model, Auth Token in Body and in Cookies("auth_token")</returns>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!Enum.TryParse<UserRole>(request.User.Role, out var userRole))
                return BadRequest("User role not exists");

            var dbUser = await _userService.GetByEmail(request.User.Email, userRole);

            if (dbUser == null)
            {
                return BadRequest("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.User.Password, dbUser.Password))
            {
                _logger.LogWarning($"User(Id={dbUser.Id}) password is not correct");
                return BadRequest("Password not correct.");
            }
            _logger.LogInformation($"User(Id={dbUser.Id}) password is correct");

            var token = _jwtGenerator.GenerateToken(dbUser.Id);
            HttpContext.Response.Cookies.Append("auth_token", token);

            return Ok(new LoginResponse
            {
                Token = token,
                User = _mapper.Map<UserModel>(dbUser)
                //User = new UserModel
                //{
                //    Id = dbUser.Id,
                //    Name = dbUser.Name,
                //    Surname = dbUser.Surname,
                //    Email = dbUser.Email,
                //    Role = dbUser.Role.ToString(),
                //}
            });
        }
        public class LoginRequest
        {
            public required LoginUser User { get; set; }
            public class LoginUser
            {
                public required string Email { get; set; }
                public required string Password { get; set; }
                public required string Role { get; set; }
            }

        }

        public class LoginResponse
        {
            public required string Token { get; set; }
            public required UserModel User { get; set; }
        }
    }
}
