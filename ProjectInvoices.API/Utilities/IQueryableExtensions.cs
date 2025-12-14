namespace ProjectInvoices.API.Utilities
{
    /// <summary>
    /// Provides extension methods on IQueryable
    /// </summary>
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Paginates the calling IQueryable object
        /// </summary>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int pageSize)         
        {
            if (page <= 0)
                page = 1;

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
