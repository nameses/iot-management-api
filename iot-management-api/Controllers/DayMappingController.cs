using AutoMapper;
using iot_management_api.Models;
using iot_management_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iot_management_api.Controllers
{
    [ApiController]
    [Route("api/dayMappings")]
    public class DayMappingController : Controller
    {
        private readonly IDayMappingService _dayMappingService;
        private readonly IMapper _mapper;
        private readonly ILogger<DayMappingController> _logger;

        public DayMappingController(IDayMappingService dayMappingService,
            IMapper mapper,
            ILogger<DayMappingController> logger)
        {
            _dayMappingService=dayMappingService;
            _mapper=mapper;
            _logger=logger;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<DayMappingModel>), 200)]
        public async Task<IActionResult> GetAvailable()
        {
            var entities = await _dayMappingService.GetAllAsync();

            if (entities==null)
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<DayMappingModel>>(entities));
        }
    }
}
