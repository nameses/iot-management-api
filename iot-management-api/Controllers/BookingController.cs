using AutoMapper;
using iot_management_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iot_management_api.Controllers
{
    [ApiController]
    [Route("api/booking")]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IBookingService bookingService,
            IMapper mapper,
            ILogger<BookingController> logger)
        {
            _bookingService=bookingService;
            _mapper=mapper;
            _logger=logger;
        }

        [HttpPost]
        [Route("book/device/{deviceId}")]
        [Authorize(Policy = "StudentAccess")]
        public async Task<IActionResult> BookDevice([FromRoute] int deviceId, [FromQuery] DateOnly date, int scheduleId)
        {
            var userId = int.Parse(HttpContext.User.Claims?.First(x => x.Type == "id").Value!);

            var res = await _bookingService.BookDeviceAsync(deviceId, userId, date, scheduleId);

            if (res==false)
                return BadRequest();

            return Ok();
        }
    }
}
