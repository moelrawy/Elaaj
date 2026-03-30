using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Domain.Entities
{
    public class Pharmacy
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string imageUrl { get; set; } = string.Empty;
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int WorkingHours { get; set; }
    }
}
