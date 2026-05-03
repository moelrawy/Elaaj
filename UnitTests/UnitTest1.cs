using Moq;
using Xunit;
using Elaaj.Application.Features.PostReplies.Commands.CreatePostReply;
using Elaaj.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;
namespace UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Handle_ShouldSendNotification_WhenCommandIsValid()
        {
            var mocknotificationService = new Mock<INotificationService>();
            var handler = new CreatePostReplyCommandHandler(mocknotificationService.Object);
            var command = new CreatePostReplyCommand
            {
                ReceiverId = "user123",
                ReplyContent = "تم الرد",
                PostId=1
            };
            await handler.Handle(command,CancellationToken.None);
            mocknotificationService.Verify(x=>x.SendReplyNotification("user123",It.IsAny<string>()),Times.Once);

        }
       
    }
}
