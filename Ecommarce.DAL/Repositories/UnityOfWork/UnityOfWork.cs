

namespace ECommerce.DAL
{
    public class UnityOfWork : IUnityOfWork
    {
        private readonly AppDbContext _context;

        public ICategoryRepository Categories {  get;}

        public IProductRepository Products { get; }
        public ICartRepository Carts { get; }
        public IOrderRepository Orders { get; }

        public UnityOfWork(AppDbContext context,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            ICartRepository cartRepository,
            IOrderRepository orderRepository)
        {
            _context=context;
            Products = productRepository;
            Categories = categoryRepository;
            Carts = cartRepository;
            Orders = orderRepository;
        }
        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await action();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
