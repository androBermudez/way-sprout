using WaySprout.Application.Ports;
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

  public Task<IReadOnlyList<JobApplication>> GetAllAsync()
  {
    return Task.FromResult(Seed);
  }

  public Task<JobApplication?> GetByIdAsync(Guid id)
  {
    var jobApplication = Seed.FirstOrDefault(ja => ja.Id == id);
    return Task.FromResult(jobApplication);
  }
}
