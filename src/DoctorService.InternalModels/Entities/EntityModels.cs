namespace DoctorService.InternalModels.Entities;

public class DoctorEntity
{
    public int Id { get; set; }
    public string DoctorId { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public bool IsActive { get; set; }
}


