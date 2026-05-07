using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Domain.Enums;

public enum PrescriptionStatus
{
    Pending,    
    Accepted,    
    Preparing,  
    OutForDelivery, 
    Delivered,   
    Cancelled    
}
