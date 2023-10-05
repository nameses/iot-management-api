using AutoMapper;
using iot_management_api.Entities;
using iot_management_api.Entities.common;
using iot_management_api.Models;
using iot_management_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iot_management_api.Controllers
{
    [ApiController]
    [Route("api/subject")]
    public class SubjectController : Controller
    {
        private readonly ISubjectService _subjectService;
        private readonly IMapper _mapper;
        private readonly ILogger<SubjectController> _logger;

        public SubjectController(ISubjectService subjectService,
            IMapper mapper,
            ILogger<SubjectController> logger)
        {
            _subjectService=subjectService;
            _mapper=mapper;
            _logger=logger;
        }
        [HttpGet]
        [Authorize(Policy = "TeacherAccess")]
        [Route("get/{id}")]
        [ProducesResponseType(typeof(SubjectModel), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var model = await _subjectService.GetById(id);

            if (model==null)
                return NotFound();

            return Ok(_mapper.Map<SubjectModel>(model));
        }

        [HttpPost]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> Create([FromBody] SubjectReq req)
        {
            //if ((await _subjectService.GetByNumber(req.Number))!=null)
            //    return BadRequest("Room already exists");

            var teacherId = HttpContext.User.Claims?.FirstOrDefault(x => x.Type == "id").Value;
            if (teacherId==null)
                return BadRequest("Troubles with current signed in user data");
            //check if exists
            var dbEntity = await _subjectService.GetByName(req.Name, req.Type, int.Parse(teacherId));
            if (dbEntity!=null)
            {
                _logger.LogInformation($"Req subject already exists");
                return BadRequest("Subject already exists");
            }
            var entityId = await _subjectService.CreateAsync(_mapper.Map<Subject>(req), int.Parse(teacherId));

            if (entityId==null)
            {
                _logger.LogInformation($"Subject with name {req.Name} was not created");
                return BadRequest();
            }

            return Ok(new
            {
                CreatedId = entityId
            });
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> Update(int id, [FromBody] SubjectReq room)
        {
            var res = await _subjectService.UpdateAsync(id, _mapper.Map<Subject>(room));

            if (!res) return NotFound();

            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _subjectService.DeleteAsync(id);

            if (!res) return NotFound();

            return Ok();
        }

        public class SubjectReq
        {
            public required string Name { get; set; }
            public required SubjectType Type { get; set; }
        }
    }
}
