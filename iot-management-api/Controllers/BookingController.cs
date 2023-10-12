using AutoMapper;
using iot_management_api.Models;
using iot_management_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace iot_management_api.Controllers
{
    [ApiController]
    [Route("api/booking")]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IBookingService bookingService,
            IDeviceService deviceService,
            IMapper mapper,
            ILogger<BookingController> logger)
        {
            _bookingService=bookingService;
            _deviceService=deviceService;
            _mapper=mapper;
            _logger=logger;
        }

        [HttpPost]
        [Route("book/device/{deviceId}")]
        [Authorize(Policy = "StudentAccess")]
        public async Task<IActionResult> BookDevice([FromRoute] int deviceId, [FromQuery] DateOnly date, int scheduleId)
        {
            if (date<DateOnly.FromDateTime(DateTime.Now))
                return BadRequest("Date is expired");

            var ifDeviceAvailable = await _deviceService.CheckIfDeviceAvailableAsync(deviceId, date, scheduleId);
            if (!ifDeviceAvailable)
                return BadRequest("Device is not available");

            var userId = int.Parse(HttpContext.User.Claims?.First(x => x.Type == "id").Value!);

            await _bookingService.BookDeviceAsync(deviceId, userId, date, scheduleId);
            return Ok();
        }

        [HttpGet]
        [Route("requests")]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> ShowStudentsRequests()
        {
            var userId = int.Parse(HttpContext.User.Claims?.First(x => x.Type == "id").Value!);

            var entities = await _bookingService.ShowStudentsRequestsForTeacherAsync(userId);

            if (entities.IsNullOrEmpty())
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<BookingModel>>(entities));
        }
    }
}
