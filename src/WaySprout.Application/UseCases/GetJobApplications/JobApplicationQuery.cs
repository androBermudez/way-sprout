using WaySprout.Application.Enums;
using WaySprout.Domain.Enums;

namespace WaySprout.Application.UseCases.GetJobApplications;

public record JobApplicationQuery(
  string? SearchText,
  IReadOnlySet<ApplicationStatus> Statuses,   // empty means all
  DateRangePreset? AppliedRange,
  JobApplicationSortCriteria? SortBy,
  SortDirection? Direction);