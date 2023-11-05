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
        private readonly IScheduleService _scheduleService;
        private readonly IMapper _mapper;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(IDeviceService deviceService,
            IScheduleService scheduleService,
            IMapper mapper,
            ILogger<DeviceController> logger)
        {
            _deviceService=deviceService;
            _scheduleService=scheduleService;
            _mapper=mapper;
            _logger=logger;
        }
        /// <summary>
        /// Show list of available devices
        /// </summary>
        /// <returns>List of devices</returns>
        /// <response code="200">Request Successful</response>
        /// <response code="400">Date is expired, Date/Schedule mismatch</response>
        /// <response code="401">Unathorized</response>
        [HttpGet]
        [Authorize]
        [Route("available")]
        [ProducesResponseType(typeof(IEnumerable<DeviceModel>), 200)]
        public async Task<IActionResult> GetAvailable(DateOnly date, int scheduleId)
        {
            //check if date expired
            if (date < DateOnly.FromDateTime(DateTime.Now))
                return BadRequest("Date is expired");

            //check if this date is the date of schedule
            var ifDateIsScheduleDate = await _scheduleService.CheckDateSchedule(date, scheduleId);
            if (!ifDateIsScheduleDate)
                return BadRequest("Date/Schedule mismatch");

            var entity = await _deviceService.GetAvailableAsync(date, scheduleId);

            if (entity==null)
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<DeviceModel>>(entity));
        }

        /// <summary>
        /// Get device by id
        /// </summary>
        /// <returns>Device</returns>
        /// <response code="200">Request Successful</response>
        /// <response code="404">Not found</response>
        /// <response code="401">Unathorized</response>
        /// <response code="403">Forbidden</response>
        [HttpGet]
        [Authorize(Policy = "TeacherAccess")]
        [Route("{id}")]
        [ProducesResponseType(typeof(DeviceModel), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _deviceService.GetByIdAsync(id);

            if (entity==null)
                return NotFound();

            return Ok(_mapper.Map<DeviceModel>(entity));
        }

        /// <summary>
        /// Get devices by room number
        /// </summary>
        /// <returns>List of devices</returns>
        /// <response code="200">Request Successful</response>
        /// <response code="401">Unathorized</response>
        /// <response code="403">Forbidden</response>
        [HttpGet]
        [Authorize(Policy = "TeacherAccess")]
        [ProducesResponseType(typeof(List<DeviceModel>), 200)]
        public async Task<IActionResult> GetByRoom([FromQuery] int room)
        {
            var entities = await _deviceService.GetByRoomAsync(room);

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
        public async Task<IActionResult> Update(int id, [FromBody] DeviceUpdateReq req)
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
            public required string Type { get; set; }
            public required string Name { get; set; }
            public required string Model { get; set; }
            public required string Description { get; set; }
            public required int Amount { get; set; }
            public required int RoomNumber { get; set; }

        }
        public class DeviceUpdateReq : DeviceReq
        {
            public required int Id { get; set; }
        }
    }
}
