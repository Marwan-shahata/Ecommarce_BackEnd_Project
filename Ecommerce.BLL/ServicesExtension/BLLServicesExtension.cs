
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.BLL
{
    public static class BLLServicesExtension
    {
        public static void AddBLLServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddValidatorsFromAssembly(typeof(BLLServicesExtension).Assembly);
        }
    }
}
