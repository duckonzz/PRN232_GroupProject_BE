using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Contract.Repositories.PaggingItems
{
    public static class IQueryableExtension
    {
        public static Task<PaginatedList<T>> GetPaginatedList<T>(this IQueryable<T> source, int pageIndex, int pageSize) where T : class
            => PaginatedList<T>.CreateAsync(source.AsNoTracking(), pageIndex, pageSize);

        public static Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable, IConfigurationProvider configuration) where TDestination : class
            => queryable.ProjectTo<TDestination>(configuration).AsNoTracking().ToListAsync();

        public static async Task<BasePaginatedList<TDestination>> ToPagedListAllAsync<TSource, TDestination>(
            this IQueryable<TSource> source,
            IMapper mapper
            )
        {
            IEnumerable<TSource> items = source;
            IList<TDestination> mappedItems = mapper.Map<IList<TDestination>>(items);
            return await Task.FromResult(new BasePaginatedList<TDestination>(mappedItems.ToList(), mappedItems.Count, 1, mappedItems.Count));
        }

        public static async Task<BasePaginatedList<TDestination>> ToPagedListAsync<TSource, TDestination>(
            this IQueryable<TSource> source,
            IMapper mapper,
            int pageIndex,
            int pageSize)
        {
            int count = await source.CountAsync();
            IEnumerable<TSource> items = await source
                                        .Skip((pageIndex - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();

            // Mapping từ TSource sang TDestination
            IList<TDestination> mappedItems = mapper.Map<IList<TDestination>>(items);

            // Trả về danh sách đã paging
            return new BasePaginatedList<TDestination>(mappedItems.ToList(), count, pageIndex, pageSize);
        }
    }
}
