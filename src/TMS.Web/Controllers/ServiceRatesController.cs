using Microsoft.AspNetCore.Mvc;
using TMS.Core.DTOs;
using TMS.Core.Interfaces;

namespace TMS.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceRatesController : ControllerBase
{
    private readonly IServiceRateService _serviceRateService;
    private readonly ILogger<ServiceRatesController> _logger;

    public ServiceRatesController(IServiceRateService serviceRateService, ILogger<ServiceRatesController> logger)
    {
        _serviceRateService = serviceRateService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rates = await _serviceRateService.GetAllAsync();
        return Ok(rates);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var rate = await _serviceRateService.GetByIdAsync(id);
        if (rate == null)
            return NotFound();
        return Ok(rate);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceRateDto dto)
    {
        var rate = await _serviceRateService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = rate.Id }, rate);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateServiceRateDto dto)
    {
        var success = await _serviceRateService.UpdateAsync(id, dto);
        if (!success)
            return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _serviceRateService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("service/{serviceType}/role/{role}")]
    public async Task<IActionResult> GetByServiceAndRole(string serviceType, string role)
    {
        var rate = await _serviceRateService.GetByServiceAndRoleAsync(serviceType, role);
        if (rate == null)
            return NotFound();
        return Ok(rate);
    }
}
