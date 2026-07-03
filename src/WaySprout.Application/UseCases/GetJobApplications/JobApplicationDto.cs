namespace WaySprout.Application.UseCases.GetJobApplications
{
  public record JobApplicationDto(Guid Id, string Company, string Position, string Status, DateOnly AppliedOn);
}