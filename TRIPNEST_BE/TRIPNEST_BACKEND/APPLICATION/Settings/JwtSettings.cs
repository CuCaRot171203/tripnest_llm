using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Settings
{
    public class JwtSettings
    {
        [Required]
        public string Key { get; set; } = null!;
        
        [Required]
        public string Issuer { get; set; } = null!;

        [Required]
        public string Audience { get; set; } = null!;

        [Range(1, 525600)]
        public int AccessTokenExpireMinutes { get; set; }

        [Range(1, 3650)]
        public int RefreshTokenExpireDays { get; set; }
    }
}
