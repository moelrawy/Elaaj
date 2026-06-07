using AutoMapper;
using Elaaj.Application.Features.Prescriptions.DTOs;
using Elaaj.Application.Users;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Prescriptions.Queries.GetPrescriptionById;

public class GetPrescriptionByIdQueryHandler : IRequestHandler<GetPrescriptionByIdQuery, MyPrescriptionDto>
{
    private readonly IGenericRepository<Prescription> _prescriptionRepository;
    private readonly IGenericRepository<Pharmacy> _pharmacyRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetPrescriptionByIdQueryHandler(
        IGenericRepository<Prescription> prescriptionRepository,
        IGenericRepository<Pharmacy> pharmacyRepository,
        IMapper mapper,
        IUserContext userContext)
    {
        _prescriptionRepository = prescriptionRepository;
        _pharmacyRepository = pharmacyRepository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<MyPrescriptionDto> Handle(GetPrescriptionByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedAccessException("ÌÃ»  ”ÃÌ· «·œŒÊ· √Ê·«");

        var prescriptions = await _prescriptionRepository.GetWhereAsync(p => p.Id == request.Id, p => p.Replies);
        var prescription = prescriptions.FirstOrDefault();

        if (prescription == null)
            throw new Exception("«·—Ê‘ … €Ì— „ÊÃÊœ….");

        if (prescription.UserId != currentUser.Id)
            throw new UnauthorizedAccessException("€Ì— „’—Õ ·ﬂ »⁄—÷ Â–Â «·—Ê‘ ….");

        var dto = _mapper.Map<MyPrescriptionDto>(prescription);

        if (dto.Replies != null && dto.Replies.Any())
        {
            var pharmacyIds = dto.Replies.Select(r => r.PharmacyId).Distinct().ToList();
            var pharmacies = await _pharmacyRepository.GetWhereAsync(p => pharmacyIds.Contains(p.Id));

            foreach (var reply in dto.Replies)
            {
                var pharmacy = pharmacies.FirstOrDefault(p => p.Id == reply.PharmacyId);
                if (pharmacy != null)
                {
                    reply.PharmacyName = pharmacy.Name;
                    reply.PharmacyImageUrl = pharmacy.ImageUrl ?? string.Empty;
                }
            }
        }

        return dto;
    }
}