using WaySprout.Application.Ports;
using WaySprout.Domain.Entities;

namespace WaySprout.Application.UseCases.CreateJobApplication;

public class CreateJobApplicationHandler(IJobApplicationRepository repository)
{
  private static readonly Guid SeedUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

  public async Task<Guid> HandleAsync(CreateJobApplicationCommand command)
  {
    var id = Guid.NewGuid();
    var application = JobApplication.Create(
      id,
      SeedUserId,
      command.Company,
      command.Position,
      command.Description,
      command.AppliedOn,
      command.Url);

    await repository.AddAsync(application);
    return id;
  }
}