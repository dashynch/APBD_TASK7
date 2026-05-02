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

    public Task<CustomerRentalsResponse?> GetCustomerRentalsAsync(int customerId)
    {
        return _rentalRepository.GetCustomerRentalsAsync(customerId);
    }

    public Task CreateRentalAsync(int customerId, CreateRentalRequest request)
    {
        return _rentalRepository.CreateRentalAsync(customerId, request);
    }

    public Task<CustomerDto?> GetCustomerAsync(int customerId)
    {
        return _rentalRepository.GetCustomerAsync(customerId);
    }

    public Task AddCustomerAsync(CustomerDto customer)
    {
        return _rentalRepository.AddCustomerAsync(customer);
    }

    public Task UpdateCustomerAsync(int customerId, CustomerDto customer)
    {
        return _rentalRepository.UpdateCustomerAsync(customerId, customer);
    }

    public Task DeleteCustomerAsync(int customerId)
    {
        return _rentalRepository.DeleteCustomerAsync(customerId);
    }
}
