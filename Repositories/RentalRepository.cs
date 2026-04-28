using APBD_TASK_7.DTOs;
using APBD_TASK_7.Exceptions;
using Microsoft.Data.SqlClient;

namespace APBD_TASK_7.Repositories;

public class RentalRepository : IRentalRepository
{
    private const string RentedStatusName = "Rented";

    private readonly string _connectionString;

    public RentalRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Missing 'Default' connection string.");
    }

    public async Task<CustomerRentalsResponse?> GetCustomerRentalsAsync(int customerId, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var customer = await GetCustomerAsync(connection, customerId, cancellationToken);
        if (customer is null)
        {
            return null;
        }

        var response = new CustomerRentalsResponse
        {
            FirstName = customer.Value.FirstName,
            LastName = customer.Value.LastName,
            Rentals = await LoadRentalsAsync(connection, customerId, cancellationToken)
        };

        return response;
    }

    public async Task CreateRentalAsync(int customerId, CreateRentalRequest request, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            if (!await CustomerExistsAsync(connection, transaction, customerId, cancellationToken))
            {
                throw new NotFoundException($"Customer with id {customerId} was not found.");
            }

            var movieIds = await ResolveMovieIdsAsync(connection, transaction, request.Movies, cancellationToken);

            var statusId = await GetStatusIdAsync(connection, transaction, RentedStatusName, cancellationToken)
                ?? throw new InvalidOperationException($"Status '{RentedStatusName}' is not configured in the database.");

            await InsertRentalAsync(connection, transaction, request.Id, request.RentalDate, customerId, statusId, cancellationToken);

            for (var i = 0; i < request.Movies.Count; i++)
            {
                await InsertRentalItemAsync(
                    connection,
                    transaction,
                    rentalId: request.Id,
                    movieId: movieIds[i],
                    priceAtRental: request.Movies[i].RentalPrice,
                    cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task<(string FirstName, string LastName)?> GetCustomerAsync(
        SqlConnection connection,
        int customerId,
        CancellationToken cancellationToken)
    {
        const string sql = @"SELECT first_name, last_name
                             FROM Customer
                             WHERE customer_id = @customerId;";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@customerId", customerId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return (reader.GetString(0), reader.GetString(1));
    }

    private static async Task<List<RentalResponse>> LoadRentalsAsync(
        SqlConnection connection,
        int customerId,
        CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT  r.rental_id,
                    r.rental_date,
                    r.return_date,
                    s.name           AS status_name,
                    m.title          AS movie_title,
                    ri.price_at_rental
            FROM    Rental       r
            JOIN    Status       s  ON s.status_id = r.status_id
            LEFT JOIN Rental_Item ri ON ri.rental_id = r.rental_id
            LEFT JOIN Movie       m  ON m.movie_id  = ri.movie_id
            WHERE   r.customer_id = @customerId
            ORDER BY r.rental_id, m.title;";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@customerId", customerId);

        var rentalsById = new Dictionary<int, RentalResponse>();

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var rentalId = reader.GetInt32(0);

            if (!rentalsById.TryGetValue(rentalId, out var rental))
            {
                rental = new RentalResponse
                {
                    Id = rentalId,
                    RentalDate = reader.GetDateTime(1),
                    ReturnDate = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                    Status = reader.GetString(3),
                    Movies = new List<RentalMovieResponse>()
                };
                rentalsById.Add(rentalId, rental);
            }

            if (!reader.IsDBNull(4))
            {
                rental.Movies.Add(new RentalMovieResponse
                {
                    Title = reader.GetString(4),
                    PriceAtRental = reader.GetDecimal(5)
                });
            }
        }

        return rentalsById.Values.ToList();
    }

    private static async Task<bool> CustomerExistsAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        int customerId,
        CancellationToken cancellationToken)
    {
        const string sql = "SELECT 1 FROM Customer WHERE customer_id = @customerId;";

        await using var command = new SqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("@customerId", customerId);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is not null;
    }

    private static async Task<List<int>> ResolveMovieIdsAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        List<CreateRentalMovieRequest> movies,
        CancellationToken cancellationToken)
    {
        const string sql = "SELECT movie_id FROM Movie WHERE title = @title;";

        var resolved = new List<int>(movies.Count);

        foreach (var movie in movies)
        {
            await using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@title", movie.Title);

            var movieId = await command.ExecuteScalarAsync(cancellationToken);
            if (movieId is null)
            {
                throw new NotFoundException($"Movie with title '{movie.Title}' was not found.");
            }

            resolved.Add((int)movieId);
        }

        return resolved;
    }

    private static async Task<int?> GetStatusIdAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        string statusName,
        CancellationToken cancellationToken)
    {
        const string sql = "SELECT status_id FROM Status WHERE name = @name;";

        await using var command = new SqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("@name", statusName);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is null ? null : (int)result;
    }

    private static async Task InsertRentalAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        int rentalId,
        DateTime rentalDate,
        int customerId,
        int statusId,
        CancellationToken cancellationToken)
    {
        // The request supplies an explicit rental_id; the column is IDENTITY,
        // so IDENTITY_INSERT must be toggled for the duration of the insert.
        const string sql = @"
            SET IDENTITY_INSERT Rental ON;
            INSERT INTO Rental (rental_id, rental_date, return_date, customer_id, status_id)
            VALUES (@rentalId, @rentalDate, NULL, @customerId, @statusId);
            SET IDENTITY_INSERT Rental OFF;";

        await using var command = new SqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("@rentalId", rentalId);
        command.Parameters.AddWithValue("@rentalDate", rentalDate);
        command.Parameters.AddWithValue("@customerId", customerId);
        command.Parameters.AddWithValue("@statusId", statusId);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task InsertRentalItemAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        int rentalId,
        int movieId,
        decimal priceAtRental,
        CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO Rental_Item (rental_id, movie_id, price_at_rental)
                             VALUES (@rentalId, @movieId, @priceAtRental);";

        await using var command = new SqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("@rentalId", rentalId);
        command.Parameters.AddWithValue("@movieId", movieId);
        command.Parameters.AddWithValue("@priceAtRental", priceAtRental);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
