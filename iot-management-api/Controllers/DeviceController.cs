using AutoMapper;
using iot_management_api.Entities;
using iot_management_api.Models;
using iot_management_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iot_management_api.Controllers
{
    [ApiController]
    [Route("api/device")]
    public class DeviceController : Controller
    {
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(IDeviceService deviceService,
            IMapper mapper,
            ILogger<DeviceController> logger)
        {
            _deviceService=deviceService;
            _mapper=mapper;
            _logger=logger;
        }

        [HttpGet]
        [Authorize(Policy = "TeacherAccess")]
        [Route("{id}")]
        [ProducesResponseType(typeof(DeviceModel), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _deviceService.GetById(id);

            if (entity==null)
                return NotFound();

            return Ok(_mapper.Map<DeviceModel>(entity));
        }

        [HttpGet]
        [Authorize(Policy = "TeacherAccess")]
        [ProducesResponseType(typeof(List<DeviceModel>), 200)]
        public async Task<IActionResult> GetByRoom([FromQuery] int room)
        {
            var entities = await _deviceService.GetByRoom(room);

            if (entities==null)
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<DeviceModel>>(entities));
        }

        [HttpPost]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> Create([FromBody] DeviceReq req)
        {
            //if ((await _roomService.GetByNumber(req.Number))!=null)
            //    return BadRequest("Room already exists");

            var entityId = await _deviceService.CreateAsync(
                new Device() { Amount=req.Amount },
                _mapper.Map<DeviceInfo>(req),
                req.RoomNumber
            );

            if (entityId==null)
                return BadRequest();

            return Ok(new
            {
                CreatedId = entityId
            });
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> Update(int id, [FromBody] DeviceReq req)
        {
            if (id!=req.Id)
                return BadRequest();

            var res = await _deviceService.UpdateAsync(id, _mapper.Map<Device>(req));

            if (!res) return NotFound();

            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _deviceService.DeleteAsync(id);

            if (!res) return NotFound();

            return Ok();
        }

        public class DeviceReq
        {
            public required int Id { get; set; }
            public required string Type { get; set; }
            public required string Name { get; set; }
            public required string Model { get; set; }
            public required string Description { get; set; }
            public required int Amount { get; set; }
            public required int RoomNumber { get; set; }

        }
    }
}
