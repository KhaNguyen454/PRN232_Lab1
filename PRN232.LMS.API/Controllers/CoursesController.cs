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
            return Ok(result);
        }

        // GET: api/courses/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var result = await _courseService.GetCourseByIdAsync(id);
            return Ok(result);
        }

        // POST: api/courses
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseDto request)
        {
            var result = await _courseService.CreateCourseAsync(request);
            return CreatedAtAction(nameof(GetCourse), new { id = result.Data?.CourseId }, result);
        }

        // PUT: api/courses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseDto request)
        {
            if (id != request.CourseId)
            {
                return BadRequest(ApiResponse<object>.Fail("ID mismatch."));
            }

            var result = await _courseService.UpdateCourseAsync(id, request);
            return Ok(result);
        }

        // DELETE: api/courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var result = await _courseService.DeleteCourseAsync(id);
            return Ok(result);
        }
    }
}
