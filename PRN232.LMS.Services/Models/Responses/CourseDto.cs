namespace PRN232.LMS.Services.Models.Responses
{
    public class CourseDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int SemesterId { get; set; }

        public SemesterDto? Semester { get; set; }
    }
}
