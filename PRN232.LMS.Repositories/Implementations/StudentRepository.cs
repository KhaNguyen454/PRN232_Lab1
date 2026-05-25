using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories.Implementations
{
    public class StudentRepository : GenericRepository<Student>, Interfaces.IStudentRepository
    {
        public StudentRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
