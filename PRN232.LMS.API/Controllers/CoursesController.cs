using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using System.Threading.Tasks;

namespace PRN232.LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        // GET: api/courses
        [HttpGet]
        public async Task<IActionResult> GetCourses([FromQuery] QueryRequest request)
        {
            var result = await _courseService.GetCoursesAsync(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // GET: api/courses/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var result = await _courseService.GetCourseByIdAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            if (result.Message == "Course not found.")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        // POST: api/courses
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseDto request)
        {
            var result = await _courseService.CreateCourseAsync(request);
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetCourse), new { id = result.Data?.CourseId }, result);
            }
            return BadRequest(result);
        }

        // PUT: api/courses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseDto request)
        {
            if (id != request.CourseId)
            {
                return BadRequest(ApiResponse<CourseDto>.Fail("ID mismatch."));
            }

            var result = await _courseService.UpdateCourseAsync(id, request);
            if (result.Success)
            {
                return Ok(result);
            }
            if (result.Message == "Course not found.")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        // DELETE: api/courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var result = await _courseService.DeleteCourseAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            if (result.Message == "Course not found.")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }
    }
}
