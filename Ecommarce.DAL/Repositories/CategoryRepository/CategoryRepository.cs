using Microsoft.EntityFrameworkCore;

namespace ECommerce.DAL
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        /*------------------------------------------------------------------*/
        public CategoryRepository(AppDbContext context): base(context)
        { }
        /*------------------------------------------------------------------*/

        public async Task<Category?> GetWithProductsAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            var query = _context.Categories.Where(c => c.Name.ToLower() == name.ToLower());
            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);
            return await query.AnyAsync();
        }


    }
}
