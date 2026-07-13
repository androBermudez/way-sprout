using WaySprout.Domain.Entities;
using WaySprout.Domain.Enums;
using WaySprout.Domain.Exceptions;

namespace WaySprout.Domain.Tests.Entities;

public class JobApplicationTests
{
  [Fact]
  public void Create_ValidInput_SetsAllFields()
  {
    var id = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var appliedOn = new DateOnly(2026, 1, 15);

    var application = JobApplication.Create(id, userId, "Acme Corp", "Software Engineer", "Full stack role.", appliedOn, "https://acme.com/jobs/123");

    Assert.Equal(id, application.Id);
    Assert.Equal(userId, application.UserId);
    Assert.Equal("Acme Corp", application.Company);
    Assert.Equal("Software Engineer", application.Position);
    Assert.Equal("Full stack role.", application.Description);
    Assert.Equal(appliedOn, application.AppliedOn);
    Assert.Equal("https://acme.com/jobs/123", application.Url);
  }

  [Fact]
  public void Create_ValidInput_DefaultsStatusToApplied()
  {
    Assert.Equal(ApplicationStatus.Applied, Valid().Status);
  }

  [Fact]
  public void Create_ValidInput_NotesIsEmpty()
  {
    Assert.Empty(Valid().Notes);
  }

  [Fact]
  public void Create_ValidInput_UrlIsNull()
  {
    Assert.Null(Valid().Url);
  }

  [Fact]
  public void Create_EmptyId_Throws()
  {
    Assert.Throws<DomainException>(() =>
      JobApplication.Create(Guid.Empty, Guid.NewGuid(), "Acme Corp", "Software Engineer", "Desc", new DateOnly(2026, 1, 15)));
  }

  [Fact]
  public void Create_EmptyUserId_Throws()
  {
    Assert.Throws<DomainException>(() =>
      JobApplication.Create(Guid.NewGuid(), Guid.Empty, "Acme Corp", "Software Engineer", "Desc", new DateOnly(2026, 1, 15)));
  }

  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  public void Create_EmptyCompany_Throws(string company)
  {
    Assert.Throws<DomainException>(() =>
      JobApplication.Create(Guid.NewGuid(), Guid.NewGuid(), company, "Software Engineer", "Desc", new DateOnly(2026, 1, 15)));
  }

  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  public void Create_EmptyPosition_Throws(string position)
  {
    Assert.Throws<DomainException>(() =>
      JobApplication.Create(Guid.NewGuid(), Guid.NewGuid(), "Acme Corp", position, "Desc", new DateOnly(2026, 1, 15)));
  }

  private static JobApplication Valid() =>
    JobApplication.Create(
      Guid.NewGuid(),
      Guid.NewGuid(),
      "Acme Corp",
      "Software Engineer",
      "Full stack role.",
      new DateOnly(2026, 1, 15));
}
