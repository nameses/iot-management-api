﻿using AutoMapper;
using iot_management_api.Models;
using iot_management_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iot_management_api.Controllers
{
    [ApiController]
    [Route("api/student")]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentController> _logger;

        public StudentController(IStudentService studentService, IMapper mapper, ILogger<StudentController> logger)
        {
            _studentService = studentService;
            _mapper=mapper;
            _logger=logger;
        }
        [HttpGet]
        [Authorize]
        [Route("get/{id}")]
        //[ProducesResponseType(typeof(), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _studentService.GetById(id);

            if (student==null)
            {
                _logger.LogInformation($"Student with id={id} not found");
                return NotFound();
            }

            return Ok(new
            {
                Student = _mapper.Map<StudentModel>(student)
            });
        }
    }
}