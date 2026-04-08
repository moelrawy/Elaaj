using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Domain.Entities;

public class Post
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Patient Patient { get; set; }
    public ICollection<PostReply> postReplies { get; set; } = new List<PostReply>();

}
