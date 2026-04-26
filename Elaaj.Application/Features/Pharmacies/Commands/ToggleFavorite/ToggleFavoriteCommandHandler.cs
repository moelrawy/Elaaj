using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;

namespace Elaaj.Application.Features.Pharmacies.Commands.ToggleFavorite;

public class ToggleFavoriteCommandHandler : IRequestHandler<ToggleFavoriteCommand, bool>
{
    private readonly IGenericRepository<UserFavorite> _repository;

    public ToggleFavoriteCommandHandler(IGenericRepository<UserFavorite> repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(ToggleFavoriteCommand request, CancellationToken cancellationToken)
    {
        
        var favorite = await _repository.GetFirstOrDefaultAsync(f =>
            f.UserId == request.UserId && f.PharmacyId == request.PharmacyId);

        if (favorite != null)
        {
            _repository.Delete(favorite);
        }
        else
        {
            await _repository.AddAsync(new UserFavorite
            {
               
                UserId = request.UserId,
                PharmacyId = request.PharmacyId
            });
        }

        await _repository.SaveChangesAsync();
        return true;
    }
}