using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Room.Request
{
    public class CreatePriceSlotRequest
    {
        [Required]
        public DateOnly ValidFrom { get; set; }

        [Required]
        public DateOnly ValidTo { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
