using System.ComponentModel.DataAnnotations;

namespace APBD_TASK_7.DTOs;

public class CreateRentalRequest
{
    [Required]
    public int Id { get; set; }

    [Required]
    public DateTime RentalDate { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one movie is required.")]
    public List<CreateRentalMovieRequest> Movies { get; set; } = new();
}
