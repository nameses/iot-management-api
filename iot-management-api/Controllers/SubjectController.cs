using AutoMapper;
using iot_management_api.Entities;
using iot_management_api.Entities.common;
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


        [HttpPost]
        [Authorize(Policy = "TeacherAccess")]
        public async Task<IActionResult> Create([FromBody] SubjectReq req)
        {
            var userId = int.Parse(HttpContext.User.Claims?.First(x => x.Type == "id").Value!);

            var entityId = await _subjectService.CreateAsync(_mapper.Map<Subject>(req), userId);

            if (entityId==null)
                return BadRequest();

            return Ok(new
            {
                CreatedId = entityId
            });
        }

        public class SubjectReq
        {
            public required string Name { get; set; }
            public required SubjectType Type { get; set; }
        }
    }
}
