using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories.Implementations
{
    public class EnrollmentRepository : GenericRepository<Enrollment>, Interfaces.IEnrollmentRepository
    {
        public EnrollmentRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
