using WaySprout.Domain.Entities;

namespace WaySprout.Application.Ports;

public interface IJobApplicationRepository
{
  Task<IReadOnlyList<JobApplication>> GetAllAsync();
}