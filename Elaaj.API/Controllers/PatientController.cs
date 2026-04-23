using Elaaj.Application.Features.Patients.Commands.CreatePatient;
using Elaaj.Application.Features.Patients.Commands.DeletePatient;
using Elaaj.Application.Features.Patients.Commands.UpdatePatient;
using Elaaj.Application.Features.Patients.Dtos;
using Elaaj.Application.Features.Patients.Queries.GetAllPatients;
using Elaaj.Application.Features.Patients.Queries.GetPatientById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Elaaj.API.Controllers
{
    [ApiController]
    [Route("api/Patients")]
    public class PatientController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll([FromQuery] GetAllPatientsQuery query)
        {
            var patients = await mediator.Send(query);
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetById([FromRoute] int id)
        {
            var patient = await mediator.Send(new GetPatientByIdQuery(id));
            return Ok(patient);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreatePatientCommand command)
        {
            var id = await mediator.Send(command);
            return Ok(id);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePatient([FromRoute] int id,UpdatePatientCommand command)
        {
            command.Id = id;
            await mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await mediator.Send(new DeletePatientCommand(id));

            return NoContent();
        }

    }
}
