using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using System.Threading.Tasks;

namespace PRN232.LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // GET: api/students
        [HttpGet]
        public async Task<IActionResult> GetStudents([FromQuery] QueryRequest request)
        {
            var result = await _studentService.GetStudentsAsync(request);
            return Ok(result);
        }

        // GET: api/students/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudent(int id)
        {
            var result = await _studentService.GetStudentByIdAsync(id);
            return Ok(result);
        }
        // POST: api/students
        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] StudentDto request)
        {
            var result = await _studentService.CreateStudentAsync(request);
            return CreatedAtAction(nameof(GetStudent), new { id = result.Data?.StudentId }, result);
        }

        // PUT: api/students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentDto request)
        {
            if (id != request.StudentId)
            {
                return BadRequest(ApiResponse<object>.Fail("ID mismatch."));
            }

            var result = await _studentService.UpdateStudentAsync(id, request);
            return Ok(result);
        }

        // DELETE: api/students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var result = await _studentService.DeleteStudentAsync(id);
            return Ok(result);
        }
    }
}
