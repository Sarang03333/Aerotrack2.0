using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[ApiController] [Route("api/[controller]")] [Authorize]
public class InventoryController : ControllerBase {
    private readonly IInventoryService _service;
    public InventoryController(IInventoryService s) => _service = s;

    [HttpGet("parts")] public async Task<IActionResult> List() => Ok(await _service.GetAllAsync());

    [HttpPost("parts")] [Authorize(Policy="InventoryWrite")]
    public async Task<IActionResult> Create([FromBody] SparePart p) {
        var res = await _service.CreateAsync(p);
        return res == null ? Conflict() : Created("", res);
    }

    [HttpPut("parts/{id}")] [Authorize(Policy="InventoryWrite")]
    public async Task<IActionResult> Update(string id, [FromBody] SparePart p) =>
        await _service.UpdateAsync(id, p) ? NoContent() : NotFound();

    [HttpPost("parts/{id}/replenish")] [Authorize(Policy="InventoryWrite")]
    public async Task<IActionResult> Replenish(string id) {
        var res = await _service.ReplenishAsync(id);
        return res == null ? NotFound() : Ok(res);
    }

    [HttpDelete("parts/{id}")] [Authorize(Policy="InventoryWrite")]
    public async Task<IActionResult> Delete(string id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();
}