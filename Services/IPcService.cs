namespace APBD_TASK_7.Services;
using APBD_TASK_7.DTOs;

public interface IPcService
{
    Task<IEnumerable<PcDto>> GetAllPcsAsync();
    Task<PcComponentsResponse?> GetPcComponentsAsync(int id);
    Task<PcDto> CreatePcAsync(CreatePcRequest request);
    Task<bool> UpdatePcAsync(int id, UpdatePcRequest request);
    Task<bool> DeletePcAsync(int id);
}