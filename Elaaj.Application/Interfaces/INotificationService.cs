using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendReplyNotification(string Id,string message);
        
    }
}

