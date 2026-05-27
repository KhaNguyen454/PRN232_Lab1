using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using System.Threading.Tasks;

namespace PRN232.LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // GET: api/enrollments
        [HttpGet]
        public async Task<IActionResult> GetEnrollments([FromQuery] QueryRequest request)
        {
            var result = await _enrollmentService.GetEnrollmentsAsync(request);
            return Ok(result);
        }

        // GET: api/enrollments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrollment(int id)
        {
            var result = await _enrollmentService.GetEnrollmentByIdAsync(id);
            return Ok(result);
        }
        // POST: api/enrollments
        [HttpPost]
        public async Task<IActionResult> CreateEnrollment([FromBody] EnrollmentDto request)
        {
            var result = await _enrollmentService.CreateEnrollmentAsync(request);
            return CreatedAtAction(nameof(GetEnrollment), new { id = result.Data?.EnrollmentId }, result);
        }

        // PUT: api/enrollments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnrollment(int id, [FromBody] EnrollmentDto request)
        {
            if (id != request.EnrollmentId)
            {
                return BadRequest(ApiResponse<object>.Fail("ID mismatch."));
            }

            var result = await _enrollmentService.UpdateEnrollmentAsync(id, request);
            return Ok(result);
        }

        // DELETE: api/enrollments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var result = await _enrollmentService.DeleteEnrollmentAsync(id);
            return Ok(result);
        }
    }
}
