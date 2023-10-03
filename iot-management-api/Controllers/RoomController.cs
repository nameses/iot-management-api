using AutoMapper;
using iot_management_api.Entities;
using iot_management_api.Models;
using iot_management_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize(Policy = "TeacherAccess")]
        [Route("get/{id}")]
        [ProducesResponseType(typeof(RoomModel), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _roomService.GetById(id);

            if (entity==null)
                return NotFound();

            return Ok(_mapper.Map<RoomModel>(entity));
        }

        [HttpGet]
        [Authorize(Policy = "TeacherAccess")]
        [Route("getByNumber/{number}")]
        [ProducesResponseType(typeof(GroupModel), 200)]
        public async Task<IActionResult> GetByGroupCode(int number)
        {
            var entity = await _roomService.GetByNumber(number);

            if (entity==null)
                return NotFound();

            return Ok(_mapper.Map<RoomModel>(entity));
        }

        [HttpPost]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> Create([FromBody] RoomReq req)
        {
            if ((await _roomService.GetByNumber(req.Number))!=null)
                return BadRequest("Room already exists");

            var entityId = await _roomService.CreateAsync(_mapper.Map<Room>(req));

            if (entityId==null)
            {
                _logger.LogInformation($"Room with room number {req.Number} was not created");
                return NotFound();
            }

            return Ok(new
            {
                CreatedId = entityId
            });
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> Update(int id, [FromBody] RoomReq room)
        {
            var res = await _roomService.UpdateAsync(id, _mapper.Map<Room>(room));

            if (!res) return NotFound();

            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _roomService.DeleteAsync(id);

            if (!res) return NotFound();

            return Ok();
        }

        public class RoomReq
        {
            public required int Number { get; set; }
            public required int Floor { get; set; }
            public string? Lable { get; set; }
        }
    }
}
