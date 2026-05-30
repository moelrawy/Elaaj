using Elaaj.Application.Common.Models;
using Elaaj.Application.Features.Users.UserDtos;
using MediatR;

namespace Elaaj.Application.Features.Users.Queries.SearchUsers;

public class SearchUsersQuery : IRequest<PagedResult<SearchUserDto>>
{
    public string? Search { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}