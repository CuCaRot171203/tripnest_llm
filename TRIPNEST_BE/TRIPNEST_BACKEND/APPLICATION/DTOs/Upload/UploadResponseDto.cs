using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Upload
{
    public class UploadResponseDto
    {
        public long PhotoId { get; set; }
        public long PropertyId { get; set; }
        public string Url { get; set; } = null!;
        public int? Order { get; set; }
        public string? Meta { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
