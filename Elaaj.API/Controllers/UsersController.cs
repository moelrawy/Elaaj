using Elaaj.Application.Common.Models;
using Elaaj.Application.Features.Users.Queries.SearchUsers;
using Elaaj.Application.Features.Users.UserDtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elaaj.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<SearchUserDto>>> SearchUsers([FromQuery] SearchUsersQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }
}