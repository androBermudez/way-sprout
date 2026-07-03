using WaySprout.Domain.Enums;

namespace WaySprout.Domain.Tests.Enums;

public class ApplicationStatusExtensionsTests
{
  [Theory]
  [InlineData(ApplicationStatus.Rejected)]
  [InlineData(ApplicationStatus.Withdrawn)]
  public void IsFinal_FinalStatuses_ReturnsTrue(ApplicationStatus status)
  {
    Assert.True(status.IsFinal());
  }

  [Theory]
  [InlineData(ApplicationStatus.Applied)]
  [InlineData(ApplicationStatus.Interviewing)]
  [InlineData(ApplicationStatus.Offer)]
  public void IsFinal_NonFinalStatuses_ReturnsFalse(ApplicationStatus status)
  {
    Assert.False(status.IsFinal());
  }
}
