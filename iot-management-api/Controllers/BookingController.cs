using AutoMapper;
using iot_management_api.Entities;
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

        //public record struct ApproveRejectDeviceReq(int bookingId);

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

        /// <summary>
        /// Get bookings for some date and schedule id
        /// </summary>
        /// <returns>List of bookings</returns>
        /// <param name="date">Date of bookings</param>
        /// <param name="scheduleId">Id of schedule(lesson)</param>
        /// <response code="200">Request Successful</response>
        /// <response code="400">Date is expired / Date-Schedule mismatch / Current user is not assigned to this schedule</response>
        /// <response code="401">Unathorized</response>
        [HttpGet]
        [Route("full")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<BookingForStudentModel>), 200)]
        public async Task<IActionResult> GetBookings([FromQuery] DateOnly date, int scheduleId)
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

            if (userRole == UserRole.Student)
            {
                var bookings = await _bookingService.GetStudentBookings(date, scheduleId, userId);

                if (bookings.IsNullOrEmpty())
                    return Ok(Array.Empty<string>());

                return Ok(_mapper.Map<IEnumerable<BookingForStudentModel>>(bookings));

            }
            else if (userRole == UserRole.Teacher)
            {
                var bookings = await _bookingService.GetBookingsForTeacher(date, scheduleId, userId);

                if (bookings.IsNullOrEmpty())
                    return Ok(Array.Empty<string>());

                return Ok(_mapper.Map<IEnumerable<BookingModel>>(bookings));
            }

            return Ok(Array.Empty<string>());
        }

        /// <summary>
        /// Get bookings for some date and schedule id
        /// </summary>
        /// <returns>List of bookings</returns>
        /// <param name="dateFrom">Date, from which get bookings(must not be in past)</param>
        /// <param name="dateTo">Date limiter</param>
        /// <response code="200">Request Successful</response>
        /// <response code="400">Wrong period(dateFrom-dateTo problems)</response>
        /// <response code="401">Unathorized</response>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBookingsFromPeriod([FromQuery] DateOnly? dateFrom, DateOnly? dateTo)
        {
            var userRole = Enum.Parse<UserRole>(HttpContext.User.Claims?.First(x => x.Type == "role").Value!);
            var userId = int.Parse(HttpContext.User.Claims?.First(x => x.Type == "id").Value!);
            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            //check dates
            if (dateFrom==null || dateFrom<currentDate)
                dateFrom=currentDate;

            if (dateTo < currentDate || dateTo < dateFrom)
                return BadRequest("Wrong period(dateFrom-dateTo problems)");

            if (userRole == UserRole.Student)
            {
                var entities = await _bookingService.GetStudentBookings(userId, dateFrom, dateTo);

                if (entities.IsNullOrEmpty())
                    return Ok(Array.Empty<string>());

                return Ok(_mapper.Map<IEnumerable<BookingForStudentModel>>(entities));
            }
            else if (userRole == UserRole.Teacher)
            {
                var entities = await _bookingService.GetBookingsForTeacher(userId, dateFrom, dateTo);

                if (entities.IsNullOrEmpty())
                    return Ok(Array.Empty<string>());

                return Ok(_mapper.Map<IEnumerable<BookingModel>>(entities));
            }

            return Ok(Array.Empty<string>());
        }

        /// <summary>
        /// Approve device booking (teacher access)
        /// </summary>
        /// <returns>List of bookings</returns>
        /// <param name="bookingId">Booking id</param>
        /// <response code="200">Request Successful</response>
        /// <response code="400">Date is expired / Teacher does not have access to this group / Booking request already approved / There are no available devices for this schedule</response>
        /// <response code="401">Unathorized</response>
        /// <response code="403">Forbidden</response>
        [HttpPost]
        [Route("approve/{bookingId}")]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> ApproveDevice([FromRoute] int bookingId)//, [FromBody] ApproveRejectDeviceReq req)
        {
            var userId = int.Parse(HttpContext.User.Claims?.First(x => x.Type == "id").Value!);

            var resStatusMessage = await _bookingService.ApproveBooking(userId, bookingId); //req.bookingId);

            if (!resStatusMessage.res)
            {
                if (resStatusMessage.message=="Not Found")
                    return NotFound();
                return BadRequest(resStatusMessage.message);
            }

            return Ok();
        }

        /// <summary>
        /// Reject device booking (teacher access)
        /// </summary>
        /// <returns>List of bookings</returns>
        /// <param name="bookingId">Booking id</param>
        /// <response code="200">Request Successful</response>
        /// <response code="400">Date is expired / Teacher does not have access to this group / Booking request already approved / There are no available devices for this schedule</response>
        /// <response code="401">Unathorized</response>
        /// <response code="403">Forbidden</response>
        [HttpPost]
        [Route("reject/{bookingId}")]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> RejectDevice([FromRoute] int bookingId)//, [FromBody] ApproveRejectDeviceReq req)
        {
            var userId = int.Parse(HttpContext.User.Claims?.First(x => x.Type == "id").Value!);

            var resStatusMessage = await _bookingService.RejectBooking(userId, bookingId); //req.bookingId);

            if (!resStatusMessage.res)
            {
                if (resStatusMessage.message=="Not Found")
                    return NotFound();
                return BadRequest(resStatusMessage.message);
            }

            return Ok();
        }

        /// <summary>
        /// Booking of device for date and schedule(lesson) (student access)
        /// </summary>
        /// <returns>List of bookings</returns>
        /// <param name="deviceId">Device id</param>
        /// <param name="date">Date of booking</param>
        /// <param name="scheduleId">Schedule id</param>
        /// <response code="200">Request Successful</response>
        /// <response code="400">Date is expired / Teacher does not have access to this group / Booking request already approved / Device is not available</response>
        /// <response code="401">Unathorized</response>
        /// <response code="403">Forbidden</response>
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
        /// <summary>
        /// Show bookings with pending status
        /// </summary>
        /// <returns>List of bookings</returns>
        /// <response code="200">Request Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unathorized</response>
        /// <response code="403">Forbidden</response>
        [HttpGet]
        [Route("requests")]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> ShowStudentsRequests()
        {
            var userId = int.Parse(HttpContext.User.Claims?.First(x => x.Type == "id").Value!);

            var entities = await _bookingService.ShowStudentsRequestsForTeacherAsync(userId);

            if (entities.IsNullOrEmpty())
                return Ok(Array.Empty<string>());

            return Ok(_mapper.Map<IEnumerable<BookingModel>>(entities));
        }

        public class BookingForStudentModel
        {
            public int Id { get; set; }
            public DateOnly Date { get; set; }
            public BookingStatus Status { get; set; }
            public DeviceModel? Device { get; set; }
        }

    }
}
