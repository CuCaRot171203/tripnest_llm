using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Reviews
{
    public class ReviewListItemDto
    {
        public Guid ReviewId { get; set; }
        public Guid UserId { get; set; }
        public string? UserDisplayName { get; set; }
        public sbyte Rating { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<string>? Photos { get; set; }
    }
}
