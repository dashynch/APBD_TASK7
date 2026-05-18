namespace APBD_TASK_7.Repositories;

using APBD_TASK_7.Data;
using APBD_TASK_7.DTOs;
using APBD_TASK_7.Models;
using Microsoft.EntityFrameworkCore;

public class PcRepository : IPcRepository
{
    private readonly AppDbContext _context;

    public PcRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PcDto>> GetAllPcsAsync()
    {
        return await _context.PCs
            .Select(pc => new PcDto
            {
                Id = pc.Id,
                Name = pc.Name,
                Weight = pc.Weight,
                Warranty = pc.Warranty,
                CreatedAt = pc.CreatedAt,
                Stock = pc.Stock
            }).ToListAsync();
    }

    public async Task<PcComponentsResponse?> GetPcComponentsAsync(int id)
    {
        var pc = await _context.PCs.Include(p => p.PCComponents)
            .ThenInclude(pc => pc.Component)
            .ThenInclude(c => c.ComponentManufacturer)
            .Include(p => p.PCComponents)
            .ThenInclude(pc => pc.Component)
            .ThenInclude(c => c.ComponentType)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pc is null)
            return null;

        return new PcComponentsResponse
        {
            Id = pc.Id,
            Name = pc.Name,
            Components = pc.PCComponents.Select(pcc => new ComponentInPcResponse
            {
                Code = pcc.ComponentCode.Trim(),
                Name = pcc.Component.Name,
                Description = pcc.Component.Description,
                Amount = pcc.Amount,
                ManufacturerName = pcc.Component.ComponentManufacturer.FullName,
                ComponentTypeName = pcc.Component.ComponentType.Name
            }).ToList()
        };
    }

    public async Task<PcDto> CreatePcAsync(CreatePcRequest request)
    {
        var pc = new PC
        {
            Name = request.Name,
            Weight = request.Weight,
            Warranty = request.Warranty,
            CreatedAt = request.CreatedAt,
            Stock = request.Stock
        };

        _context.PCs.Add(pc);
        await _context.SaveChangesAsync();

        return new PcDto
        {
            Id = pc.Id,
            Name = pc.Name,
            Weight = pc.Weight,
            Warranty = pc.Warranty,
            CreatedAt = pc.CreatedAt,
            Stock = pc.Stock
        };
    }

    public async Task<bool> UpdatePcAsync(int id, UpdatePcRequest request)
    {
        var pc = await _context.PCs.FindAsync(id);
        
        if (pc is null)
            return false;

        pc.Name = request.Name;
        pc.Weight = request.Weight;
        pc.Warranty = request.Warranty;
        pc.CreatedAt = request.CreatedAt;
        pc.Stock = request.Stock;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePcAsync(int id)
    {
        var pc = await _context.PCs.FindAsync(id);
        
        if (pc is null)
            return false;

        _context.PCs.Remove(pc);
        await _context.SaveChangesAsync();
        return true;
    }
}