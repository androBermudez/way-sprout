using WaySprout.Application.Ports;
using WaySprout.Application.UseCases.GetJobApplications;
using WaySprout.Application.UseCases.UpdateJobApplication;
using WaySprout.Domain.Entities;
using WaySprout.Domain.Enums;

namespace WaySprout.Application.Tests.UseCases.UpdateJobApplication;

public class UpdateJobApplicationHandlerTests
{
  [Fact]
  public async Task HandleAsync_ExistingId_UpdatesStatusUrlDescriptionAndReturnsTrue()
  {
    var application = Valid();
    var fake = new FakeJobApplicationRepository(application);
    var handler = new UpdateJobApplicationHandler(fake, new FixedTimeProvider());
    var command = new UpdateJobApplicationCommand(
      application.Id,
      ApplicationStatus.Interviewing,
      "Updated description.",
      "https://newcorp.com/jobs/456");

    var result = await handler.HandleAsync(command);

    Assert.True(result);
    Assert.NotNull(fake.Updated);
    Assert.Equal(ApplicationStatus.Interviewing, fake.Updated.Status);
    Assert.Equal("Updated description.", fake.Updated.Description);
    Assert.Equal("https://newcorp.com/jobs/456", fake.Updated.Url);
  }

  [Fact]
  public async Task HandleAsync_ExistingId_SetsUpdatedAtUtc()
  {
    var application = Valid();
    var fake = new FakeJobApplicationRepository(application);
    var fixedNow = new DateTime(2026, 7, 13, 12, 0, 0, DateTimeKind.Utc);
    var handler = new UpdateJobApplicationHandler(fake, new FixedTimeProvider(fixedNow));
    var command = new UpdateJobApplicationCommand(application.Id, ApplicationStatus.Applied, "Desc.", null);

    await handler.HandleAsync(command);

    Assert.Equal(fixedNow, fake.Updated!.UpdatedAtUtc);
  }

  [Fact]
  public async Task HandleAsync_NullUrl_ClearsUrl()
  {
    var application = JobApplication.Create(
      Guid.NewGuid(), Guid.NewGuid(), "Acme", "Dev", "Desc.",
      new DateOnly(2026, 1, 15), "https://original.com");
    var fake = new FakeJobApplicationRepository(application);
    var handler = new UpdateJobApplicationHandler(fake, new FixedTimeProvider());
    var command = new UpdateJobApplicationCommand(application.Id, ApplicationStatus.Applied, "Desc.", null);

    await handler.HandleAsync(command);

    Assert.Null(fake.Updated!.Url);
  }

  [Fact]
  public async Task HandleAsync_UnknownId_ReturnsFalse()
  {
    var fake = new FakeJobApplicationRepository();
    var handler = new UpdateJobApplicationHandler(fake, new FixedTimeProvider());
    var command = new UpdateJobApplicationCommand(
      Guid.NewGuid(), ApplicationStatus.Applied, "Desc.", null);

    var result = await handler.HandleAsync(command);

    Assert.False(result);
    Assert.Null(fake.Updated);
  }

  private static JobApplication Valid() =>
    JobApplication.Create(
      Guid.NewGuid(), Guid.NewGuid(), "Acme Corp", "Software Engineer",
      "Full stack role.", new DateOnly(2026, 1, 15), null);

  private sealed class FakeJobApplicationRepository(params JobApplication[] applications) : IJobApplicationRepository
  {
    public JobApplication? Updated { get; private set; }

    public Task<JobApplication?> GetByIdAsync(Guid id) =>
      Task.FromResult(applications.FirstOrDefault(a => a.Id == id));

    public Task<bool> UpdateAsync(JobApplication application)
    {
      Updated = application;
      return Task.FromResult(true);
    }

    public Task<IReadOnlyList<JobApplication>> GetAllAsync(JobApplicationFilter filter) =>
      Task.FromResult<IReadOnlyList<JobApplication>>([]);

    public Task AddAsync(JobApplication application) => Task.CompletedTask;
  }

  private sealed class FixedTimeProvider(DateTime? now = null) : TimeProvider
  {
    private readonly DateTime _now = now ?? new DateTime(2026, 7, 13, 12, 0, 0, DateTimeKind.Utc);

    public override DateTimeOffset GetUtcNow() => new(_now);
  }
}
