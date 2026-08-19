namespace backend.Common
{
    public class PagedResponse<T>
    {
        public IReadOnlyList<T> Data { get; set; } = [];

        public PaginationMetadata Pagination { get; set; } = new();
    }

    public class PaginationMetadata
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }

        public bool HasPrevious { get; set; }

        public bool HasNext { get; set; }
    }
}
