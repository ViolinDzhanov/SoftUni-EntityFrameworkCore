using Invoices.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType(nameof(Address))]
    public class ImportAddressDto
    {
        [Required]
        [MinLength(10)]
        [MaxLength(20)]
        [XmlElement(nameof(StreetName))]
        public string StreetName { get; set; } = null!;

        [Required]
        [XmlElement(nameof(StreetNumber))]
        public int StreetNumber { get; set; }

        [Required]
        [XmlElement(nameof(PostCode))]
        public string PostCode { get; set; } = null!;

        [Required]
        [MinLength(5)]
        [MaxLength(15)]
        [XmlElement(nameof(City))]
        public string City { get; set; } = null!;

        [Required]
        [MinLength(5)]
        [MaxLength(15)]
        [XmlElement(nameof(Country))]
        public string Country { get; set; } = null!;

    }
}
