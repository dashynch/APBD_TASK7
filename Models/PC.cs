namespace APBD_TASK_7.Models;

public class PC
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public float Weight { get; set; }
    public int Warranty { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Stock { get; set; }

    public ICollection<PCComponent> PCComponents { get; set; } = new List<PCComponent>();
}