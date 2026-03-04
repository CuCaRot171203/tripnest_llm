using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Reviews
{
    public class UpdateReviewRequestDto
    {
        public sbyte Rating { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public List<string>? Photos { get; set; }
    }
}
