namespace DoctorService.Repository;

public static class StoredProcedureNames
{
    public const string GetDoctorsPaged = "dbo.GetDoctorsPaged";
    public const string GetDoctorById = "dbo.GetDoctorById";
    public const string GetDoctorByDoctorId = "dbo.GetDoctorByDoctorId";
    public const string GetDoctorByUserId = "dbo.GetDoctorByUserId";
    public const string GetDoctorsBySpecializationPaged = "dbo.GetDoctorsBySpecializationPaged";
    public const string GetSpecializations = "dbo.GetSpecializations";
    public const string GenerateDoctorId = "dbo.GenerateDoctorId";
    public const string CreateDoctor = "dbo.CreateDoctor";
    public const string UpdateDoctor = "dbo.UpdateDoctor";
    public const string DeleteDoctor = "dbo.DeleteDoctor";
}
