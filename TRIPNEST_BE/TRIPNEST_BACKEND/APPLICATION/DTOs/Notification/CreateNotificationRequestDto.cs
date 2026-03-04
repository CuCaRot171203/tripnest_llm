using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Notification
{
    public class CreateNotificationRequestDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Type { get; set; } = null!;

        [Required]
        public string Payload { get; set; } = null!;
        public bool SendNow { get; set; } = true;
    }
}
