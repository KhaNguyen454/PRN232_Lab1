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
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // GET: api/semesters/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSemester(int id)
        {
            var result = await _semesterService.GetSemesterByIdAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            if (result.Message == "Semester not found.")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        // POST: api/semesters
        [HttpPost]
        public async Task<IActionResult> CreateSemester([FromBody] SemesterDto request)
        {
            var result = await _semesterService.CreateSemesterAsync(request);
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetSemester), new { id = result.Data?.SemesterId }, result);
            }
            return BadRequest(result);
        }

        // PUT: api/semesters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSemester(int id, [FromBody] SemesterDto request)
        {
            if (id != request.SemesterId)
            {
                return BadRequest(ApiResponse<SemesterDto>.Fail("ID mismatch."));
            }

            var result = await _semesterService.UpdateSemesterAsync(id, request);
            if (result.Success)
            {
                return Ok(result);
            }
            if (result.Message == "Semester not found.")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        // DELETE: api/semesters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSemester(int id)
        {
            var result = await _semesterService.DeleteSemesterAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            if (result.Message == "Semester not found.")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }
    }
}
