using DoctorService.InternalModels.Entities;

namespace DoctorService.InternalModels.DTOs;

public class CreateDoctorDto
{
    public string DoctorId { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
}

public class UpdateDoctorDto : CreateDoctorDto
{
    public bool IsActive { get; set; } = true;
}

public class DoctorDto
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

    public static DoctorDto FromEntity(DoctorEntity entity) => new()
    {
        Id = entity.Id,
        DoctorId = entity.DoctorId,
        UserId = entity.UserId,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        Specialization = entity.Specialization,
        Email = entity.Email,
        Phone = entity.Phone,
        YearsOfExperience = entity.YearsOfExperience,
        IsActive = entity.IsActive
    };
}


