using DoctorService.InternalModels.Entities;

namespace DoctorService.Services;

public static class DoctorStore
{
    public static int DoctorSeed = 1;
    public static readonly List<DoctorEntity> Doctors = new()
    {
        new DoctorEntity
        {
            Id = 1,
            DoctorId = "DOC001",
            UserId = 1001,
            FirstName = "Kiran",
            LastName = "Rao",
            Specialization = "Cardiology",
            Email = "kiran.rao@hm.local",
            Phone = "9000001001",
            YearsOfExperience = 8,
            IsActive = true
        }
    };
}


