using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Domain.Enums;

public enum PrescriptionStatus
{
    Active = 0,        
    OffersReceived = 1, 
    Accepted = 2,       
    Completed = 3,      
    Rejected = 4,      
    Cancelled = 5      
}
