using ECommerce.Common;
using ECommerce.BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ECommerce.BLL;

public class ImageService : IImageService
{
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ImageService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
        _config = config;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<GeneralResult<ImageUploadResponseDto>> UploadImageAsync(
        IFormFile file,
        string subfolder = "general")
    {
        // ── Validate file ─────────────────────────────────────────────

        if (file == null || file.Length == 0)
        {
            return GeneralResult<ImageUploadResponseDto>
                .Fail("No file uploaded.");
        }

        if (file.Length > ImageSettings.MaxFileSizeBytes)
        {
            return GeneralResult<ImageUploadResponseDto>
                .Fail($"File size exceeds the {ImageSettings.MaxFileSizeBytes / 1024 / 1024}MB limit.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!ImageSettings.AllowedExtensions.Contains(extension))
        {
            return GeneralResult<ImageUploadResponseDto>
                .Fail($"File type not allowed. Allowed types: {string.Join(", ", ImageSettings.AllowedExtensions)}");
        }

        // ── Build safe file path ──────────────────────────────────────

        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var uploadPath = Path.Combine(ImageSettings.UploadFolder, subfolder);

        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var filePath = Path.Combine(uploadPath, uniqueFileName);

        // ── Save file ─────────────────────────────────────────────────

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // ── Build public URL ──────────────────────────────────────────

        var request = _httpContextAccessor.HttpContext?.Request;

        var baseUrl = request != null
            ? $"{request.Scheme}://{request.Host}"
            : _config["AppSettings:BaseUrl"] ?? "http://localhost:5000";

        var url = $"{baseUrl}/uploads/{subfolder}/{uniqueFileName}";

        // ── Response DTO ──────────────────────────────────────────────

        var response = new ImageUploadResponseDto
        {
            FileName = uniqueFileName,
            Url = url,
            FileSizeBytes = file.Length,
            ContentType = file.ContentType
        };
        
        return GeneralResult<ImageUploadResponseDto>
            .SuccessResult(response, "Image uploaded successfully.");
    }
}