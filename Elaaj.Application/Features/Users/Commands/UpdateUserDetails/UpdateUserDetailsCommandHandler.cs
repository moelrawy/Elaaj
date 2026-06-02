using Elaaj.Application.Interfaces;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Elaaj.Application.Features.Users.Commands.UpdateUserDetails;

public class UpdateUserDetailsCommandHandler(
    ILogger<UpdateUserDetailsCommandHandler> logger,
    IUserContext userContext,
    IUserStore<User> userStore,
    IFileService fileService) : IRequestHandler<UpdateUserDetailsCommand>
{
    public async Task Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken)
    {
        var userContextData = userContext.GetCurrentUser();
        if (userContextData == null) throw new UnauthorizedAccessException();

        logger.LogInformation("Updating user: {UserId}", userContextData.Id);

        var dbUser = await userStore.FindByIdAsync(userContextData.Id, cancellationToken);
        if (dbUser == null)
        {
            throw new NotFoundException(nameof(User), userContextData.Id);
        }

        if (request.ImageFile != null && request.ImageFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(dbUser.imageUrl))
            {
                fileService.DeleteFile(dbUser.imageUrl);
            }
            dbUser.imageUrl = await fileService.UploadFileAsync(request.ImageFile, "users");
        }

        if (request.FullName != null) dbUser.FullName = request.FullName;
        if (request.DateOfBirth.HasValue) dbUser.DateOfBirth = request.DateOfBirth;
        if (request.Address != null) dbUser.Address = request.Address;
        if (request.Latitude.HasValue) dbUser.Latitude = request.Latitude.Value;
        if (request.Longitude.HasValue) dbUser.Longitude = request.Longitude.Value;

        await userStore.UpdateAsync(dbUser, cancellationToken);
    }
}
