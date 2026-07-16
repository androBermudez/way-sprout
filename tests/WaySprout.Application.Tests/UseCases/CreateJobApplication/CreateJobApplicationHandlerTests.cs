using WaySprout.Application.Ports;
using WaySprout.Application.UseCases.CreateJobApplication;
using WaySprout.Application.UseCases.GetJobApplications;
using WaySprout.Domain.Entities;

namespace WaySprout.Application.Tests.UseCases.CreateJobApplication;

public class CreateJobApplicationHandlerTests
{
  [Fact]
  public async Task HandleAsync_ValidCommand_AddsApplicationAndReturnsNewGuid()
  {
    var fake = new FakeJobApplicationRepository();
    var handler = new CreateJobApplicationHandler(fake);
    var command = new CreateJobApplicationCommand(
      "Acme Corp", "Software Engineer", "Full stack role.", new DateOnly(2026, 7, 1), null);

    var id = await handler.HandleAsync(command);

    Assert.NotEqual(Guid.Empty, id);
    Assert.NotNull(fake.Added);
    Assert.Equal(id, fake.Added.Id);
    Assert.Equal("Acme Corp", fake.Added.Company);
    Assert.Equal("Software Engineer", fake.Added.Position);
    Assert.Equal("Full stack role.", fake.Added.Description);
    Assert.Equal(new DateOnly(2026, 7, 1), fake.Added.AppliedOn);
    Assert.Null(fake.Added.Url);
  }

  [Fact]
  public async Task HandleAsync_CommandWithUrl_StoresUrl()
  {
    var fake = new FakeJobApplicationRepository();
    var handler = new CreateJobApplicationHandler(fake);
    var command = new CreateJobApplicationCommand(
      "Acme Corp", "Software Engineer", "Desc", new DateOnly(2026, 7, 1), "https://linkedin.com/job/123");

    await handler.HandleAsync(command);

    Assert.Equal("https://linkedin.com/job/123", fake.Added?.Url);
  }

  [Fact]
  public async Task HandleAsync_EachCall_ReturnsUniqueId()
  {
    var handler = new CreateJobApplicationHandler(new FakeJobApplicationRepository());
    var command = new CreateJobApplicationCommand("A", "B", "C", new DateOnly(2026, 7, 1), null);

    var id1 = await handler.HandleAsync(command);
    var id2 = await handler.HandleAsync(command);

    Assert.NotEqual(id1, id2);
  }

  private sealed class FakeJobApplicationRepository : IJobApplicationRepository
  {
    public JobApplication? Added { get; private set; }

    public Task AddAsync(JobApplication application)
    {
      Added = application;
      return Task.CompletedTask;
    }

    public Task<IReadOnlyList<JobApplication>> GetAllAsync(JobApplicationFilter filter) =>
      Task.FromResult<IReadOnlyList<JobApplication>>([]);

    public Task<JobApplication?> GetByIdAsync(Guid id) => Task.FromResult<JobApplication?>(null);

    public Task<bool> UpdateAsync(JobApplication application) => Task.FromResult(false);
  }
}