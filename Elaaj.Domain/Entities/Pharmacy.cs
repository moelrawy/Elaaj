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
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string imageUrl { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string WorkingHours { get; set; } = string.Empty;
        public bool HasDelivery { get; set; }
        public string ContactNumber { get; set; } = string.Empty;
        public string OwnerId { get; set; } = default!;
        public virtual User Owner { get; set; } = default!;


        public ICollection<PostReply> Replies { get; set; } = new List<PostReply>();
        public ICollection<PharmacyMessage> SentMessages { get; set; } = new List<PharmacyMessage>();
        public ICollection<PharmacyMessage> ReceivedMessages { get; set; } = new List<PharmacyMessage>();
        public virtual ICollection<PharmacyAdmin> Admins { get; set; } = new List<PharmacyAdmin>();
    }
}
