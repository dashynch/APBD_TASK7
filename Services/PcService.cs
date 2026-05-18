namespace APBD_TASK_7.Services;
using APBD_TASK_7.DTOs;
using APBD_TASK_7.Repositories;

public class PcService : IPcService
{
    private readonly IPcRepository _repository;

    public PcService(IPcRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<PcDto>> GetAllPcsAsync()
    {
        return _repository.GetAllPcsAsync();
    }

    public Task<PcComponentsResponse?> GetPcComponentsAsync(int id)
    {
        return _repository.GetPcComponentsAsync(id);
    }

    public Task<PcDto> CreatePcAsync(CreatePcRequest request)
    {
        return _repository.CreatePcAsync(request);
    }

    public Task<bool> UpdatePcAsync(int id, UpdatePcRequest request)
    {
        return _repository.UpdatePcAsync(id, request);
    }

    public Task<bool> DeletePcAsync(int id)
    {
        return _repository.DeletePcAsync(id);
    }
}