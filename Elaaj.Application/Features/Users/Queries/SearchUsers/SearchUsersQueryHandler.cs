using Elaaj.Application.Common.Models;
using Elaaj.Application.Features.Users.UserDtos;
using Elaaj.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Elaaj.Application.Features.Users.Queries.SearchUsers;

public class SearchUsersQueryHandler(UserManager<User> userManager) 
    : IRequestHandler<SearchUsersQuery, PagedResult<SearchUserDto>>
{
    public async Task<PagedResult<SearchUserDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var query = userManager.Users.AsNoTracking();

        // Apply case-insensitive search by FullName and Email
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            query = query.Where(u => 
                (u.FullName != null && u.FullName.ToLower().Contains(searchTerm)) || 
                (u.Email != null && u.Email.ToLower().Contains(searchTerm)));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination and projection mapping
        var users = await query
            .OrderBy(u => u.FullName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new SearchUserDto
            {
                Id = u.Id,
                FullName = u.FullName!,
                Email = u.Email!
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<SearchUserDto>
        {
            Items = users,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}