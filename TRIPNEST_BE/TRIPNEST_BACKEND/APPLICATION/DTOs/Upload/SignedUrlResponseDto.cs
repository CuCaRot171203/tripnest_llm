using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Upload
{
    public class SignedUrlResponseDto
    {
        public string UploadUrl { get; set; } = null!;    
        public string Key { get; set; } = null!;          
        public DateTime ExpiresAt { get; set; }
    }
}
