using Elaaj.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Notifications.Commands.MarkAsRead
{
    public record MarkAllNotificationsAsReadCommand(string UserId) : IRequest<Unit>;

    public class MarkAllNotificationsAsReadCommandHandler:IRequestHandler<MarkAllNotificationsAsReadCommand,Unit>
    {
        private readonly IApplicationDbContext _context;

        public MarkAllNotificationsAsReadCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(MarkAllNotificationsAsReadCommand request,CancellationToken cancellationToken)
        {
            var unreadNotifications=await _context.Notifications
                .Where(n=>n.UserId==request.UserId && !n.IsRead)
                .ToListAsync(cancellationToken);

            foreach(var notification in unreadNotifications)
            {
                notification.MarkAsRead();
            }
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

    }
}
