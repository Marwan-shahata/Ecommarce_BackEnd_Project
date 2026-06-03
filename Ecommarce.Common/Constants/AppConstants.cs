namespace ECommerce.Common;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Customer = "Customer";
    public const string AdminOrCustomer = "Admin,Customer";
}

public static class Policies
{
    public const string AdminOnly = "AdminOnly";
    public const string CustomerOnly = "CustomerOnly";
    public const string AuthenticatedUser = "AuthenticatedUser";
}

public static class ClaimTypes
{
    public const string UserId = "uid";
    public const string Email = "email";
    public const string FullName = "fullName";
}

public static class ImageSettings
{
    public const long MaxFileSizeBytes = 5 * 1024 * 1024; 
    public static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
    public const string UploadFolder = "wwwroot/uploads";
}

public static class PaginationDefaults
{
    public const int PageNumber = 1;
    public const int PageSize = 10;
    public const int MaxPageSize = 50;
}
