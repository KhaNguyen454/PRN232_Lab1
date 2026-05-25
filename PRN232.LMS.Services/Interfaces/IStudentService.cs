using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Interfaces
{
    public interface IStudentService
    {
        Task<ApiResponse<PagedResponse<object>>> GetStudentsAsync(QueryRequest request);
        Task<ApiResponse<StudentDto>> GetStudentByIdAsync(int id);
        Task<ApiResponse<StudentDto>> CreateStudentAsync(StudentDto request);
        Task<ApiResponse<StudentDto>> UpdateStudentAsync(int id, StudentDto request);
        Task<ApiResponse<bool>> DeleteStudentAsync(int id);
    }
}
