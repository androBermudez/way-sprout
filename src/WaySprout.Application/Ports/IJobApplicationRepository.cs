using WaySprout.Application.UseCases.GetJobApplications;
using WaySprout.Domain.Entities;

namespace WaySprout.Application.Ports;

public interface IJobApplicationRepository
{
  Task<IReadOnlyList<JobApplication>> GetAllAsync(JobApplicationFilter filter);

  Task<JobApplication?> GetByIdAsync(Guid id);

  Task AddAsync(JobApplication application);

  Task<bool> UpdateAsync(JobApplication application);
}