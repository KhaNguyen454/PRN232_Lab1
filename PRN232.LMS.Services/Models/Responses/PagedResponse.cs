namespace PRN232.LMS.Services.Models.Responses
{
    public class PagedResponse<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public T Data { get; set; } = default!;

        public PagedResponse(T data, int page, int pageSize, int totalItems, int totalPages)
        {
            Data = data;
            Page = page;
            PageSize = pageSize;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }
    }
}
