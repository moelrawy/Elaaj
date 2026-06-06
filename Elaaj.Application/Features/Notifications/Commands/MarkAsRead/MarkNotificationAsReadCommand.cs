using Elaaj.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Notifications.Commands.MarkAsRead
{
    public record MarkNotificationAsReadCommand(Guid Id) : IRequest<Unit>;

    public class MarkNotificationAsReadCommandHandler:IRequestHandler<MarkNotificationAsReadCommand,Unit>
    {
        private readonly IApplicationDbContext _context;
        public MarkNotificationAsReadCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Unit> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken);
            if (notification == null)
                throw new KeyNotFoundException("الإشعار مش موجود");

            notification.MarkAsRead();

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
