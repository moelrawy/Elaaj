using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Domain.Entities
{
    public class PharmacyMessage
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;


        public int SenderPharmacyId { get; set; }
        public Pharmacy SenderPharmacy { get; set; } = null!;

        public int? ReceiverPharmacyId { get; set; }
        public Pharmacy? ReceiverPharmacy { get; set; }
    }
}
