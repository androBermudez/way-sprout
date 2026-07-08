using WaySprout.Application.Enums;
using WaySprout.Application.Ports;
using WaySprout.Application.UseCases.GetJobApplications;
using WaySprout.Domain.Entities;

namespace WaySprout.Infrastructure.Repositories;

public class InMemoryJobApplicationRepository : IJobApplicationRepository
{
  private static readonly Guid SeedUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

  private static readonly IReadOnlyList<JobApplication> Seed =
  [
    JobApplication.Create(
      Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"),
      SeedUserId,
      "Nébula Soft",
      "Backend Engineer",
      """
      We're looking for someone to join the platform team, focused on designing
      internal APIs and evolving our billing core. You'll work with hexagonal
      architecture, message queues, and relational databases.
      """,
      new DateOnly(2026, 5, 12)
    ),
    JobApplication.Create(
      Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002"),
      SeedUserId,
      "Cronos Digital",
      "Frontend Engineer",
      """
      Product team looking to strengthen the frontend of our analytics suite.
      Main stack is React and TypeScript, with a focus on accessibility and
      reusable components.
      """,
      new DateOnly(2026, 6, 2)
    ),
    JobApplication.Create(
      Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003"),
      SeedUserId,
      "Vertice Labs",
      "Data Analyst",
      """
      Role focused on building dashboards and reporting models for the
      commercial team. Experience with advanced SQL and data visualization
      tools is valued.
      """,
      new DateOnly(2026, 6, 20)
    ),
  ];

  public Task<IReadOnlyList<JobApplication>> GetAllAsync(JobApplicationFilter filter)
  {
    IEnumerable<JobApplication> applications = Seed;

    if (!string.IsNullOrWhiteSpace(filter.SearchText))
    {
      applications = applications.Where(a =>
        a.Company.Contains(filter.SearchText, StringComparison.OrdinalIgnoreCase) ||
        a.Position.Contains(filter.SearchText, StringComparison.OrdinalIgnoreCase));
    }

    if (filter.Statuses.Count > 0)
    {
      applications = applications.Where(a => filter.Statuses.Contains(a.Status));
    }

    if (filter.AppliedFrom is { } from)
    {
      applications = applications.Where(a => a.AppliedOn >= from);
    }

    if (filter.AppliedTo is { } to)
    {
      applications = applications.Where(a => a.AppliedOn <= to);
    }

    applications = Sort(applications, filter.SortBy, filter.Direction);

    return Task.FromResult<IReadOnlyList<JobApplication>>([.. applications]);
  }

  private static IEnumerable<JobApplication> Sort(
    IEnumerable<JobApplication> applications,
    JobApplicationSortCriteria? sortBy,
    SortDirection? direction)
  {
    if (sortBy is null)
    {
      return applications;
    }

    var descending = direction == SortDirection.Desc;

    Func<JobApplication, string>? keySelector = sortBy switch
    {
      JobApplicationSortCriteria.Company => a => a.Company,
      JobApplicationSortCriteria.Position => a => a.Position,
      JobApplicationSortCriteria.DateApplied => a => a.AppliedOn.ToString("yyyy-MM-dd"),
      _ => null,
    };

    if (keySelector is null)
    {
      return applications;
    }
    return descending
      ? applications.OrderByDescending(keySelector, StringComparer.OrdinalIgnoreCase)
      : applications.OrderBy(keySelector, StringComparer.OrdinalIgnoreCase);
  }

  public Task<JobApplication?> GetByIdAsync(Guid id)
  {
    var jobApplication = Seed.FirstOrDefault(ja => ja.Id == id);
    return Task.FromResult(jobApplication);
  }
}
