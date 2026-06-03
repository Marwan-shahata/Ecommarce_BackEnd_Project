using ECommerce.Common;

namespace ECommerce.DAL

{
    public interface IProductRepository : IGenericRepository<Product>
    {
        public Task<IEnumerable<Product>> GetAllWithCategoryAsync();

        //Task<IEnumerable<Product>> SearchProductsAsync(string name);

        Task<Product?> GetWithCategoryAsync(int id);
        /// <summary>
        /// Handles filtering, searching, sorting, and pagination at the DB level.
        /// Doing this in EF (not in memory) is critical for performance at scale.
        /// </summary>
        Task<PagedResult<Product>> GetPagedAsync(ProductQueryParams queryParams);


    }
} 
