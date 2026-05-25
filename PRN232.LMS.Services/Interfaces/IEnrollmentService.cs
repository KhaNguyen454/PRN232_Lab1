using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<ApiResponse<PagedResponse<object>>> GetEnrollmentsAsync(QueryRequest request);
        Task<ApiResponse<EnrollmentDto>> GetEnrollmentByIdAsync(int id);
        Task<ApiResponse<EnrollmentDto>> CreateEnrollmentAsync(EnrollmentDto request);
        Task<ApiResponse<EnrollmentDto>> UpdateEnrollmentAsync(int id, EnrollmentDto request);
        Task<ApiResponse<bool>> DeleteEnrollmentAsync(int id);
    }
}
