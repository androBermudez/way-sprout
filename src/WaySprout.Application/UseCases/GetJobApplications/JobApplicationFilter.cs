using WaySprout.Application.Enums;
using WaySprout.Domain.Enums;

namespace WaySprout.Application.UseCases.GetJobApplications;

public record JobApplicationFilter(
  string? SearchText,
  IReadOnlySet<ApplicationStatus> Statuses,   // empty = all
  DateOnly? AppliedFrom,
  DateOnly? AppliedTo,
  JobApplicationSortCriteria? SortBy,
  SortDirection? Direction);
