using DoctorService.InternalModels.DTOs;
using DoctorService.InternalModels.Entities;
using DoctorService.Repository;
using DoctorService.Utils.Common;
using System.Net.Http.Json;
using System.Text.Json;

namespace DoctorService.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;

    public DoctorService(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<ApiResponse<PagedResult<DoctorDto>>> GetDoctorsAsync(SearchQuery searchQuery)
    {
        var page = await _doctorRepository.GetDoctorsAsync(searchQuery);
        var dto = new PagedResult<DoctorDto>(page.Items.Select(DoctorDto.FromEntity).ToList(), page.TotalCount, page.PageNumber, page.PageSize);
        return ApiResponse<PagedResult<DoctorDto>>.Ok(dto);
    }

    public async Task<ApiResponse<DoctorDto>> GetDoctorByIdAsync(int id)
    {
        var doctor = await _doctorRepository.GetDoctorByIdAsync(id);
        return doctor is null ? ApiResponse<DoctorDto>.Fail("Doctor not found") : ApiResponse<DoctorDto>.Ok(DoctorDto.FromEntity(doctor));
    }

    public async Task<ApiResponse<DoctorDto>> GetDoctorByDoctorIdAsync(string doctorId)
    {
        var doctor = await _doctorRepository.GetDoctorByDoctorIdAsync(doctorId);
        return doctor is null ? ApiResponse<DoctorDto>.Fail("Doctor not found") : ApiResponse<DoctorDto>.Ok(DoctorDto.FromEntity(doctor));
    }

    public async Task<ApiResponse<DoctorDto>> GetDoctorByUserIdAsync(int userId)
    {
        var doctor = await _doctorRepository.GetDoctorByUserIdAsync(userId);
        return doctor is null ? ApiResponse<DoctorDto>.Fail("Doctor not found") : ApiResponse<DoctorDto>.Ok(DoctorDto.FromEntity(doctor));
    }

    public async Task<ApiResponse<PagedResult<DoctorDto>>> GetDoctorsBySpecializationAsync(string specialization, int pageNumber, int pageSize)
    {
        var page = await _doctorRepository.GetDoctorsBySpecializationAsync(specialization, pageNumber, pageSize);
        var dto = new PagedResult<DoctorDto>(page.Items.Select(DoctorDto.FromEntity).ToList(), page.TotalCount, page.PageNumber, page.PageSize);
        return ApiResponse<PagedResult<DoctorDto>>.Ok(dto);
    }

    public async Task<ApiResponse<IEnumerable<string>>> GetSpecializationsAsync()
    {
        return ApiResponse<IEnumerable<string>>.Ok(await _doctorRepository.GetSpecializationsAsync());
    }

    public async Task<ApiResponse<string>> GenerateDoctorIdAsync()
    {
        return ApiResponse<string>.Ok(await _doctorRepository.GenerateDoctorIdAsync());
    }

    public async Task<ApiResponse<DoctorDto>> CreateDoctorAsync(CreateDoctorDto createDoctorDto)
    {
        if (string.IsNullOrWhiteSpace(createDoctorDto.DoctorId))
        {
            createDoctorDto.DoctorId = await _doctorRepository.GenerateDoctorIdAsync();
        }

        var entity = new DoctorEntity
        {
            DoctorId = createDoctorDto.DoctorId,
            UserId = createDoctorDto.UserId,
            FirstName = createDoctorDto.FirstName,
            LastName = createDoctorDto.LastName,
            Specialization = createDoctorDto.Specialization,
            Email = createDoctorDto.Email,
            Phone = createDoctorDto.Phone,
            YearsOfExperience = createDoctorDto.YearsOfExperience,
            IsActive = true
        };

        var created = await _doctorRepository.CreateDoctorAsync(entity);
        return ApiResponse<DoctorDto>.Ok(DoctorDto.FromEntity(created), "Doctor created successfully");
    }

    public async Task<ApiResponse<DoctorDto>> CreateDoctorWithUserAsync(CreateDoctorWithUserDto createDoctorWithUserDto, string? authorizationHeader, CancellationToken cancellationToken = default)
    {
        var authServiceBaseUrl = GetAuthServiceBaseUrl();
        var userId = createDoctorWithUserDto.UserId ?? 0;
        var createdUserId = 0;

        if (userId <= 0)
        {
            if (string.IsNullOrWhiteSpace(createDoctorWithUserDto.Email))
            {
                return ApiResponse<DoctorDto>.Fail("Email is required to create a user account");
            }

            if (string.IsNullOrWhiteSpace(createDoctorWithUserDto.Password))
            {
                return ApiResponse<DoctorDto>.Fail("Password is required to create a user account");
            }

            var createdUser = await CreateUserAsync(createDoctorWithUserDto, authServiceBaseUrl, authorizationHeader, cancellationToken);
            if (!createdUser.Success || createdUser.UserId <= 0)
            {
                return ApiResponse<DoctorDto>.Fail(createdUser.ErrorMessage);
            }

            userId = createdUser.UserId;
            createdUserId = createdUser.UserId;
        }

        try
        {
            var createDoctorDto = new CreateDoctorDto
            {
                DoctorId = createDoctorWithUserDto.DoctorId,
                UserId = userId,
                FirstName = createDoctorWithUserDto.FirstName,
                LastName = createDoctorWithUserDto.LastName,
                Specialization = createDoctorWithUserDto.Specialization,
                Email = createDoctorWithUserDto.Email,
                Phone = createDoctorWithUserDto.Phone,
                YearsOfExperience = createDoctorWithUserDto.YearsOfExperience
            };

            var result = await CreateDoctorAsync(createDoctorDto);
            if (result.Success)
            {
                result.Message = "Doctor and user account created successfully";
                return result;
            }

            if (createdUserId > 0)
            {
                await TryRollbackUserAsync(createdUserId, authServiceBaseUrl, authorizationHeader, cancellationToken);
            }

            return result;
        }
        catch (Exception ex)
        {
            if (createdUserId > 0)
            {
                await TryRollbackUserAsync(createdUserId, authServiceBaseUrl, authorizationHeader, cancellationToken);
            }

            return ApiResponse<DoctorDto>.Fail("Failed to create doctor with user account", ex.Message);
        }
    }

    public async Task<ApiResponse<DoctorDto>> UpdateDoctorAsync(int id, UpdateDoctorDto updateDoctorDto)
    {
        var entity = new DoctorEntity
        {
            DoctorId = updateDoctorDto.DoctorId,
            UserId = updateDoctorDto.UserId,
            FirstName = updateDoctorDto.FirstName,
            LastName = updateDoctorDto.LastName,
            Specialization = updateDoctorDto.Specialization,
            Email = updateDoctorDto.Email,
            Phone = updateDoctorDto.Phone,
            YearsOfExperience = updateDoctorDto.YearsOfExperience,
            IsActive = updateDoctorDto.IsActive
        };

        var updated = await _doctorRepository.UpdateDoctorAsync(id, entity);
        return updated is null ? ApiResponse<DoctorDto>.Fail("Doctor not found") : ApiResponse<DoctorDto>.Ok(DoctorDto.FromEntity(updated), "Doctor updated successfully");
    }

    public async Task<ApiResponse<string>> DeleteDoctorAsync(int id)
    {
        var deleted = await _doctorRepository.DeleteDoctorAsync(id);
        return deleted ? ApiResponse<string>.Ok("Doctor deleted successfully") : ApiResponse<string>.Fail("Doctor not found");
    }

    private async Task<(bool Success, int UserId, string ErrorMessage)> CreateUserAsync(CreateDoctorWithUserDto dto, string authServiceBaseUrl, string? authorizationHeader, CancellationToken cancellationToken)
    {
        var username = string.IsNullOrWhiteSpace(dto.Username)
            ? dto.Email.Split('@')[0]
            : dto.Username;

        var createUserPayload = new
        {
            username,
            email = dto.Email,
            password = dto.Password,
            firstName = dto.FirstName,
            lastName = dto.LastName,
            phone = dto.Phone,
            role = string.IsNullOrWhiteSpace(dto.Role) ? UserRole.Doctor.ToString() : dto.Role,
            isActive = dto.IsActive
        };

        using var client = new HttpClient
        {
            BaseAddress = new Uri(authServiceBaseUrl, UriKind.Absolute)
        };
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/users")
        {
            Content = JsonContent.Create(createUserPayload)
        };

        if (!string.IsNullOrWhiteSpace(authorizationHeader))
        {
            request.Headers.TryAddWithoutValidation("Authorization", authorizationHeader);
        }

        using var response = await client.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = TryReadApiErrorMessage(body)
                ?? $"Auth service returned {(int)response.StatusCode}";
            return (false, 0, errorMessage);
        }

        var parsed = JsonSerializer.Deserialize<AuthApiResponse<AuthUserDto>>(body, JsonOptions);
        if (parsed is null)
        {
            return (false, 0, "Unable to parse user creation response");
        }

        if (!parsed.Success || parsed.Data is null || parsed.Data.Id <= 0)
        {
            return (false, 0, string.IsNullOrWhiteSpace(parsed.Message) ? "Failed to create user account" : parsed.Message);
        }

        return (true, parsed.Data.Id, string.Empty);
    }

    private async Task TryRollbackUserAsync(int userId, string authServiceBaseUrl, string? authorizationHeader, CancellationToken cancellationToken)
    {
        try
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri(authServiceBaseUrl, UriKind.Absolute)
            };
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/users/{userId}");
            if (!string.IsNullOrWhiteSpace(authorizationHeader))
            {
                request.Headers.TryAddWithoutValidation("Authorization", authorizationHeader);
            }

            using var response = await client.SendAsync(request, cancellationToken);
        }
        catch
        {
            // Ignore rollback exceptions because the primary request has already failed.
        }
    }

    private static string? TryReadApiErrorMessage(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            var parsed = JsonSerializer.Deserialize<AuthApiResponse<object>>(body, JsonOptions);
            if (!string.IsNullOrWhiteSpace(parsed?.Message))
            {
                return parsed.Message;
            }
        }
        catch
        {
            // Ignore parse issues and fallback to generic status-based message.
        }

        return null;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static string GetAuthServiceBaseUrl()
    {
        var fromEnv = Environment.GetEnvironmentVariable("SERVICES__AUTHSERVICEBASEURL")
            ?? Environment.GetEnvironmentVariable("AUTH_SERVICE_BASE_URL");
        return string.IsNullOrWhiteSpace(fromEnv)
            ? "http://auth-service:8080"
            : fromEnv;
    }

    private sealed class AuthApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

    private sealed class AuthUserDto
    {
        public int Id { get; set; }
    }
}
