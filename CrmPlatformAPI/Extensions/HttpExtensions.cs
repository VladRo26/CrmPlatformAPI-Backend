using CrmPlatformAPI.Helpers;
using System.Text.Json;

namespace CrmPlatformAPI.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPagination<T>(this HttpResponse response, PagedList<T> data)
        {
            var paginationHeader = new PaginationHeader(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, jsonOptions));

            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");

        }
        
    }
}
