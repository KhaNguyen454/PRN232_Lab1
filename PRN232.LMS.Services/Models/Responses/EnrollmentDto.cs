using System;

namespace PRN232.LMS.Services.Models.Responses
{
    public class EnrollmentDto
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = string.Empty;

        // Optional expanded data
        public CourseDto? Course { get; set; }
        public StudentDto? Student { get; set; }
    }
}
