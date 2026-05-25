using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Interfaces
{
    public interface ISemesterService
    {
        Task<ApiResponse<PagedResponse<object>>> GetSemestersAsync(QueryRequest request);
        Task<ApiResponse<SemesterDto>> GetSemesterByIdAsync(int id);
        Task<ApiResponse<SemesterDto>> CreateSemesterAsync(SemesterDto request);
        Task<ApiResponse<SemesterDto>> UpdateSemesterAsync(int id, SemesterDto request);
        Task<ApiResponse<bool>> DeleteSemesterAsync(int id);
    }
}
