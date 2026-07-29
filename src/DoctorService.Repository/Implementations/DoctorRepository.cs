using Dapper;
using DoctorService.Data;
using DoctorService.InternalModels.Entities;
using DoctorService.Utils.Common;
using System.Data;

namespace DoctorService.Repository;

public class DoctorRepository : BaseRepository, IDoctorRepository
{
    public DoctorRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    public async Task<PagedResult<DoctorEntity>> GetDoctorsAsync(SearchQuery searchQuery)
    {
        return await ExecuteWithConnectionAsync(async connection =>
        {
            using var grid = await connection.QueryMultipleAsync(
                StoredProcedureNames.GetDoctorsPaged,
                new { searchQuery.PageNumber, searchQuery.PageSize, SearchTerm = searchQuery.SearchTerm },
                commandType: CommandType.StoredProcedure);

            var items = (await grid.ReadAsync<DoctorEntity>()).ToList();
            var total = await grid.ReadFirstAsync<int>();
            return new PagedResult<DoctorEntity>(items, total, searchQuery.PageNumber, searchQuery.PageSize);
        });
    }

    public Task<DoctorEntity?> GetDoctorByIdAsync(int id)
    {
        return QuerySingleOrDefaultAsync<DoctorEntity>(
            StoredProcedureNames.GetDoctorById,
            new { Id = id },
            commandType: CommandType.StoredProcedure);
    }

    public Task<DoctorEntity?> GetDoctorByDoctorIdAsync(string doctorId)
    {
        return QuerySingleOrDefaultAsync<DoctorEntity>(
            StoredProcedureNames.GetDoctorByDoctorId,
            new { DoctorId = doctorId },
            commandType: CommandType.StoredProcedure);
    }

    public Task<DoctorEntity?> GetDoctorByUserIdAsync(int userId)
    {
        return QuerySingleOrDefaultAsync<DoctorEntity>(
            StoredProcedureNames.GetDoctorByUserId,
            new { UserId = userId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<PagedResult<DoctorEntity>> GetDoctorsBySpecializationAsync(string specialization, int pageNumber, int pageSize)
    {
        return await ExecuteWithConnectionAsync(async connection =>
        {
            using var grid = await connection.QueryMultipleAsync(
                StoredProcedureNames.GetDoctorsBySpecializationPaged,
                new { Specialization = specialization, PageNumber = pageNumber, PageSize = pageSize },
                commandType: CommandType.StoredProcedure);

            var items = (await grid.ReadAsync<DoctorEntity>()).ToList();
            var total = await grid.ReadFirstAsync<int>();
            return new PagedResult<DoctorEntity>(items, total, pageNumber, pageSize);
        });
    }

    public async Task<IReadOnlyCollection<string>> GetSpecializationsAsync()
    {
        var items = await QueryAsync<string>(
            StoredProcedureNames.GetSpecializations,
            commandType: CommandType.StoredProcedure);
        return items.ToList();
    }

    public async Task<string> GenerateDoctorIdAsync()
    {
        return await ExecuteScalarAsync<string>(
            StoredProcedureNames.GenerateDoctorId,
            commandType: CommandType.StoredProcedure) ?? string.Empty;
    }

    public async Task<DoctorEntity> CreateDoctorAsync(DoctorEntity doctor)
    {
        var id = await ExecuteScalarAsync<int>(
            StoredProcedureNames.CreateDoctor,
            new
            {
                doctor.DoctorId,
                doctor.UserId,
                doctor.FirstName,
                doctor.LastName,
                doctor.Specialization,
                doctor.Email,
                doctor.Phone,
                YearsOfExperience = doctor.YearsOfExperience,
                IsActive = doctor.IsActive
            },
            commandType: CommandType.StoredProcedure);

        doctor.Id = id;
        if (string.IsNullOrWhiteSpace(doctor.DoctorId))
        {
            doctor.DoctorId = $"DOC{id:000}";
        }
        return doctor;
    }

    public async Task<DoctorEntity?> UpdateDoctorAsync(int id, DoctorEntity doctor)
    {
        var rowsAffected = await ExecuteAsync(
            StoredProcedureNames.UpdateDoctor,
            new
            {
                Id = id,
                doctor.UserId,
                doctor.FirstName,
                doctor.LastName,
                doctor.Specialization,
                doctor.Email,
                doctor.Phone,
                doctor.YearsOfExperience,
                doctor.IsActive
            },
            commandType: CommandType.StoredProcedure);

        if (rowsAffected <= 0)
        {
            return null;
        }

        return await GetDoctorByIdAsync(id);
    }

    public async Task<bool> DeleteDoctorAsync(int id)
    {
        var rowsAffected = await ExecuteAsync(
            StoredProcedureNames.DeleteDoctor,
            new { Id = id },
            commandType: CommandType.StoredProcedure);
        return rowsAffected > 0;
    }
}
