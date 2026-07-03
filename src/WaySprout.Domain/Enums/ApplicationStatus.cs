namespace WaySprout.Domain.Enums;

public enum ApplicationStatus
{
  Applied,
  Interviewing,
  Offer,
  Rejected,
  Withdrawn
}

public static class ApplicationStatusExtensions
{
  public static bool IsFinal(this ApplicationStatus status)
  {
    return status == ApplicationStatus.Rejected || status == ApplicationStatus.Withdrawn;
  }
}