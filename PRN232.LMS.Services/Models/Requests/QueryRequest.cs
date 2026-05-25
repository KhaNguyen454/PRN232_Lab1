namespace PRN232.LMS.Services.Models.Requests
{
    public class QueryRequest
    {
        public string? Keyword { get; set; }
        public string? SortBy { get; set; } // e.g., "FullName desc, DateOfBirth asc"
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Select { get; set; } // e.g., "StudentId,FullName"
        public string? Expand { get; set; } // e.g., "Enrollments"
    }
}
