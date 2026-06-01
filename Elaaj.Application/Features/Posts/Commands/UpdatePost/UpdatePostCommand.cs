using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Elaaj.Application.Features.Posts.Commands.UpdatePost;

public class UpdatePostCommand : IRequest<bool>
{
    public int Id { get; set; }

    [Required(ErrorMessage = "ãÍÊæì ÇáÇÓÊİÓÇÑ ãØáæÈ.")]
    [MaxLength(1000, ErrorMessage = "ÇáÇÓÊİÓÇÑ Øæíá ÌÏÇğ.")]
    public string Content { get; set; } = string.Empty;

    public IFormFile? File { get; set; }
}