using WaySprout.Application.Ports;
using WaySprout.Application.UseCases.GetJobApplications;
using WaySprout.Domain.Entities;

namespace WaySprout.Application.Tests.UseCases.GetJobApplications;

public class GetJobApplicationsHandlerTests
{
  [Fact]
  public async Task HandleAsync_RepositoryHasApplications_ReturnsMappedDtos()
  {
    var application = Valid();
    var handler = new GetJobApplicationsHandler(new FakeJobApplicationRepository(application));

    var result = await handler.HandleAsync();

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
    var handler = new GetJobApplicationsHandler(new FakeJobApplicationRepository());

    var result = await handler.HandleAsync();

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

  private sealed class FakeJobApplicationRepository(params JobApplication[] applications) : IJobApplicationRepository
  {
    public Task<IReadOnlyList<JobApplication>> GetAllAsync() =>
      Task.FromResult<IReadOnlyList<JobApplication>>(applications);
  }
}
