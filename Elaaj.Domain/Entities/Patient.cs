using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Domain.Entities
{
    public class Patient
    {
        public int Id { get; set; } 
        public string FullName { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public ICollection<Post> posts { get; set; } = new List<Post>();
    }
}

