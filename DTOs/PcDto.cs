namespace APBD_TASK_7.DTOs;
using System.ComponentModel.DataAnnotations;

public class PcDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public float Weight { get; set; }
    public int Warranty { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Stock { get; set; }
}

public class PcComponentsResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<ComponentInPcResponse> Components { get; set; } = new();
}

public class ComponentInPcResponse
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Amount { get; set; }
    public string ManufacturerName { get; set; } = null!;
    public string ComponentTypeName { get; set; } = null!;
}

public class CreatePcRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [Required]
    public float Weight { get; set; }

    [Required]
    public int Warranty { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public int Stock { get; set; }
}

public class UpdatePcRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [Required]
    public float Weight { get; set; }

    [Required]
    public int Warranty { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public int Stock { get; set; }
}