using CarPark.Application.DTOs;
using CarPark.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ParkingController : ControllerBase
{
    private readonly IParkingService _service;
    public ParkingController(IParkingService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Park([FromBody] ParkRequest request)
    {
        try
        {
            var res = await _service.ParkVehicleAsync(request);
            return Ok(res);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetStatus()
    {
        var res = await _service.GetStatusAsync();
        return Ok(res);
    }

    [HttpPost("exit")]
    public async Task<IActionResult> Exit([FromBody] ExitRequest request)
    {
        try
        {
            var res = await _service.ExitVehicleAsync(request);
            return Ok(res);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
