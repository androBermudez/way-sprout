namespace WaySprout.Application.UseCases.CreateJobApplication;

public record CreateJobApplicationCommand(
  string Company,
  string Position,
  string Description,
  DateOnly AppliedOn,
  string? Url
);