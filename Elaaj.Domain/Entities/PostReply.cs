using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Domain.Entities;

public class PostReply
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int PharmacyId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Post Post { get; set; } = null!;
    public Pharmacy Pharmacy { get; set; } = null!;
    public int PostId { get; set; }

}
