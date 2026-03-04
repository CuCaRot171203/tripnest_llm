using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Upload
{
    public class GetSignedUrlRequestDto
    {
        public string Path { get; set; } = null!;         
        public int ExpirySeconds { get; set; } = 300;     
        public string? ContentType { get; set; }          
    }
}
