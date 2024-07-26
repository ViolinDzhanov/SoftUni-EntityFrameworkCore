using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cadastre.Data.Models
{
    public class PropertyCitizen
    {
        [ForeignKey(nameof(Property))]
        public int PropertyId { get; set; }

        [Required]
        public Property Property { get; set; } = null!;

        [ForeignKey(nameof(Citizen))]
        public int CitizenId { get; set; }

        public Citizen Citizen { get; set; }
    }
}
