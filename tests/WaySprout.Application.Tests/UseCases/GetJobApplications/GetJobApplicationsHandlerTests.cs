using WaySprout.Application.Ports;
using WaySprout.Application.Services;
using WaySprout.Application.UseCases.GetJobApplications;
using WaySprout.Domain.Entities;
using WaySprout.Domain.Enums;

namespace WaySprout.Application.Tests.UseCases.GetJobApplications;

public class GetJobApplicationsHandlerTests
{
  [Fact]
  public async Task HandleAsync_RepositoryHasApplications_ReturnsMappedDtos()
  {
    var application = Valid();
    var handler = Handler(new FakeJobApplicationRepository(application));

    var result = await handler.HandleAsync(EmptyQuery());

    var dto = Assert.Single(result);
    Assert.Equal(application.Id, dto.Id);
    Assert.Equal(application.Company, dto.Company);
    Assert.Equal(application.Position, dto.Position);
    Assert.Equal(application.Status.ToString(), dto.Status);
    Assert.Equal(application.AppliedOn, dto.AppliedOn);
  }

  [Fact]
  public async Task HandleAsync_RepositoryEmpty_ReturnsEmptyList()
  {
    var handler = Handler(new FakeJobApplicationRepository());

    var result = await handler.HandleAsync(EmptyQuery());

    Assert.Empty(result);
  }

  private static JobApplication Valid() =>
    JobApplication.Create(
      Guid.NewGuid(),
      Guid.NewGuid(),
      "Acme Corp",
      "Software Engineer",
      "Full stack role.",
      new DateOnly(2026, 1, 15));

  private static JobApplicationQuery EmptyQuery() =>
    new(SearchText: null, Statuses: new HashSet<ApplicationStatus>(), AppliedRange: null, SortBy: null, Direction: null);

  private static GetJobApplicationsHandler Handler(IJobApplicationRepository repository) =>
    new(repository, new DateRangePresetResolver(TimeProvider.System));

  private sealed class FakeJobApplicationRepository(params JobApplication[] applications) : IJobApplicationRepository
  {
    public Task<IReadOnlyList<JobApplication>> GetAllAsync(JobApplicationFilter filter) =>
      Task.FromResult<IReadOnlyList<JobApplication>>(applications);

    public Task<JobApplication?> GetByIdAsync(Guid id) =>
      Task.FromResult(applications.FirstOrDefault(a => a.Id == id));
  }
}
