using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Elaaj.Application.Features.Pharmacies.Commands.ToggleFavorite;

public class ToggleFavoriteCommandHandler : IRequestHandler<ToggleFavoriteCommand, bool>
{
    private readonly IGenericRepository<UserFavorite> _repository;
    private readonly IUserContext _userContext;

    public ToggleFavoriteCommandHandler(IGenericRepository<UserFavorite> repository, IUserContext userContext)
    {
        _repository = repository;
        _userContext = userContext;
    }

    public async Task<bool> Handle(ToggleFavoriteCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        if (currentUser == null)
            throw new UnauthorizedAccessException("يجب تسجيل الدخول لإضافة الصيدلية للمفضلة.");

        var favorite = await _repository.GetFirstOrDefaultAsync(f =>
            f.UserId == currentUser.Id && f.PharmacyId == request.PharmacyId);

        if (favorite != null)
        {
            _repository.Delete(favorite);
        }
        else
        {
            await _repository.AddAsync(new UserFavorite
            {
                UserId = currentUser.Id,
                PharmacyId = request.PharmacyId
            });
        }

        await _repository.SaveChangesAsync();

        return true;
    }
}