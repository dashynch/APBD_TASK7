using APBD_TASK_7.DTOs;
using APBD_TASK_7.Repositories;

namespace APBD_TASK_7.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;

    public RentalService(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    public Task<CustomerRentalsResponse?> GetCustomerRentalsAsync(int customerId, CancellationToken cancellationToken)
        => _rentalRepository.GetCustomerRentalsAsync(customerId, cancellationToken);

    public Task CreateRentalAsync(int customerId, CreateRentalRequest request, CancellationToken cancellationToken)
        => _rentalRepository.CreateRentalAsync(customerId, request, cancellationToken);
}
