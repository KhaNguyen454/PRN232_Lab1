using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories.Implementations
{
    public class CourseRepository : GenericRepository<Course>, Interfaces.ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
