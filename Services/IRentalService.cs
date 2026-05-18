using APBD_TASK_7.DTOs;

namespace APBD_TASK_7.Services;

public interface IRentalService
{
    Task<CustomerRentalsResponse?> GetCustomerRentalsAsync(int customerId);

    Task CreateRentalAsync(int customerId, CreateRentalRequest request);

    Task<CustomerDto?> GetCustomerAsync(int customerId);

    Task AddCustomerAsync(CustomerDto customer);

    Task UpdateCustomerAsync(int customerId, CustomerDto customer);

    Task DeleteCustomerAsync(int customerId);
}

  