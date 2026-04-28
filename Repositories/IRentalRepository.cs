using APBD_TASK_7.DTOs;

namespace APBD_TASK_7.Repositories;

public interface IRentalRepository
{
    Task<CustomerRentalsResponse?> GetCustomerRentalsAsync(int customerId, CancellationToken cancellationToken);

    Task CreateRentalAsync(int customerId, CreateRentalRequest request, CancellationToken cancellationToken);
}
