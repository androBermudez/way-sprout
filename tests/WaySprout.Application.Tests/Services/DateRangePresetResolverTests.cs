using WaySprout.Application.Enums;
using WaySprout.Application.Services;

namespace WaySprout.Application.Tests.Services;

public class DateRangePresetResolverTests
{
  // Fixed "today" for every test: Wednesday, 2026-06-24.
  private static readonly DateOnly Today = new(2026, 6, 24);

  [Fact]
  public void Resolve_Today_ReturnsTodayToToday()
  {
    var (from, to) = Resolver().Resolve(DateRangePreset.Today);

    Assert.Equal(Today, from);
    Assert.Equal(Today, to);
  }

  [Fact]
  public void Resolve_Last2Days_ReturnsYesterdayToToday()
  {
    var (from, to) = Resolver().Resolve(DateRangePreset.Last2Days);

    Assert.Equal(Today.AddDays(-1), from);
    Assert.Equal(Today, to);
  }

  [Fact]
  public void Resolve_Last7Days_ReturnsSevenDayRangeEndingToday()
  {
    var (from, to) = Resolver().Resolve(DateRangePreset.Last7Days);

    Assert.Equal(Today.AddDays(-6), from);
    Assert.Equal(Today, to);
  }

  [Fact]
  public void Resolve_Last30Days_ReturnsThirtyDayRangeEndingToday()
  {
    var (from, to) = Resolver().Resolve(DateRangePreset.Last30Days);

    Assert.Equal(Today.AddDays(-29), from);
    Assert.Equal(Today, to);
  }

  [Fact]
  public void Resolve_Last90Days_ReturnsNinetyDayRangeEndingToday()
  {
    var (from, to) = Resolver().Resolve(DateRangePreset.Last90Days);

    Assert.Equal(Today.AddDays(-89), from);
    Assert.Equal(Today, to);
  }

  [Fact]
  public void Resolve_ThisWeek_ReturnsMondayToToday()
  {
    var (from, to) = Resolver().Resolve(DateRangePreset.ThisWeek);

    Assert.Equal(new DateOnly(2026, 6, 22), from); // Monday
    Assert.Equal(Today, to);
  }

  [Fact]
  public void Resolve_ThisMonth_ReturnsFirstOfMonthToToday()
  {
    var (from, to) = Resolver().Resolve(DateRangePreset.ThisMonth);

    Assert.Equal(new DateOnly(2026, 6, 1), from);
    Assert.Equal(Today, to);
  }

  private static DateRangePresetResolver Resolver() =>
    new(new FixedTimeProvider(Today.ToDateTime(TimeOnly.MinValue)));

  private sealed class FixedTimeProvider(DateTime now) : TimeProvider
  {
    public override DateTimeOffset GetUtcNow() => new(now, TimeSpan.Zero);
  }
}
