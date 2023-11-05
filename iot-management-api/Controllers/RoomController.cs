using AutoMapper;
using iot_management_api.Models;
using iot_management_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace iot_management_api.Controllers
{
    [ApiController]
    [Route("api/room")]
    public class RoomController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomController> _logger;

        public RoomController(IRoomService roomService, IMapper mapper, ILogger<RoomController> logger)
        {
            _roomService = roomService;
            _mapper=mapper;
            _logger=logger;
        }

        [HttpGet]
        [Route("all")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<RoomModel>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var entities = await _roomService.GetAllAsync();

            if (entities.IsNullOrEmpty())
                return Ok(Array.Empty<string>());

            return Ok(_mapper.Map<IEnumerable<RoomModel>>(entities));
        }
    }
}
