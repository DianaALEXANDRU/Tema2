using Core.Services;
using DataLayer.Dtos;
using DataLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace Project.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private UserService userService { get; set; }

        public UsersController(UserService userService)
        {
            this.userService = userService;
        }


        [HttpPost("/register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterDto payload)
        {
            userService.Register(payload);
            return Ok();
        }

        [HttpPost("/login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginDto payload)
        {
            var jwtToken = userService.Validate(payload);
            if(jwtToken == null)
            {
                return Unauthorized();

            }
            return Ok(new { token = jwtToken });
        }


        [HttpGet("students-only")]
        [Authorize(Roles = "Student")]
        public ActionResult<string> HelloStudents()
        {
            return Ok("Hello students!");
        }

        [HttpGet("teacher-only")]
        [Authorize(Roles = "Profesor")]
        public ActionResult<string> HelloTeachers()
        {
            return Ok("Hello teachers!");
        }

        [HttpGet("geades")]
        [Authorize(Roles = "Profesor, Student")]

        public IActionResult GetGrades()
        {
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == null)
            {
                return Unauthorized();
            }

            if (role == "Student")
            {
                var id = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (id == null)
                {
          
                    return BadRequest();
                }

                var grades = userService.GetGradesByStudent(int.Parse(id));
                return Ok(grades);
            }
            else
            {
                var grades = userService.GetGradesForTeacher();
                
                return Ok(grades);
            }
        }

      

    }
}
