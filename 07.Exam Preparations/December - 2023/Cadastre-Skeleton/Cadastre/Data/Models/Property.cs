using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cadastre.Data.Models
{
    public class Property
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string PropertyIdentifier { get; set; } = null!;

        [Required]
        public int Area { get; set; }

        [MaxLength(500)]
        public string? Details { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; } = null!;

        [Required]
        public DateTime DateOfAcquisition  { get; set; }

        [ForeignKey(nameof(District))]
        public int DistrictId { get; set; }

        public District District { get; set; } = null!;

        public ICollection<PropertyCitizen> PropertiesCitizens { get; set; } = new List<PropertyCitizen>();
    }
}
