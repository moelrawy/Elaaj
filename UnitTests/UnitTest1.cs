using Elaaj.Application.Features.PostReplies.Commands.CreatePostReply;
using Elaaj.Application.Features.Users;
using Elaaj.Application.Interfaces;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using Moq;
using Restaurants.Domain.Constants;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
namespace UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Handle_ShouldSendNotification_WhenCommandIsValid()
        {
            // 1. Arrange (تجهيز الـ Mocks)
            var mockNotificationService = new Mock<INotificationService>();
            var mockReplyRepo = new Mock<IGenericRepository<PostReply>>();
            var mockPostRepo = new Mock<IGenericRepository<Post>>();
            var mockPharmacyRepo = new Mock<IGenericRepository<Pharmacy>>();
            var mockUserContext = new Mock<IUserContext>();

            // 2. تجهيز الـ Current User (مهم عشان الـ Authorization في الـ Handler)
            var currentUser = new CurrentUser(
              "user123",              // Id
              "admin@elaaj.com",      // Email
              new[] { UserRoles.PharmacyOwner }, // Roles
                null,                   // DateOfBirth (خسارة إنك لسه معندكش تاريخ ميلاد هههه)
                null,                   // ProfileImageUrl
                null,                   // Latitude
                null                    // Longitude
);
            mockUserContext.Setup(x => x.GetCurrentUser()).Returns(currentUser);

            // 3. تجهيز الـ Handler (تمرير الـ 5 حاجات)
            var handler = new CreatePostReplyCommandHandler(
                mockNotificationService.Object,
                mockReplyRepo.Object,
                mockPostRepo.Object,
                mockPharmacyRepo.Object,
                mockUserContext.Object
            );

            var command = new CreatePostReplyCommand
            {
                ReceiverId = "receiver123",
                ReplyContent = "تم الرد",
                PostId = 1,
                PharmacyId = Guid.NewGuid() // لازم تبعت الـ PharmacyId كمان
            };

            // 4. Act
            await handler.Handle(command, CancellationToken.None);

            // 5. Assert
            mockNotificationService.Verify(x => x.SendReplyNotification("receiver123", It.IsAny<string>()), Times.Once);
        }

    }
}
