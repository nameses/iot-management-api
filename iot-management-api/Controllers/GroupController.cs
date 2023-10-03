using AutoMapper;
using iot_management_api.Entities;
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
        [Authorize(Policy = "TeacherAccess")]
        [Route("get/{id}")]
        [ProducesResponseType(typeof(GroupModel), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var group = await _groupService.GetById(id);

            if (group==null)
                return NotFound();

            return Ok(_mapper.Map<GroupModel>(group));
        }

        [HttpGet]
        [Authorize(Policy = "TeacherAccess")]
        [Route("getByGroupCode/{groupcode}")]
        [ProducesResponseType(typeof(GroupModel), 200)]
        public async Task<IActionResult> GetByGroupCode(string groupcode)
        {
            var group = await _groupService.GetByGroupCode(groupcode);

            if (group==null)
                return NotFound();

            return Ok(_mapper.Map<GroupModel>(group));
        }

        [HttpPost]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> Create([FromBody] GroupReq group)
        {
            if ((await _groupService.GetByGroupCode(group.GroupCode))!=null)
                return BadRequest("Group already exists");

            var groupId = await _groupService.CreateAsync(_mapper.Map<Group>(group));

            if (groupId==null)
            {
                _logger.LogInformation($"Group with group code {group.GroupCode} was not created");
                return NotFound();
            }

            return Ok(new
            {
                CreatedId = groupId
            });
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> Update(int id, [FromBody] GroupReq group)
        {
            var res = await _groupService.UpdateAsync(id, _mapper.Map<Group>(group));

            if (!res) return NotFound();

            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _groupService.DeleteAsync(id);

            if (!res) return NotFound();

            return Ok();
        }

        public class GroupReq
        {
            public required string GroupCode { get; set; }
            public required int Term { get; set; }
        }
    }
}
