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

        [HttpGet]
        [Authorize]
        [Route("full")]
        [ProducesResponseType(typeof(ScheduleFullResponse), 200)]
        public async Task<IActionResult> GetFull()
        {
            var userId = int.Parse(HttpContext.User.Claims?.First(x => x.Type == "id").Value!);
            var userRole = Enum.Parse<UserRole>(HttpContext.User.Claims?.First(x => x.Type == "role").Value!);

            var scheduleDict = await _scheduleService.GetFullAsync(userRole, userId);

            if (scheduleDict==null)
                return NotFound();

            return Ok(new ScheduleFullResponse
            {
                Schedule = scheduleDict,
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
