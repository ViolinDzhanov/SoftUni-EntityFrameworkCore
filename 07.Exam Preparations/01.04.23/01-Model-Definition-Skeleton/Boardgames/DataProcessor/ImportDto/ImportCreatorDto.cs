using Boardgames.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType(nameof(Creator))]
    public class ImportCreatorDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(7)]
        [XmlElement(nameof(FirstName))]
        public string FirstName { get; set; } = null!;

        [Required]
        [MinLength(2)]
        [MaxLength(7)]
        [XmlElement(nameof(LastName))]
        public string LastName { get; set; } = null!;

        [XmlArray(nameof(Boardgames))]
        public ImportBoardGameDto[] Boardgame { get; set; }
    }
}
