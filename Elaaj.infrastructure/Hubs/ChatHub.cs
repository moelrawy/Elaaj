using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Elaaj.infrastructure.Hubs;

[Authorize]
public class ChatHub : Hub
{
    // دالة لربط اليوزر بالـ ConnectionId بتاعه أول ما يفتح الأبلكيشن
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            // بنضيف اليوزر لجروب باسم الـ ID بتاعه عشان نعرف نبعتله رسائل دايركت
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }
        await base.OnConnectedAsync();
    }
}
