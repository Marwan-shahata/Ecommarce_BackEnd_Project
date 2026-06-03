using ECommerce.Common;
using ECommerce.BLL;
using Microsoft.AspNetCore.Http;
namespace ECommerce.BLL
{
    public interface ICategoryService
    {
        Task<GeneralResult<IEnumerable<CategoryResponseDto>>> GetAllAsync();
        Task<GeneralResult<CategoryResponseDto>> GetByIdAsync(int id);
        Task<GeneralResult<CategoryResponseDto>> CreateAsync(CreateCategoryDto dto);
        Task<GeneralResult<CategoryResponseDto>> UpdateAsync(int id, UpdateCategoryDto dto);
        Task<GeneralResult<bool>> DeleteAsync(int id);
        Task<GeneralResult<CategoryResponseDto>> UploadImageAsync(int id, IFormFile image);

    }
}