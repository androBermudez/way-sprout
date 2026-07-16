using WaySprout.Application.Ports;
using WaySprout.Application.UseCases.GetJobApplicationById;
using WaySprout.Application.UseCases.GetJobApplications;
using WaySprout.Domain.Entities;

namespace WaySprout.Application.Tests.UseCases.GetJobApplicationById;

public class GetJobApplicationByIdHandlerTests
{
  [Fact]
  public async Task HandleAsync_ExistingId_ReturnsMappedDto()
  {
    var application = JobApplication.Create(
      Guid.NewGuid(),
      Guid.NewGuid(),
      "Acme Corp",
      "Software Engineer",
      "Full stack role.",
      new DateOnly(2026, 1, 15),
      "https://acme.example/jobs/123");
    var handler = new GetJobApplicationByIdHandler(new FakeJobApplicationRepository(application));

    var dto = await handler.HandleAsync(application.Id);

    Assert.NotNull(dto);
    Assert.Equal(application.Id, dto.Id);
    Assert.Equal(application.Company, dto.Company);
    Assert.Equal(application.Position, dto.Position);
    Assert.Equal(application.Status.ToString(), dto.Status);
    Assert.Equal(application.AppliedOn, dto.AppliedOn);
    Assert.Equal(application.Description, dto.Description);
    Assert.Equal(application.Url, dto.Url);
  }

  [Fact]
  public async Task HandleAsync_NonexistentId_ReturnsNull()
  {
    var handler = new GetJobApplicationByIdHandler(new FakeJobApplicationRepository());

    var dto = await handler.HandleAsync(Guid.NewGuid());

    Assert.Null(dto);
  }

  private static JobApplication Valid() =>
    JobApplication.Create(
      Guid.NewGuid(),
      Guid.NewGuid(),
      "Acme Corp",
      "Software Engineer",
      "Full stack role.",
      new DateOnly(2026, 1, 15),
      null);

  private sealed class FakeJobApplicationRepository(params JobApplication[] applications) : IJobApplicationRepository
  {
    public Task<IReadOnlyList<JobApplication>> GetAllAsync(JobApplicationFilter filter) =>
      Task.FromResult<IReadOnlyList<JobApplication>>(applications);

    public Task<JobApplication?> GetByIdAsync(Guid id) =>
      Task.FromResult(applications.FirstOrDefault(a => a.Id == id));

    public Task AddAsync(JobApplication application)
    {
      applications = [.. applications, application];
      return Task.CompletedTask;
    }

    public Task<bool> UpdateAsync(JobApplication application)
    {
      var index = Array.FindIndex(applications, a => a.Id == application.Id);
      if (index < 0)
      {
        return Task.FromResult(false);
      }
      applications[index] = application;
      return Task.FromResult(true);
    }

  }
}