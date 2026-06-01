using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Posts.DTOs
{
    public class UpdatePostDto
    {
        [Required(ErrorMessage = "محتوى الاستفسار مطلوب.")]
        [MaxLength(1000, ErrorMessage = "الاستفسار طويل جداً.")]
        public string Content { get; set; } = string.Empty;

        public IFormFile? File { get; set; }
    }
}
