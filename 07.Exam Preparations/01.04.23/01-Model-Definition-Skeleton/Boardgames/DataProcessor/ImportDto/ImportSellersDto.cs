using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Boardgames.DataProcessor.ImportDto
{
    public class ImportSellersDto
    {
        [Required]
        [MinLength(5)]
        [StringLength(20)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(2)]
        [StringLength(30)]
        public string Address { get; set; } = null!;

        [Required]
        public string Country { get; set; } = null!;

        [Required]
        [RegularExpression(@"^www\.[A-Za-z0-9-]+\.com$")]
        public string Website { get; set; } = null!;

        [JsonProperty(nameof(Boardgames))]
        public int[] BoardGamesId { get; set; }
    }
}
