using Elaaj.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Notifications.Queries.GetNotifications
{
    public record GetUnreadNotificationsCountQuery(string UserId) : IRequest<int>;

    public class GetUnreadNotificationsCountQueryHandler : IRequestHandler<GetUnreadNotificationsCountQuery, int>
    {
        private readonly IApplicationDbContext _context;

        public GetUnreadNotificationsCountQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(GetUnreadNotificationsCountQuery request, CancellationToken cancellationToken)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == request.UserId && !n.IsRead, cancellationToken);
        }
    }

}
