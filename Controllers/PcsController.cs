namespace APBD_TASK_7.Controllers;
using APBD_TASK_7.DTOs;
using APBD_TASK_7.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/pcs")]
public class PcsController : ControllerBase
{
    private readonly IPcService _pcService;

    public PcsController(IPcService pcService)
    {
        _pcService = pcService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _pcService.GetAllPcsAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}/components")]
    public async Task<IActionResult> GetComponents(int id)
    {
        var result = await _pcService.GetPcComponentsAsync(id);
        if (result is null)
            return NotFound($"PC with id {id} was not found.");
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePcRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _pcService.CreatePcAsync(request);
        return CreatedAtAction(nameof(GetComponents), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePcRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _pcService.UpdatePcAsync(id, request);
        if (!updated)
            return NotFound($"PC with id {id} was not found.");
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _pcService.DeletePcAsync(id);
        if (!deleted)
            return NotFound($"PC with id {id} was not found.");
        return NoContent();
    }
}