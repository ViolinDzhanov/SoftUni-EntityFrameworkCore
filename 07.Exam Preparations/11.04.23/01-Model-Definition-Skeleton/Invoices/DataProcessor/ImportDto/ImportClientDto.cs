﻿using Invoices.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType(nameof(Client))]
    public class ImportClientDto
    {
        [XmlElement(nameof(Name))]
        [Required]
        [MinLength(10)]
        [MaxLength(25)]
        public string Name { get; set; } = null!;

        [XmlElement(nameof(NumberVat))]
        [Required]
        [MinLength(10)]
        [MaxLength(15)]
        public string NumberVat { get; set; } = null!;

        [XmlArray(nameof(Addresses))]
        public ImportAddressDto[] Addresses { get; set; }
    }
}
