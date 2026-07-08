using WaySprout.Application.Enums;
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

  [Fact]
  public async Task HandleAsync_QueryHasAppliedRange_ResolvesPresetBeforeCallingRepository()
  {
    var today = new DateOnly(2026, 6, 24);
    var fake = new FakeJobApplicationRepository();
    var handler = new GetJobApplicationsHandler(
      fake,
      new DateRangePresetResolver(new FixedTimeProvider(today.ToDateTime(TimeOnly.MinValue))));

    await handler.HandleAsync(EmptyQuery() with { AppliedRange = DateRangePreset.Today });

    Assert.NotNull(fake.LastFilter);
    Assert.Equal(today, fake.LastFilter.AppliedFrom);
    Assert.Equal(today, fake.LastFilter.AppliedTo);
  }

  [Fact]
  public async Task HandleAsync_QueryHasNoAppliedRange_PassesNullDatesToRepository()
  {
    var fake = new FakeJobApplicationRepository();
    var handler = Handler(fake);

    await handler.HandleAsync(EmptyQuery());

    Assert.NotNull(fake.LastFilter);
    Assert.Null(fake.LastFilter.AppliedFrom);
    Assert.Null(fake.LastFilter.AppliedTo);
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
    public JobApplicationFilter? LastFilter { get; private set; }

    public Task<IReadOnlyList<JobApplication>> GetAllAsync(JobApplicationFilter filter)
    {
      LastFilter = filter;
      return Task.FromResult<IReadOnlyList<JobApplication>>(applications);
    }

    public Task<JobApplication?> GetByIdAsync(Guid id) =>
      Task.FromResult(applications.FirstOrDefault(a => a.Id == id));
  }

  private sealed class FixedTimeProvider(DateTime now) : TimeProvider
  {
    public override DateTimeOffset GetUtcNow() => new(now, TimeSpan.Zero);
  }
}
