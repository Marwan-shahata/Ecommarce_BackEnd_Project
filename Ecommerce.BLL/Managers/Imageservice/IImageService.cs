using ECommerce.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.BLL
{

    public interface IImageService
    {
      Task<GeneralResult<ImageUploadResponseDto>> UploadImageAsync(IFormFile file, string subfolder = "general");
    }

}
