using ECommerce.BLL;
using ECommerce.Common;
using ECommerce.DAL;
using ECommerce.BLL;
using Microsoft.AspNetCore.Http;

namespace ECommerce.BLL
{


    public class CategoryService : ICategoryService
    {
        private readonly IUnityOfWork _uow;
        private readonly IImageService _imageService;

        public CategoryService(IUnityOfWork uow, IImageService imageService)
        {
            _uow = uow;
            _imageService = imageService;
        }

        public async Task<GeneralResult<IEnumerable<CategoryResponseDto>>> GetAllAsync()
        {
            var categories = await _uow.Categories.GetAllAsync();

            var dtos = categories.Select(MapToDto);

            return GeneralResult<IEnumerable<CategoryResponseDto>>
                .SuccessResult(dtos);
        }

        public async Task<GeneralResult<CategoryResponseDto>> GetByIdAsync(int id)
        {
            var category = await _uow.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return GeneralResult<CategoryResponseDto>
                    .NotFound($"Category with ID {id} not found.");
            }

            return GeneralResult<CategoryResponseDto>
                .SuccessResult(MapToDto(category));
        }

        public async Task<GeneralResult<CategoryResponseDto>> CreateAsync(CreateCategoryDto dto)
        {
            if (await _uow.Categories.NameExistsAsync(dto.Name))
            {
                return GeneralResult<CategoryResponseDto>
                    .Fail($"Category '{dto.Name}' already exists.");
            }

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _uow.Categories.AddAsync(category);
            await _uow.SaveChangesAsync();

            return GeneralResult<CategoryResponseDto>
                .SuccessResult(
                    MapToDto(category),
                    "Category created successfully."
                );
        }

        public async Task<GeneralResult<CategoryResponseDto>> UpdateAsync(
            int id,
            UpdateCategoryDto dto)
        {
            var category = await _uow.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return GeneralResult<CategoryResponseDto>
                    .NotFound($"Category with ID {id} not found.");
            }

            if (await _uow.Categories.NameExistsAsync(dto.Name, excludeId: id))
            {
                return GeneralResult<CategoryResponseDto>
                    .Fail($"Category '{dto.Name}' already exists.");
            }

            category.Name = dto.Name;
            category.Description = dto.Description;

            _uow.Categories.Update(category);

            await _uow.SaveChangesAsync();

            return GeneralResult<CategoryResponseDto>
                .SuccessResult(
                    MapToDto(category),
                    "Category updated successfully."
                );
        }

        public async Task<GeneralResult<bool>> DeleteAsync(int id)
        {
            var category = await _uow.Categories.GetWithProductsAsync(id);

            if (category == null)
            {
                return GeneralResult<bool>
                    .NotFound($"Category with ID {id} not found.");
            }

            if (category.Products.Any(p => !p.IsDeleted))
            {
                return GeneralResult<bool>
                    .Fail("Cannot delete a category that has active products.");
            }

            category.IsDeleted = true;

            _uow.Categories.Update(category);

            await _uow.SaveChangesAsync();

            return GeneralResult<bool>
                .SuccessResult(true, "Category deleted successfully.");
        }

        public async Task<GeneralResult<CategoryResponseDto>> UploadImageAsync(
            int id,
            IFormFile image)
        {
            var category = await _uow.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return GeneralResult<CategoryResponseDto>
                    .NotFound($"Category with ID {id} not found.");
            }

            var uploadResult = await _imageService.UploadImageAsync(
                image,
                "categories");

            if (!uploadResult.Success)
            {
                return GeneralResult<CategoryResponseDto>
                    .Fail(uploadResult.Message);
            }

            category.ImageUrl = uploadResult.Data!.Url;

            _uow.Categories.Update(category);

            await _uow.SaveChangesAsync();

            return GeneralResult<CategoryResponseDto>
                .SuccessResult(
                    MapToDto(category),
                    "Image uploaded successfully."
                );
        }

        // ─────────────────────────────────────────────────────────────
        // Mapper
        // ─────────────────────────────────────────────────────────────

        private static CategoryResponseDto MapToDto(Category c)
        {
            return new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                ProductCount = c.Products?.Count(p => !p.IsDeleted) ?? 0,
                CreatedAt = c.CreatedAt
            };
        }
    }
}
