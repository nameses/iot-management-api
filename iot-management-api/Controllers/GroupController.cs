using AutoMapper;
using iot_management_api.Models;
using iot_management_api.Services;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [Route("get/{id}")]
        [ProducesResponseType(typeof(GroupModel), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var group = await _groupService.GetById(id);

            if (group==null)
            {
                _logger.LogInformation($"Group with id={id} not found");
                return NotFound();
            }

            return Ok(new
            {
                Student = _mapper.Map<GroupModel>(group)
            });
        }
    }
}
