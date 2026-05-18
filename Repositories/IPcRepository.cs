namespace APBD_TASK_7.Repositories;
using APBD_TASK_7.DTOs;

public interface IPcRepository
{
    Task<IEnumerable<PcDto>> GetAllPcsAsync();
    Task<PcComponentsResponse?> GetPcComponentsAsync(int id);
    Task<PcDto> CreatePcAsync(CreatePcRequest request);
    Task<bool> UpdatePcAsync(int id, UpdatePcRequest request);
    Task<bool> DeletePcAsync(int id);
}