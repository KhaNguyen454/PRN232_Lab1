using System;

namespace PRN232.LMS.Services.Models.Responses
{
    public class SemesterDto
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
