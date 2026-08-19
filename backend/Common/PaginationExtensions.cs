using Microsoft.EntityFrameworkCore;

namespace backend.Common
{
    public static class PaginationExtensions
    {
        public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(this IQueryable<T> query, PaginationRequest request)
        {
            var totalRecords = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize);

            var data = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResponse<T>
            {
                Data = data,
                Pagination = new PaginationMetadata
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    HasPrevious = request.PageNumber > 1,
                    HasNext = request.PageNumber < totalPages
                }
            };
        }
    }
}