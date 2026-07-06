using WaySprout.Application.Ports;

namespace WaySprout.Application.UseCases.GetJobApplicationById;

public class GetJobApplicationByIdHandler(IJobApplicationRepository repository)
{
  public IJobApplicationRepository Repository { get; } = repository;

  public async Task<GetJobApplicationByIdDto?> HandleAsync(Guid id)
  {
    var jobApplication = await Repository.GetByIdAsync(id);

    if (jobApplication is null)
    {
      return null;
    }

    return new GetJobApplicationByIdDto(
      jobApplication.Id,
      jobApplication.Company,
      jobApplication.Position,
      jobApplication.Status.ToString(),
      jobApplication.AppliedOn,
      jobApplication.Description
    );
  }
}