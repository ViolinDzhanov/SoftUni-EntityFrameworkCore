using Boardgames.Data.Models;
using Boardgames.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType(nameof(Boardgame))]
    public class ImportBoardGameDto
    {
        [Required]
        [MinLength(10)]
        [StringLength(20)]
        [XmlElement(nameof(Name))]
        public string Name { get; set; } = null!;

        [Required]
        [Range(1, 10)]
        [XmlElement(nameof(Rating))]
        public double Rating { get; set; }

        [Required]
        [Range(2018, 2023)]
        [XmlElement(nameof(YearPublished))]
        public int YearPublished { get; set; }

        [Required]
        [Range(0, 4)]
        [XmlElement(nameof(CategoryType))]
        public int CategoryType { get; set; }

        [Required]
        public string Mechanics { get; set; } = null!;
    }
}
