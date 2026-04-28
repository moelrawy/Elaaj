using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Elaaj.Application.Features.Users.Commands.UpdateUserDetails;

public class UpdateUserDetailsCommandHandler(ILogger<UpdateUserDetailsCommandHandler> logger,
   IUserContext userContext,
   IUserStore<User> userStore) : IRequestHandler<UpdateUserDetailsCommand>
{
    public async Task Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken)
    {
        var userContextData = userContext.GetCurrentUser();

        logger.LogInformation("Updating user: {UserId}, with {@Request}", userContextData!.Id, request);

        var dbUser = await userStore.FindByIdAsync(userContextData.Id, cancellationToken);

        if (dbUser == null)
        {
            throw new NotFoundException(nameof(User), userContextData.Id);
        }

        dbUser.DateOfBirth = request.DateOfBirth;

        await userStore.UpdateAsync(dbUser, cancellationToken);
    }
}
