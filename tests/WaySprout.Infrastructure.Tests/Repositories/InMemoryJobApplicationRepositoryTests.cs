using WaySprout.Application.Enums;
using WaySprout.Application.UseCases.GetJobApplications;
using WaySprout.Domain.Enums;
using WaySprout.Infrastructure.Repositories;

namespace WaySprout.Infrastructure.Tests.Repositories;

public class InMemoryJobApplicationRepositoryTests
{
  [Fact]
  public async Task GetAllAsync_NoFilter_ReturnsAllInSeedOrder()
  {
    var result = await Repository().GetAllAsync(EmptyFilter());

    Assert.Equal(["Nébula Soft", "Cronos Digital", "Vertice Labs"], result.Select(a => a.Company));
  }

  [Fact]
  public async Task GetAllAsync_SearchTextMatchesCompany_ReturnsOnlyThatApplication()
  {
    var result = await Repository().GetAllAsync(EmptyFilter() with { SearchText = "nébula" });

    var application = Assert.Single(result);
    Assert.Equal("Nébula Soft", application.Company);
  }

  [Fact]
  public async Task GetAllAsync_SearchTextMatchesPosition_ReturnsOnlyThatApplication()
  {
    var result = await Repository().GetAllAsync(EmptyFilter() with { SearchText = "analyst" });

    var application = Assert.Single(result);
    Assert.Equal("Vertice Labs", application.Company);
  }

  [Fact]
  public async Task GetAllAsync_SearchTextMatchesNothing_ReturnsEmpty()
  {
    var result = await Repository().GetAllAsync(EmptyFilter() with { SearchText = "nonexistent" });

    Assert.Empty(result);
  }

  [Fact]
  public async Task GetAllAsync_StatusFilterMatchesSeededStatus_ReturnsAll()
  {
    var result = await Repository().GetAllAsync(
      EmptyFilter() with { Statuses = new HashSet<ApplicationStatus> { ApplicationStatus.Applied } });

    Assert.Equal(3, result.Count);
  }

  [Fact]
  public async Task GetAllAsync_StatusFilterMatchesNoSeededStatus_ReturnsEmpty()
  {
    var result = await Repository().GetAllAsync(
      EmptyFilter() with { Statuses = new HashSet<ApplicationStatus> { ApplicationStatus.Rejected } });

    Assert.Empty(result);
  }

  [Fact]
  public async Task GetAllAsync_AppliedFrom_ExcludesEarlierApplications()
  {
    var result = await Repository().GetAllAsync(
      EmptyFilter() with { AppliedFrom = new DateOnly(2026, 6, 1) });

    Assert.Equal(["Cronos Digital", "Vertice Labs"], result.Select(a => a.Company));
  }

  [Fact]
  public async Task GetAllAsync_AppliedTo_ExcludesLaterApplications()
  {
    var result = await Repository().GetAllAsync(
      EmptyFilter() with { AppliedTo = new DateOnly(2026, 6, 2) });

    Assert.Equal(["Nébula Soft", "Cronos Digital"], result.Select(a => a.Company));
  }

  [Fact]
  public async Task GetAllAsync_AppliedFromAndTo_ReturnsOnlyWithinRange()
  {
    var result = await Repository().GetAllAsync(
      EmptyFilter() with { AppliedFrom = new DateOnly(2026, 6, 1), AppliedTo = new DateOnly(2026, 6, 10) });

    var application = Assert.Single(result);
    Assert.Equal("Cronos Digital", application.Company);
  }

  [Fact]
  public async Task GetAllAsync_SortByCompanyAscending_ReturnsAlphabeticalOrder()
  {
    var result = await Repository().GetAllAsync(
      EmptyFilter() with { SortBy = JobApplicationSortCriteria.Company, Direction = SortDirection.Asc });

    Assert.Equal(["Cronos Digital", "Nébula Soft", "Vertice Labs"], result.Select(a => a.Company));
  }

  [Fact]
  public async Task GetAllAsync_SortByCompanyDescending_ReturnsReverseAlphabeticalOrder()
  {
    var result = await Repository().GetAllAsync(
      EmptyFilter() with { SortBy = JobApplicationSortCriteria.Company, Direction = SortDirection.Desc });

    Assert.Equal(["Vertice Labs", "Nébula Soft", "Cronos Digital"], result.Select(a => a.Company));
  }

  [Fact]
  public async Task GetAllAsync_SortByDateAppliedDescending_ReturnsMostRecentFirst()
  {
    var result = await Repository().GetAllAsync(
      EmptyFilter() with { SortBy = JobApplicationSortCriteria.DateApplied, Direction = SortDirection.Desc });

    Assert.Equal(["Vertice Labs", "Cronos Digital", "Nébula Soft"], result.Select(a => a.Company));
  }

  private static InMemoryJobApplicationRepository Repository() => new();

  private static JobApplicationFilter EmptyFilter() =>
    new(SearchText: null, Statuses: new HashSet<ApplicationStatus>(), AppliedFrom: null, AppliedTo: null, SortBy: null, Direction: null);
}
