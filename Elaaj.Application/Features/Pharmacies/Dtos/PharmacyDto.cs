using System.Text.Json.Serialization;

namespace Elaaj.Application.Features.Pharmacies.Dtos;

public class PharmacyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty; 
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string WorkingHours { get; set; } = string.Empty;
    public bool HasDelivery { get; set; }
    public string ContactNumber { get; set; } = string.Empty;
    public double? Distance { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public bool IsOpen { get; set; } = true;
    //public double Rating { get; set; } = 5.0;
    public DateTime? CreatedAt { get; set; }
    [JsonIgnore]
    public string Role { get; set; } = string.Empty;
}
