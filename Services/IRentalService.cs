using APBD_TASK_7.DTOs;

namespace APBD_TASK_7.Services;

public interface IRentalService
{
    Task<CustomerRentalsResponse?> GetCustomerRentalsAsync(int customerId, CancellationToken cancellationToken);

    Task CreateRentalAsync(int customerId, CreateRentalRequest request, CancellationToken cancellationToken);
}
