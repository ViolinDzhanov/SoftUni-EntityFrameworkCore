using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgency.DataProcessor.ImportDtos
{
    public class ImportBookingsDto
    {
        [Required]
        public string BookingDate { get; set; } = null!;

        [Required]
        public string CustomerName { get; set; } = null!;

        [Required]
        public string TourPackageName { get; set; } = null!;
    }
}
