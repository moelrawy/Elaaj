using MediatR;
using Elaaj.Application.Interfaces;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using Restaurants.Domain.Constants;

namespace Elaaj.Application.Features.PostReplies.Commands.CreatePostReply
{
    public class CreatePostReplyCommandHandler : IRequestHandler<CreatePostReplyCommand, int>
    {
        private readonly INotificationService _notificationService;
        private readonly IGenericRepository<PostReply> _replyRepository;
        private readonly IGenericRepository<Post> _postRepository;
        private readonly IGenericRepository<Pharmacy> _pharmacyRepository; // ✅ أضف ده
        private readonly IUserContext _userContext;

        public CreatePostReplyCommandHandler(
            INotificationService notificationService,
            IGenericRepository<PostReply> replyRepository,
            IGenericRepository<Post> postRepository,
            IGenericRepository<Pharmacy> pharmacyRepository, // ✅ أضف ده
            IUserContext userContext)
        {
            _notificationService = notificationService;
            _replyRepository = replyRepository;
            _postRepository = postRepository;
            _pharmacyRepository = pharmacyRepository; // ✅ أضف ده
            _userContext = userContext;
        }

        public async Task<int> Handle(CreatePostReplyCommand request, CancellationToken cancellationToken)
        {
            // ✅ 1. تحقق من اليوزر
            var currentUser = _userContext.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("يجب تسجيل الدخول أولاً");

            // ✅ 2. بس الصيدلي يقدر يرد
            bool canReply = currentUser.IsInRole(UserRoles.PharmacyOwner)
                         || currentUser.IsInRole(UserRoles.PharmacyAdmin);

            if (!canReply)
                throw new UnauthorizedAccessException("بس الصيدلي يقدر يرد على الاستشارات");

            // ✅ 3. تحقق إن الـ Post موجود
            var post = await _postRepository.GetByIdAsync(request.PostId);
            if (post == null)
                throw new KeyNotFoundException("الاستشارة مش موجودة");

            // ✅ 4. تحقق إن الصيدلية بتاعته هو
            var pharmacy = await _pharmacyRepository.GetByIdAsync(request.PharmacyId);
            if (pharmacy == null)
                throw new KeyNotFoundException("الصيدلية مش موجودة");

            if (pharmacy.OwnerId != currentUser.Id)
                throw new UnauthorizedAccessException("مش صيدليتك!");

            // ✅ 5. احفظ الرد في الـ Database
            var reply = new PostReply
            {
                UserId = currentUser.Id,
                PostId = request.PostId,
                PharmacyId = request.PharmacyId, // ✅ أضف ده
                Message = request.ReplyContent,
                CreatedAt = DateTime.UtcNow
            };

            await _replyRepository.AddAsync(reply);
            await _replyRepository.SaveChangesAsync();

            // ✅ 6. ابعت Notification
            await _notificationService.SendReplyNotification(
                request.ReceiverId,
                "تم الرد على استشارتك"
            );

            return reply.Id;
        }
    }
}