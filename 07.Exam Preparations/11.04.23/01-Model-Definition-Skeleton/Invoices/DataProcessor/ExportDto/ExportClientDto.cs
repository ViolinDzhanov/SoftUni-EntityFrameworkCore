using Invoices.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ExportDto
{
    [XmlType("Client")]
    public class ExportClientDto
    {
        [XmlElement("ClientName")]
        public string Name { get; set; }

        [XmlElement("VatNumber")]
        public string VatNumber { get; set; }

        [XmlAttribute("InvoicesCount")]
        public int InvoicesCount { get; set; }

        [XmlArray(nameof(Invoices))]
        public ExportInvoiceDto[] Invoices { get; set; } = null!;
    }
}
