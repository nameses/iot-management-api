using AutoMapper;
using iot_management_api.Entities.common;
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
        private readonly IMapper _mapper;
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(IScheduleService scheduleService, IMapper mapper, ILogger<ScheduleController> logger)
        {
            _scheduleService = scheduleService;
            _mapper=mapper;
            _logger=logger;
        }

        [HttpGet]
        [Authorize]
        //[Route("full")]
        //[ProducesResponseType(typeof(Dictionary<, ScheduleModel>), 200)]
        public async Task<IActionResult> GetFull()
        {
            var userId = int.Parse(HttpContext.User.Claims?.First(x => x.Type == "id").Value!);
            var userRole = Enum.Parse<UserRole>(HttpContext.User.Claims?.First(x => x.Type == "role").Value!);

            var scheduleDict = await _scheduleService.GetFull(userRole, userId);

            if (scheduleDict==null)
                return NotFound();

            return Ok(scheduleDict);
            //    new
            //{
            //    schedule = scheduleDict,
            //currentWeek = GetCurrentWeek(),
            //});
        }
        //private int GetCurrentWeek()
        //{
        //    var date = DateTime.Now;
        //    DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
        //    DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
        //    if (firstMonthMonday > date)
        //    {
        //        firstMonthDay = firstMonthDay.AddMonths(-1);
        //        firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
        //    }
        //    return (date - firstMonthMonday).Days / 7 + 1;
        //}
    }
}
