using ECommerce.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.BLL
{
    public interface IAuthService
    {
        Task<GeneralResult<AuthResponseDto>> RegisterAsync(RegisterRequestDto dto);
        Task<GeneralResult<AuthResponseDto>> LoginAsync(LoginRequestDto dto);
    }
}
