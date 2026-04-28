using System.ComponentModel.DataAnnotations;

namespace APBD_TASK_7.DTOs;

public class CreateRentalMovieRequest
{
    [Required]
    public string Title { get; set; } = null!;

    [Required]
    [Range(0.0, double.MaxValue, ErrorMessage = "Rental price must be non-negative.")]
    public decimal RentalPrice { get; set; }
}
