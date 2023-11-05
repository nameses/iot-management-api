using AutoMapper;
using iot_management_api.Entities.common;
using iot_management_api.Helper;
using iot_management_api.Models;
using iot_management_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iot_management_api.Controllers
{
    [ApiController]
    [Route("api/schedule")]
    public class ScheduleController : Controller
    {
        private readonly IScheduleService _scheduleService;
        private readonly StudyWeekService _weekService;
        private readonly IMapper _mapper;
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(
            IScheduleService scheduleService,
            StudyWeekService weekService,
            IMapper mapper,
            ILogger<ScheduleController> logger)
        {
            _scheduleService = scheduleService;
            _weekService=weekService;
            _mapper=mapper;
            _logger=logger;
        }

        /// <summary>
        /// Show a full 2-week schedule and current week(1 or 2)
        /// </summary>
        /// <returns>Dictionary with key of WeekEnum(0-1), each contains a dictionary with key DayEnum(0-6) and value List of ScheduleModel</returns>
        /// <response code="200">Request Successful. Returns dictionary with key of WeekEnum(0-1), each contains a dictionary with key DayEnum(0-6) and value List of ScheduleModel</response>
        /// <response code="404">Not found</response>
        /// <response code="401">Unathorized</response>
        [HttpGet]
        [Authorize]
        [Route("full")]
        [ProducesResponseType(typeof(ScheduleFullResponse), 200)]
        public async Task<IActionResult> GetFull([FromQuery] DateOnly? date)
        {
            var userId = int.Parse(HttpContext.User.Claims?.First(x => x.Type == "id").Value!);
            var userRole = Enum.Parse<UserRole>(HttpContext.User.Claims?.First(x => x.Type == "role").Value!);

            if (date == null) date = DateOnly.FromDateTime(DateTime.Now);

            var scheduleDict = await _scheduleService.GetFullAsync(userRole, userId, date.Value);

            return Ok(new ScheduleFullResponse
            {
                Schedule = scheduleDict!,
                CurrentWeek = _weekService.GetCurrentWeek(),
            });
        }

        public class ScheduleFullResponse
        {
            public required Dictionary<WeekEnum, Dictionary<DateOnly, List<ScheduleModel>>> Schedule { get; set; }
            public required WeekEnum CurrentWeek { get; set; }
        }
    }
}
