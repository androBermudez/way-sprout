namespace WaySprout.Application.UseCases.GetJobApplicationById;

public record GetJobApplicationByIdDto(
  Guid Id,
  string Company,
  string Position,
  string Status,
  DateOnly AppliedOn,
  string Description
);
