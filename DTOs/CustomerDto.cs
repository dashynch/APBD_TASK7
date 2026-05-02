using System.ComponentModel.DataAnnotations;

namespace APBD_TASK_7.DTOs;

public class CustomerDto
{
    public int Id { get; set; }

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;
}
