using Medicines.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Pharmacy")]
    public class ImportPharmacyDto
    {

        [Required]
        [XmlElement("Name")]
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [Required]
        [XmlElement("PhoneNumber")]
        [RegularExpression(@"^\(\d{3}\) \d{3}-\d{4}$")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [XmlAttribute("non-stop")]
        [RegularExpression(@"^(true|false)$")]
        public string IsNonStop { get; set; } = null!;

        [XmlArray("Medicines")]
        [XmlArrayItem("Medicine")]
        public ImportMedicineDto[] Medicines { get; set; }
    }
}
