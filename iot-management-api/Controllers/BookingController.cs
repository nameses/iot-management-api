using AutoMapper;
using iot_management_api.Entities.common;
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
        private readonly IScheduleService _scheduleService;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingController> _logger;

        public record struct ApproveDeviceReq(int bookingId);

        public BookingController(IBookingService bookingService,
            IDeviceService deviceService,
            IScheduleService scheduleService,
            IMapper mapper,
            ILogger<BookingController> logger)
        {
            _bookingService=bookingService;
            _deviceService=deviceService;
            _scheduleService=scheduleService;
            _mapper=mapper;
            _logger=logger;
        }

        [HttpPost]
        [Route("approve")]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> ApproveDevice([FromBody] ApproveDeviceReq req)
        {
            var userId = int.Parse(HttpContext.User.Claims?.First(x => x.Type == "id").Value!);

            var resStatusMessage = await _bookingService.ApproveBooking(userId, req.bookingId);

            if (!resStatusMessage.res)
            {
                if (resStatusMessage.message=="Not Found")
                    return NotFound();
                return BadRequest(resStatusMessage.Item2);
            }

            return Ok();
        }

        [HttpPost]
        [Route("device/{deviceId}")]
        [Authorize(Policy = "StudentAccess")]
        public async Task<IActionResult> BookDevice([FromRoute] int deviceId, [FromQuery] DateOnly date, int scheduleId)
        {
            var userRole = Enum.Parse<UserRole>(HttpContext.User.Claims?.First(x => x.Type == "role").Value!);
            var userId = int.Parse(HttpContext.User.Claims?.First(x => x.Type == "id").Value!);
            //check if date expired
            if (date < DateOnly.FromDateTime(DateTime.Now))
                return BadRequest("Date is expired");

            //check if this date is the date of schedule
            var ifDateIsScheduleDate = await _scheduleService.CheckDateSchedule(date, scheduleId);
            if (!ifDateIsScheduleDate)
                return BadRequest("Date/Schedule mismatch");

            //check if User is assigned to this schedule
            var ifUserAssignedToSchedule = await _scheduleService.CheckUserAssignmentToSchedule(userRole, userId, scheduleId);
            if (!ifUserAssignedToSchedule)
                return BadRequest("Current user is not assigned to this schedule");

            //check if device available for date/schedule
            var ifDeviceAvailable = await _deviceService.CheckIfDeviceAvailableAsync(deviceId, date, scheduleId);
            if (!ifDeviceAvailable)
                return BadRequest("Device is not available");



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
