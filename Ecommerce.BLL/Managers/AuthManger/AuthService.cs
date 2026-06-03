using ECommerce.Common;
using ECommerce.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClaimTypes = ECommerce.Common.ClaimTypes;

namespace ECommerce.BLL;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _config;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration config)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _config = config;
    }

    public async Task<GeneralResult<AuthResponseDto>> RegisterAsync(RegisterRequestDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);

        if (existingUser != null)
        {
            return GeneralResult<AuthResponseDto>
                .Fail("Email is already registered.");
        }

        var user = new ApplicationUser
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            PhoneNumber = dto.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            
            var errors = new Dictionary<string, List<Errors>>
            {
                ["Identity"] = result.Errors.Select(e => new Errors
                {
                    Code = e.Code,
                    Description = e.Description
                }).ToList()
            };

            return GeneralResult<AuthResponseDto>
                .Fail(errors, "Registration failed.");
        }

        //await EnsureRoleExistsAsync(Roles.Customer);
        //await _userManager.AddToRoleAsync(user, Roles.Customer);

        //var roles = await _userManager.GetRolesAsync(user);
        //var token = GenerateJwtToken(user, roles);

        //var response = BuildAuthResponse(user, roles, token);

        //return GeneralResult<AuthResponseDto>
        //    .SuccessResult(response, "Registration successful.");
        var allowedRoles = new[] { Roles.Customer, Roles.Admin };

        var role = allowedRoles.Contains(dto.Role)
            ? dto.Role
            : Roles.Customer;

        await EnsureRoleExistsAsync(role);
        await _userManager.AddToRoleAsync(user, role);

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        var response = BuildAuthResponse(user, roles, token);

        return GeneralResult<AuthResponseDto>
            .SuccessResult(response, "Registration successful.");
    }

    public async Task<GeneralResult<AuthResponseDto>> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null || !user.IsActive)
        {
            return GeneralResult<AuthResponseDto>
                .Fail("Invalid email or password.");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!passwordValid)
        {
            return GeneralResult<AuthResponseDto>
                .Fail("Invalid email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        var response = BuildAuthResponse(user, roles, token);

        return GeneralResult<AuthResponseDto>
            .SuccessResult(response, "Login successful.");
    }

    // ─────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────

    //private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
    //{
    //    var jwtSettings = _config.GetSection("JwtSettings");

    //    var key = new SymmetricSecurityKey(
    //        Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)
    //    );

    //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //    var expiry = DateTime.UtcNow.AddMinutes(
    //        double.Parse(jwtSettings["ExpirationMinutes"]!)
    //    );


    //    var claims = new List<Claim>
    //    {
    //        new(ClaimTypes.UserId, user.Id),
    //        new(ClaimTypes.Email, user.Email!),
    //        new(ClaimTypes.FullName, user.FullName),
    //        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    //    };

    //    claims.AddRange(
    //        roles.Select(r =>
    //            new Claim(System.Security.Claims.ClaimTypes.Role, r)
    //        )
    //    );

    //    var token = new JwtSecurityToken(
    //        issuer: jwtSettings["Issuer"],
    //        audience: jwtSettings["Audience"],
    //        claims: claims,
    //        expires: expiry,
    //        signingCredentials: creds
    //    );

    //    return new JwtSecurityTokenHandler().WriteToken(token);
    //}
    private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var jwtSettings = _config.GetSection("JwtSettings");

        var secret = jwtSettings["SecretKey"]
            ?? throw new Exception("Jwt SecretKey is missing");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiryMinutes = jwtSettings.GetValue<double>("ExpirationMinutes", 60);

        var expiry = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
    {
        new(ClaimTypes.UserId, user.Id),
        new(ClaimTypes.Email, user.Email!),
        new(ClaimTypes.FullName, user.FullName ?? ""),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        claims.AddRange(
            roles.Select(r => new Claim(System.Security.Claims.ClaimTypes.Role, r))
        );

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private static AuthResponseDto BuildAuthResponse(
        ApplicationUser user,
        IList<string> roles,
        string token)
    {
        return new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Token = token,
            TokenExpiry = DateTime.UtcNow.AddMinutes(60),
            Roles = roles
        };
    }

    private async Task EnsureRoleExistsAsync(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}