using WaySprout.Application.Enums;
using WaySprout.Domain.Enums;

namespace WaySprout.Application.UseCases.GetJobApplications;

public record JobApplicationQuery(
  string? SearchText,
  IReadOnlySet<ApplicationStatus> Statuses,   // vacío = todos
  DateRangePreset? AppliedRange,
  JobApplicationSortCriteria? SortBy,
  SortDirection? Direction);