using Elaaj.Application.Features.PostReplies.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Posts.DTOs;

public record PostDto
{
    public int Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public DateTime CreatedAt { get; init; }

    public ICollection<PostReplyDto> Replies { get; set; } = new List<PostReplyDto>();
}