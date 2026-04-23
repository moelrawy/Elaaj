using AutoMapper;
using Elaaj.Application.Features.Patients.Commands.CreatePatient;
using Elaaj.Application.Features.Patients.Commands.UpdatePatient;
using Elaaj.Domain.Entities;
namespace Elaaj.Application.Features.Patients.Dtos
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {

            CreateMap<CreatePatientCommand, Patient>();

            CreateMap<UpdatePatientCommand, Patient>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Patient, PatientDto>();
        }
    }
}
