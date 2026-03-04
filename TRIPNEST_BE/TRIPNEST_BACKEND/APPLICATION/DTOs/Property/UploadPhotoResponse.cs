using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Property
{
    public class UploadPhotoResponse
    {
        public long PhotoId { get; set; }
        public string Url { get; set; } = null!;
    }
}
