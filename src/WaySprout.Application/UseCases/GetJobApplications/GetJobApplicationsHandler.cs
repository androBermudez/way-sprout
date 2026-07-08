using WaySprout.Application.Ports;
using WaySprout.Application.Services;

namespace WaySprout.Application.UseCases.GetJobApplications;

public class GetJobApplicationsHandler(
  IJobApplicationRepository repository,
  DateRangePresetResolver dateRangeResolver)
{
  public IJobApplicationRepository Repository { get; } = repository;

  public async Task<IReadOnlyList<JobApplicationDto>> HandleAsync(JobApplicationQuery query)
  {
    DateOnly? appliedFrom = null;
    DateOnly? appliedTo = null;

    if (query.AppliedRange is { } preset)
    {
      (appliedFrom, appliedTo) = dateRangeResolver.Resolve(preset);
    }

    var filter = new JobApplicationFilter(
      query.SearchText,
      query.Statuses,
      appliedFrom,
      appliedTo,
      query.SortBy,
      query.Direction);

    var jobApplications = await Repository.GetAllAsync(filter);

    return [.. jobApplications.Select(ja => new JobApplicationDto(
      ja.Id,
      ja.Company,
      ja.Position,
      ja.Status.ToString(),
      ja.AppliedOn
    ))];
  }
}
