namespace APBD_TASK_7.DTOs;

public class RentalResponse
{
    public int Id { get; set; }
    public DateTime RentalDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = null!;
    public List<RentalMovieResponse> Movies { get; set; } = new();
}
