using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using System.Threading.Tasks;

namespace PRN232.LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _semesterService;

        public SemestersController(ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        // GET: api/semesters
        [HttpGet]
        public async Task<IActionResult> GetSemesters([FromQuery] QueryRequest request)
        {
            var result = await _semesterService.GetSemestersAsync(request);
            return Ok(result);
        }

        // GET: api/semesters/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSemester(int id)
        {
            var result = await _semesterService.GetSemesterByIdAsync(id);
            return Ok(result);
        }

        // POST: api/semesters
        [HttpPost]
        public async Task<IActionResult> CreateSemester([FromBody] SemesterDto request)
        {
            var result = await _semesterService.CreateSemesterAsync(request);
            return CreatedAtAction(nameof(GetSemester), new { id = result.Data?.SemesterId }, result);
        }

        // PUT: api/semesters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSemester(int id, [FromBody] SemesterDto request)
        {
            if (id != request.SemesterId)
            {
                return BadRequest(ApiResponse<object>.Fail("ID mismatch."));
            }

            var result = await _semesterService.UpdateSemesterAsync(id, request);
            return Ok(result);
        }

        // DELETE: api/semesters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSemester(int id)
        {
            var result = await _semesterService.DeleteSemesterAsync(id);
            return Ok(result);
        }
    }
}
