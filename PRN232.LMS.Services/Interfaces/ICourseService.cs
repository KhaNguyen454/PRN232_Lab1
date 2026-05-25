using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Interfaces
{
    public interface ICourseService
    {
        Task<ApiResponse<PagedResponse<object>>> GetCoursesAsync(QueryRequest request);
        Task<ApiResponse<CourseDto>> GetCourseByIdAsync(int id);
        Task<ApiResponse<CourseDto>> CreateCourseAsync(CourseDto request);
        Task<ApiResponse<CourseDto>> UpdateCourseAsync(int id, CourseDto request);
        Task<ApiResponse<bool>> DeleteCourseAsync(int id);
    }
}
