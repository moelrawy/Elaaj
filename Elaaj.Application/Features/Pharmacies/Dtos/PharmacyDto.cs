namespace Elaaj.Application.Features.Pharmacies.Dtos;

public class PharmacyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string imageUrl { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string WorkingHours { get; set; } = string.Empty;
    public bool HasDelivery { get; set; }
    public string ContactNumber { get; set; } = string.Empty;

}
