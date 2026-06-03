using ECommerce.Common;
using ECommerce.DAL;
using ECommerce.BLL;
using Microsoft.AspNetCore.Http;

namespace ECommerce.BLL
{

    public class ProductService : IProductService
    {
        private readonly IUnityOfWork _uow;
        private readonly IImageService _imageService;

        public ProductService(IUnityOfWork uow, IImageService imageService)
        {
            _uow = uow;
            _imageService = imageService;
        }

        public async Task<GeneralResult<PagedResult<ProductResponseDto>>> GetPagedAsync(ProductQueryParams queryParams)
        {
            var paged = await _uow.Products.GetPagedAsync(queryParams);

            var dtoItems = paged.Items.Select(MapToDto);

            var result = PagedResult<ProductResponseDto>.Create(
                dtoItems,
                paged.TotalCount,
                paged.PageNumber,
                paged.PageSize
            );

            return GeneralResult<PagedResult<ProductResponseDto>>
                .SuccessResult(result);
        }

        public async Task<GeneralResult<ProductResponseDto>> GetByIdAsync(int id)
        {
            var product = await _uow.Products.GetWithCategoryAsync(id);

            if (product == null)
            {
                return GeneralResult<ProductResponseDto>
                    .NotFound($"Product with ID {id} not found.");
            }

            return GeneralResult<ProductResponseDto>
                .SuccessResult(MapToDto(product));
        }

        public async Task<GeneralResult<ProductResponseDto>> CreateAsync(CreateProductDto dto)
        {
            var categoryExists = await _uow.Categories.ExistsAsync(c => c.Id == dto.CategoryId);

            if (!categoryExists)
            {
                return GeneralResult<ProductResponseDto>
                    .NotFound($"Category with ID {dto.CategoryId} not found.");
            }

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                CategoryId = dto.CategoryId
            };

            await _uow.Products.AddAsync(product);
            await _uow.SaveChangesAsync();

            var created = await _uow.Products.GetWithCategoryAsync(product.Id);

            return GeneralResult<ProductResponseDto>
                .SuccessResult(MapToDto(created!), "Product created successfully.");
        }

        public async Task<GeneralResult<ProductResponseDto>> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _uow.Products.GetByIdAsync(id);

            if (product == null)
            {
                return GeneralResult<ProductResponseDto>
                    .NotFound($"Product with ID {id} not found.");
            }

            var categoryExists = await _uow.Categories.ExistsAsync(c => c.Id == dto.CategoryId);

            if (!categoryExists)
            {
                return GeneralResult<ProductResponseDto>
                    .NotFound($"Category with ID {dto.CategoryId} not found.");
            }

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.CategoryId = dto.CategoryId;

            _uow.Products.Update(product);
            await _uow.SaveChangesAsync();

            var updated = await _uow.Products.GetWithCategoryAsync(product.Id);

            return GeneralResult<ProductResponseDto>
                .SuccessResult(MapToDto(updated!), "Product updated successfully.");
        }

        public async Task<GeneralResult<bool>> DeleteAsync(int id)
        {
            var product = await _uow.Products.GetByIdAsync(id);

            if (product == null)
            {
                return GeneralResult<bool>
                    .NotFound($"Product with ID {id} not found.");
            }

            product.IsDeleted = true;

            _uow.Products.Update(product);
            await _uow.SaveChangesAsync();

            return GeneralResult<bool>
                .SuccessResult(true, "Product deleted successfully.");
        }

        public async Task<GeneralResult<ProductResponseDto>> UploadImageAsync(int id, IFormFile image)
        {
            var product = await _uow.Products.GetWithCategoryAsync(id);

            if (product == null)
            {
                return GeneralResult<ProductResponseDto>
                    .NotFound($"Product with ID {id} not found.");
            }

            var uploadResult = await _imageService.UploadImageAsync(image, "products");

            if (!uploadResult.Success)
            {
                return GeneralResult<ProductResponseDto>
                    .Fail(uploadResult.Message);
            }

            product.ImageUrl = uploadResult.Data!.Url;

            _uow.Products.Update(product);
            await _uow.SaveChangesAsync();

            return GeneralResult<ProductResponseDto>
                .SuccessResult(MapToDto(product), "Image uploaded successfully.");
        }

        // ─────────────────────────────────────────────────────────────
        // Mapper
        // ─────────────────────────────────────────────────────────────

        private static ProductResponseDto MapToDto(Product p) {
            return new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? string.Empty,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }
        
    }
}