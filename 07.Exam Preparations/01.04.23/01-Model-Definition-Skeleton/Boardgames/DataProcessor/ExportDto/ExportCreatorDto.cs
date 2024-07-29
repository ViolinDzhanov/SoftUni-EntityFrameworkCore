using Boardgames.Data.Models;
using Boardgames.DataProcessor.ImportDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ExportDto
{
    [XmlType("Creator")]
    public class ExportCreatorDto
    {
        [XmlElement("CreatorName")]
        public string CreatorName { get; set; } = null!;

        [XmlAttribute("BoardgamesCount")]
        public int BoardgamesCount { get; set; }

        [XmlArray("Boardgames")]
        public ExportGameDto[] Boardgames { get; set; } = null!;
    }
}
