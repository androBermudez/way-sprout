using WaySprout.Domain.Enums;

namespace WaySprout.Application.UseCases.UpdateJobApplication;

public record UpdateJobApplicationCommand(
  Guid Id,
  ApplicationStatus Status,
  string Description,
  string? Url
);
