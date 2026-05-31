using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Interfaces.Services
{
    public static class OtpStore
    {
        // نخزن: الكود هو المفتاح، والـ Value هي (UserId + Expiry)
        private static readonly Dictionary<string, (string UserId, DateTime Expiry)> _otps = new();

        public static void SaveOtp(string userId, string otp, int expirationMinutes)
        {
            _otps[otp] = (userId, DateTime.UtcNow.AddMinutes(expirationMinutes));
        }

        // الدالة دي هترجع الـ UserId لو الكود صح، أو null لو غلط
        public static string? GetUserIdByOtp(string otp)
        {
            if (_otps.TryGetValue(otp, out var entry) && entry.Expiry > DateTime.UtcNow)
            {
                return entry.UserId;
            }
            return null;
        }

        public static void RemoveOtp(string otp) => _otps.Remove(otp);
    }
}
