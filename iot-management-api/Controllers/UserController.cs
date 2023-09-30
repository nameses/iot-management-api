using iot_management_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iot_management_api.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger=logger;
        }
        [HttpGet]
        [Authorize]
        [Route("get/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetById(id);

            if (user==null)
            {
                _logger.LogInformation($"User with id={id} not found");
                return NotFound();
            }

            return Ok(user);
        }
    }
}
