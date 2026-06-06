using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Domain.Entities
{
    public class UserDevices
    {
        public Guid Id { get; private set; }
        public string UserId { get; private set; }
        public string DeviceToken { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private UserDevices() { }

        public UserDevices(string userId, string deviceToken)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            DeviceToken = deviceToken;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
