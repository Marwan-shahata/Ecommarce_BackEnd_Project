using ECommerce.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.BLL
{
    public interface IProductService
    {
        Task<GeneralResult<PagedResult<ProductResponseDto>>> GetPagedAsync(ProductQueryParams queryParams);

        Task<GeneralResult<ProductResponseDto>> GetByIdAsync(int id);

        Task<GeneralResult<ProductResponseDto>> CreateAsync(CreateProductDto dto);

        Task<GeneralResult<ProductResponseDto>> UpdateAsync(int id, UpdateProductDto dto);

        Task<GeneralResult<bool>> DeleteAsync(int id);

        Task<GeneralResult<ProductResponseDto>> UploadImageAsync(int id, IFormFile image);
    }
}
