namespace ECommerce.DAL

{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category?> GetWithProductsAsync(int id);

        Task<bool> NameExistsAsync(string name, int? excludeId = null);
    }
}
