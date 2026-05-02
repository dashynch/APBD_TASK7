using APBD_TASK_7.DTOs;
using APBD_TASK_7.Exceptions;
using APBD_TASK_7.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_TASK_7.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly IRentalService _rentalService;

    public CustomersController(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    [HttpGet("{id:int}/rentals")]
    public async Task<IActionResult> GetCustomerRentals(int id)
    {
        var customer = await _rentalService.GetCustomerRentalsAsync(id);
        if (customer is null)
        {
            return NotFound($"Customer with id {id} was not found.");
        }

        return Ok(customer);
    }

    [HttpPost("{id:int}/rentals")]
    public async Task<IActionResult> CreateRental(int id, [FromBody] CreateRentalRequest request)
    {
        try
        {
            await _rentalService.CreateRentalAsync(id, request);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }

        return CreatedAtAction(nameof(GetCustomerRentals), new { id }, null);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCustomer(int id)
    {
        var customer = await _rentalService.GetCustomerAsync(id);
        if (customer is null)
        {
            return NotFound($"Customer with id {id} was not found.");
        }

        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> AddCustomer([FromBody] CustomerDto customer)
    {
        await _rentalService.AddCustomerAsync(customer);
        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerDto customer)
    {
        await _rentalService.UpdateCustomerAsync(id, customer);
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        await _rentalService.DeleteCustomerAsync(id);
        return Ok();
    }
}
