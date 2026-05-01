using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Posts.Commands.CreatePost;

public class CreatePostCommand : IRequest<int>
{
    
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "محتوى الاستفسار مطلوب.")]
    [MaxLength(1000, ErrorMessage = "الاستفسار طويل جداً.")]
    public string Content { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }
}
