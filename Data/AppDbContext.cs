namespace APBD_TASK_7.Data;

using APBD_TASK_7.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<PC> PCs { get; set; }
    public DbSet<Component> Components { get; set; }
    public DbSet<PCComponent> PCComponents { get; set; }
    public DbSet<ComponentType> ComponentTypes { get; set; }
    public DbSet<ComponentManufacturer> ComponentManufacturers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PC>(e =>
        {
            e.ToTable("PCs");
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(50);
            e.Property(p => p.Weight).IsRequired().HasColumnType("float(5)");
            e.Property(p => p.Warranty).IsRequired();
            e.Property(p => p.CreatedAt).IsRequired().HasColumnType("datetime");
            e.Property(p => p.Stock).IsRequired();
        });

        modelBuilder.Entity<ComponentType>(e =>
        {
            e.ToTable("ComponentTypes");
            e.HasKey(ct => ct.Id);
            e.Property(ct => ct.Abbreviation).IsRequired().HasMaxLength(30);
            e.Property(ct => ct.Name).IsRequired().HasMaxLength(150);
        });

        modelBuilder.Entity<ComponentManufacturer>(e =>
        {
            e.ToTable("ComponentManufacturers");
            e.HasKey(cm => cm.Id);
            e.Property(cm => cm.Abbreviation).IsRequired().HasMaxLength(30);
            e.Property(cm => cm.FullName).IsRequired().HasMaxLength(300);
            e.Property(cm => cm.FoundationDate).IsRequired().HasColumnType("date");
        });

        modelBuilder.Entity<Component>(e =>
        {
            e.ToTable("Components");
            e.HasKey(c => c.Code);
            e.Property(c => c.Code).IsRequired().HasColumnType("char(10)").HasMaxLength(10);
            e.Property(c => c.Name).IsRequired().HasMaxLength(300);
            e.Property(c => c.Description).HasColumnType("nvarchar(max)");
            e.HasOne(c => c.ComponentManufacturer).WithMany(m => m.Components).HasForeignKey(c => c.ComponentManufacturersId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(c => c.ComponentType).WithMany(t => t.Components).HasForeignKey(c => c.ComponentTypesId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PCComponent>(e =>
        {
            e.ToTable("PCComponents");
            e.HasKey(pc => new { pc.PCId, pc.ComponentCode });
            e.Property(pc => pc.Amount).IsRequired();
            e.HasOne(pc => pc.PC).WithMany(p => p.PCComponents).HasForeignKey(pc => pc.PCId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(pc => pc.Component).WithMany(c => c.PCComponents).HasForeignKey(pc => pc.ComponentCode).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ComponentType>().HasData(
            new ComponentType { Id = 1, Abbreviation = "CPU", Name = "Central Processing Unit" },
            new ComponentType { Id = 2, Abbreviation = "GPU", Name = "Graphics Processing Unit" },
            new ComponentType { Id = 3, Abbreviation = "RAM", Name = "Random Access Memory" }
        );

        modelBuilder.Entity<ComponentManufacturer>().HasData(
            new ComponentManufacturer { Id = 1, Abbreviation = "Intel", FullName = "Intel Corporation", FoundationDate = new DateTime(1968, 7, 18) },
            new ComponentManufacturer { Id = 2, Abbreviation = "AMD", FullName = "Advanced Micro Devices", FoundationDate = new DateTime(1969, 5, 1) },
            new ComponentManufacturer { Id = 3, Abbreviation = "NVIDIA", FullName = "NVIDIA Corporation", FoundationDate = new DateTime(1993, 4, 5) }
        );

        modelBuilder.Entity<Component>().HasData(
            new Component { Code = "I9-13900K ", Name = "Intel Core i9-13900K", Description = "High-end desktop CPU", ComponentManufacturersId = 1, ComponentTypesId = 1 },
            new Component { Code = "RX7900XTX ", Name = "AMD Radeon RX 7900 XTX", Description = "Flagship AMD GPU", ComponentManufacturersId = 2, ComponentTypesId = 2 },
            new Component { Code = "DDR5-32GB ", Name = "DDR5 32GB Kit", Description = "High-speed RAM", ComponentManufacturersId = 2, ComponentTypesId = 3 }
        );

        modelBuilder.Entity<PC>().HasData(
            new PC { Id = 1, Name = "Gaming Beast X", Weight = 12.5f, Warranty = 36, CreatedAt = new DateTime(2026, 5, 8, 9, 0, 0), Stock = 5 },
            new PC { Id = 2, Name = "Office Mini Pro", Weight = 4.2f, Warranty = 24, CreatedAt = new DateTime(2026, 4, 15, 13, 30, 0), Stock = 12 },
            new PC { Id = 3, Name = "Workstation Ultra", Weight = 18.0f, Warranty = 48, CreatedAt = new DateTime(2026, 3, 1, 10, 0, 0), Stock = 3 }
        );

        modelBuilder.Entity<PCComponent>().HasData(
            new PCComponent { PCId = 1, ComponentCode = "I9-13900K ", Amount = 1 },
            new PCComponent { PCId = 1, ComponentCode = "RX7900XTX ", Amount = 2 },
            new PCComponent { PCId = 2, ComponentCode = "DDR5-32GB ", Amount = 2 },
            new PCComponent { PCId = 3, ComponentCode = "I9-13900K ", Amount = 2 },
            new PCComponent { PCId = 3, ComponentCode = "RX7900XTX ", Amount = 1 }
        );
    }
}