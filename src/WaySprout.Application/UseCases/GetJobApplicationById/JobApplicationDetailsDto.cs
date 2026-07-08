namespace WaySprout.Application.UseCases.GetJobApplicationById;

public record JobApplicationDetailsDto(
  Guid Id,
  string Company,
  string Position,
  string Status,
  DateOnly AppliedOn,
  string Description
);
