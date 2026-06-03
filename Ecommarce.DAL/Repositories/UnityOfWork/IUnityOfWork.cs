
namespace ECommerce.DAL
{
    public interface IUnityOfWork
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        ICartRepository Carts { get; }
        IOrderRepository Orders { get; }

        Task ExecuteInTransactionAsync(Func<Task> action);

        Task<int> SaveChangesAsync();
    }
}
