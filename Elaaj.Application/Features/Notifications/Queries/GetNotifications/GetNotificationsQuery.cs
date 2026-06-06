using Elaaj.Application.DTOs;
using Elaaj.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Elaaj.Application.Features.Notifications.Queries.GetNotifications
{
    public record GetNotificationsQuery(string UserId,int PageNumber=1,int PageSize=10,bool?IsRead=null):IRequest<PagedList<NotificationDto>>;
    
    public class GetNotificationsQueryHandler :IRequestHandler<GetNotificationsQuery, PagedList<NotificationDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetNotificationsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PagedList<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken) 
        {
            var query=_context.Notifications
                .Where(n=>n.UserId==request.UserId)
                .OrderByDescending(n=>n.CreatedAt)
                .AsNoTracking();

            if (request.IsRead.HasValue) 
            {
                query = query.Where(n => n.IsRead == request.IsRead.Value);
            }

            var totalCount=await query.CountAsync(cancellationToken);

            var items =await query
                .Skip((request.PageNumber- 1)* request.PageSize)
                .Take(request.PageSize)
                .Select(n=> new NotificationDto(n.Id,n.UserId,n.Title,n.Message,n.Type,n.IsRead,n.CreatedAt,n.ReadAt,n.RelatedEntityId,n.RelatedEntityType))
                .ToListAsync(cancellationToken);

            return new PagedList<NotificationDto>(items,totalCount,request.PageNumber,request.PageSize);
        }
    }

}
