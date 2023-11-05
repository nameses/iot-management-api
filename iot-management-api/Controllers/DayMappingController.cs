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
        /// <summary>
        /// Get a list of day mapping (lesson number, its start and end)
        /// </summary>
        /// <returns>List of daymappings</returns>
        /// <response code="200">Request Successful</response>
        /// <response code="401">Unathorized</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<DayMappingModel>), 200)]
        public async Task<IActionResult> GetAvailable()
        {
            var entities = await _dayMappingService.GetAllAsync();

            if (entities==null)
                return Ok(Array.Empty<string>());

            return Ok(_mapper.Map<IEnumerable<DayMappingModel>>(entities));
        }
    }
}
