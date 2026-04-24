using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            f.PatientId == request.PatientId && f.PharmacyId == request.PharmacyId);

        if (favorite != null)
        {
            _repository.Delete(favorite);
        }
        else
        {
            await _repository.AddAsync(new UserFavorite
            {
                PatientId = request.PatientId,
                PharmacyId = request.PharmacyId
            });
        }
        await _repository.SaveChangesAsync();
        return true;
    }
}
