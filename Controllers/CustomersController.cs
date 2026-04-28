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
    [ProducesResponseType(typeof(CustomerRentalsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerRentals(int id, CancellationToken cancellationToken)
    {
        var customer = await _rentalService.GetCustomerRentalsAsync(id, cancellationToken);
        if (customer is null)
        {
            return NotFound($"Customer with id {id} was not found.");
        }

        return Ok(customer);
    }

    [HttpPost("{id:int}/rentals")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateRental(
        int id,
        [FromBody] CreateRentalRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _rentalService.CreateRentalAsync(id, request, cancellationToken);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }

        return CreatedAtAction(nameof(GetCustomerRentals), new { id }, null);
    }
}
