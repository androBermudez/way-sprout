using WaySprout.Application.Enums;

namespace WaySprout.Application.Services;

public class DateRangePresetResolver(TimeProvider timeProvider)
{
  public (DateOnly From, DateOnly To) Resolve(DateRangePreset preset)
  {
    var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

    return preset switch
    {
      DateRangePreset.Today => (today, today),
      DateRangePreset.Last2Days => (today.AddDays(-1), today),
      DateRangePreset.Last7Days => (today.AddDays(-6), today),
      DateRangePreset.Last30Days => (today.AddDays(-29), today),
      DateRangePreset.Last90Days => (today.AddDays(-89), today),
      DateRangePreset.ThisWeek => (StartOfWeek(today), today),
      DateRangePreset.ThisMonth => (new DateOnly(today.Year, today.Month, 1), today),
      _ => throw new ArgumentOutOfRangeException(nameof(preset), preset, "Unknown date range preset."),
    };
  }

  private static DateOnly StartOfWeek(DateOnly date)
  {
    // ISO 8601: weeks start on Monday.
    var offset = ((int)date.DayOfWeek + 6) % 7;
    return date.AddDays(-offset);
  }
}
