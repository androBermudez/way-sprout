using WaySprout.Application.Ports;

namespace WaySprout.Application.UseCases.GetJobApplications;

public class GetJobApplicationsHandler(IJobApplicationRepository repository)
{
  public IJobApplicationRepository Repository { get; } = repository;

  public async Task<IReadOnlyList<JobApplicationDto>> HandleAsync()
  {
    var jobApplications = await Repository.GetAllAsync();

    return [.. jobApplications.Select(ja => new JobApplicationDto(
      ja.Id,
      ja.Company,
      ja.Position,
      ja.Status.ToString(),
      ja.AppliedOn
    ))];
  }
}
