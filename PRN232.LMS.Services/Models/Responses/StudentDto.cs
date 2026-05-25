using System;
using System.Collections.Generic;

namespace PRN232.LMS.Services.Models.Responses
{
    public class StudentDto
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        
        // Optional expanded data
        public ICollection<EnrollmentDto>? Enrollments { get; set; }
    }
}
