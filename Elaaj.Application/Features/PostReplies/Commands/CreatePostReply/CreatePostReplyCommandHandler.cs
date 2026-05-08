using Elaaj.Application.Interfaces;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using Restaurants.Domain.Constants;
namespace Elaaj.Application.Features.PostReplies.Commands.CreatePostReply
{
    public class CreatePostReplyCommandHandler : IRequestHandler<CreatePostReplyCommand,int>
    {
        private readonly INotificationService _notificationService;
        private readonly IGenericRepository<PostReply> _replyRepository;
        private readonly IGenericRepository<Post> _postRepository;
        private readonly IGenericRepository<Pharmacy> _pharmacyRepository;
        private readonly IUserContext _userContext;
        public CreatePostReplyCommandHandler(INotificationService notificationService, IGenericRepository<PostReply> replyRepository, IGenericRepository<Post> postRepository, IGenericRepository<Pharmacy> pharmacyRepository, IUserContext userContext)
        {
            _notificationService = notificationService;
            _replyRepository = replyRepository;
            _postRepository = postRepository;
            _pharmacyRepository = pharmacyRepository;
            _userContext = userContext;
        }
        public async Task<int> Handle(CreatePostReplyCommand request,CancellationToken cancellationToken)
        {
            var currentUser = _userContext.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");

            bool canReply = currentUser.IsInRole(UserRoles.PharmacyOwner)
                         || currentUser.IsInRole(UserRoles.PharmacyAdmin);

            if (!canReply)
                throw new UnauthorizedAccessException("بس الصيدلي يقدر يرد على الاستشارات");

            var post = await _postRepository.GetByIdAsync(request.PostId);
            if (post == null)
                throw new KeyNotFoundException("الاستشارة مش موجودة");

            var pharmacy = await _pharmacyRepository.GetByIdAsync(request.PharmacyId);
            if (pharmacy == null)
                throw new KeyNotFoundException("الصيدلية مش موجودة");

            if (pharmacy.OwnerId != currentUser.Id)
                throw new UnauthorizedAccessException("مش صيدليتك!");

            var reply = new PostReply
            {
                UserId = currentUser.Id,
                PostId = request.PostId,
                PharmacyId = request.PharmacyId,
                Message = request.ReplyContent,
                CreatedAt = DateTime.UtcNow
            };

            await _replyRepository.AddAsync(reply);
            await _replyRepository.SaveChangesAsync();

            await _notificationService.SendReplyNotification(request.ReceiverId, "تم الرد علي استشارتك");
            return reply.Id;
        }

    }
}
