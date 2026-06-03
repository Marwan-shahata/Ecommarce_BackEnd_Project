using ECommerce.Common;
using Microsoft.EntityFrameworkCore;
namespace ECommerce.DAL
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository 
    {
        /*------------------------------------------------------------------*/
        public ProductRepository(AppDbContext context): base(context) { }
       
        public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()
        {
            return await _context.Products.Include(e => e.Category).AsNoTracking().ToListAsync();
        }
        
        public async Task<PagedResult<Product>> GetPagedAsync(ProductQueryParams q)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // Filtering — each condition layered on the IQueryable (translated to SQL WHERE)
            if (q.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == q.CategoryId.Value);

            if (!string.IsNullOrWhiteSpace(q.Name))
                query = query.Where(p => p.Name.Contains(q.Name));

            if (q.MinPrice.HasValue)
                query = query.Where(p => p.Price >= q.MinPrice.Value);

            if (q.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= q.MaxPrice.Value);

            // Sorting
            query = q.SortBy?.ToLower() switch
            {
                "price" => q.SortDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "name" => q.SortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                _ => q.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            // Clamp page size to prevent abuse
            var pageSize = Math.Min(q.PageSize, PaginationDefaults.MaxPageSize);
            var items = await query
                .Skip((q.PageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return PagedResult<Product>.Create(items, totalCount, q.PageNumber, pageSize);
        }

        public async Task<Product?> GetWithCategoryAsync(int id)
            => await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);




    }
}
