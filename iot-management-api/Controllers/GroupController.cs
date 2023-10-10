using AutoMapper;
using iot_management_api.Models;
using iot_management_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace iot_management_api.Controllers
{
    [ApiController]
    [Route("api/group")]
    public class GroupController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;
        private readonly ILogger<GroupController> _logger;

        public GroupController(IGroupService groupService, IMapper mapper, ILogger<GroupController> logger)
        {
            _groupService = groupService;
            _mapper=mapper;
            _logger=logger;
        }
        [HttpGet]
        [ProducesResponseType(typeof(GroupModel), 200)]
        public async Task<IActionResult> GetAll()
        {
            var group = await _groupService.GetAllAsync();

            if (group==null)
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<GroupModel>?>(group));
        }
    }
}