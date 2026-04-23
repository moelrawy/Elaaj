namespace Elaaj.Application.Features.Patients.Dtos;

public class PatientDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}
