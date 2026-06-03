using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.BLL
{
    public class AuthResponseDto
    {
        

        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
        public DateTime TokenExpiry { get; set; }
    }
}
