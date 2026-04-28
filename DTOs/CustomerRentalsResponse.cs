namespace APBD_TASK_7.DTOs;

public class CustomerRentalsResponse
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public List<RentalResponse> Rentals { get; set; } = new();
}
