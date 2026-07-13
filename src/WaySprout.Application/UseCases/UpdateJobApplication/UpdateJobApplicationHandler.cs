using WaySprout.Application.Ports;

namespace WaySprout.Application.UseCases.UpdateJobApplication;

public class UpdateJobApplicationHandler(IJobApplicationRepository repository, TimeProvider timeProvider)
{
  public async Task<bool> HandleAsync(UpdateJobApplicationCommand command)
  {
    var application = await repository.GetByIdAsync(command.Id);
    if (application is null)
    {
      return false;
    }

    application.Update(
      command.Status,
      timeProvider.GetUtcNow().UtcDateTime,
      command.Url);

    application.UpdateDescription(command.Description);

    await repository.UpdateAsync(application);
    return true;
  }
}
