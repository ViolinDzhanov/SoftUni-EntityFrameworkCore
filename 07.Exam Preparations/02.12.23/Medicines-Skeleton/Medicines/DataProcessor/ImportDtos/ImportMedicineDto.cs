using Medicines.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Medicine")]
    public class ImportMedicineDto
    {
        [Required]
        [XmlAttribute("category")]
        [Range(0, 4)]
        public int Category { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(150)]
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [Range(0.01, 1000.00)]
        [XmlElement("Price")]
        public double Price { get; set; }

        [Required]
        [XmlElement("ProductionDate")]
        public string ProductionDate { get; set; } = null!;

        [Required]
        [XmlElement("ExpiryDate")]
        public string ExpiryDate { get; set; } = null!;

        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        [XmlElement("Producer")]
        public string Producer { get; set; } = null!;
    }
}
