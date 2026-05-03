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


        public Guid SenderPharmacyId { get; set; }
        public virtual Pharmacy SenderPharmacy { get; set; } = null!;

        public Guid? ReceiverPharmacyId { get; set; }
        public virtual Pharmacy? ReceiverPharmacy { get; set; }
    }
}
