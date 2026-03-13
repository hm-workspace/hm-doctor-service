using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DoctorService.Utils.Common;
using DoctorService.InternalModels.DTOs;
using DoctorService.Services;

namespace DoctorService.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/doctors")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<DoctorDto>>>> GetDoctors([FromQuery] SearchQuery searchQuery)
    {
        return Ok(await _doctorService.GetDoctorsAsync(searchQuery));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<DoctorDto>>> GetDoctor(int id)
    {
        var result = await _doctorService.GetDoctorByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("by-doctor-id/{doctorId}")]
    public async Task<ActionResult<ApiResponse<DoctorDto>>> GetDoctorByDoctorId(string doctorId)
    {
        var result = await _doctorService.GetDoctorByDoctorIdAsync(doctorId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("by-user-id/{userId:int}")]
    public async Task<ActionResult<ApiResponse<DoctorDto>>> GetDoctorByUserId(int userId)
    {
        var result = await _doctorService.GetDoctorByUserIdAsync(userId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("specialization/{specialization}")]
    public async Task<ActionResult<ApiResponse<PagedResult<DoctorDto>>>> GetDoctorsBySpecialization(string specialization, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(await _doctorService.GetDoctorsBySpecializationAsync(specialization, pageNumber, pageSize));
    }

    [HttpGet("specializations")]
    public async Task<ActionResult<ApiResponse<IEnumerable<string>>>> GetSpecializations()
    {
        return Ok(await _doctorService.GetSpecializationsAsync());
    }

    [HttpGet("generate-id")]
    public async Task<ActionResult<ApiResponse<string>>> GenerateDoctorId()
    {
        return Ok(await _doctorService.GenerateDoctorIdAsync());
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DoctorDto>>> CreateDoctor([FromBody] CreateDoctorDto createDoctorDto)
    {
        var result = await _doctorService.CreateDoctorAsync(createDoctorDto);
        return CreatedAtAction(nameof(GetDoctor), new { id = result.Data?.Id ?? 0 }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<DoctorDto>>> UpdateDoctor(int id, [FromBody] UpdateDoctorDto updateDoctorDto)
    {
        var result = await _doctorService.UpdateDoctorAsync(id, updateDoctorDto);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteDoctor(int id)
    {
        var result = await _doctorService.DeleteDoctorAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}


