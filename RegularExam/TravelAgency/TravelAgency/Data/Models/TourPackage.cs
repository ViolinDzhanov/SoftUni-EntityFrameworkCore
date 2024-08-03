﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgency.Data.Models
{
    public class TourPackage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string PackageName { get; set; } = null!;
        public string? Description  { get; set; }

        [Required]
        public decimal Price { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<TourPackageGuide> TourPackagesGuides { get; set; } = new List<TourPackageGuide>();
    }
}
