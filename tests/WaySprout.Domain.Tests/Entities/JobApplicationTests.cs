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

  [Fact]
  public void Update_SetsStatusUrlAndUpdatedAt()
  {
    var application = Valid();
    var utcNow = new DateTime(2026, 7, 13, 10, 0, 0, DateTimeKind.Utc);

    application.Update(ApplicationStatus.Interviewing, utcNow, "https://newcorp.com/jobs/456");

    Assert.Equal(ApplicationStatus.Interviewing, application.Status);
    Assert.Equal(utcNow, application.UpdatedAtUtc);
    Assert.Equal("https://newcorp.com/jobs/456", application.Url);
  }

  [Fact]
  public void Update_NullUrl_ClearsUrl()
  {
    var application = JobApplication.Create(Guid.NewGuid(), Guid.NewGuid(), "Acme Corp", "Software Engineer", "Desc.", new DateOnly(2026, 1, 15), "https://acme.com/jobs/1");

    application.Update(ApplicationStatus.Applied, DateTime.UtcNow, null);

    Assert.Null(application.Url);
  }

  [Fact]
  public void Update_DoesNotChangeCompanyPositionDescriptionAppliedOn()
  {
    var application = Valid();

    application.Update(ApplicationStatus.Offer, DateTime.UtcNow);

    Assert.Equal("Acme Corp", application.Company);
    Assert.Equal("Software Engineer", application.Position);
    Assert.Equal("Full stack role.", application.Description);
    Assert.Equal(new DateOnly(2026, 1, 15), application.AppliedOn);
  }

  [Fact]
  public void Update_DoesNotChangeImmutableFields()
  {
    var application = Valid();
    var originalId = application.Id;
    var originalUserId = application.UserId;
    var originalCreatedAt = application.CreatedAtUtc;

    application.Update(ApplicationStatus.Offer, DateTime.UtcNow);

    Assert.Equal(originalId, application.Id);
    Assert.Equal(originalUserId, application.UserId);
    Assert.Equal(originalCreatedAt, application.CreatedAtUtc);
  }

  [Fact]
  public void UpdateDescription_SetsDescription()
  {
    var application = Valid();

    application.UpdateDescription("New description text.");

    Assert.Equal("New description text.", application.Description);
  }

  [Fact]
  public void UpdateDescription_DoesNotChangeOtherFields()
  {
    var application = Valid();
    var originalStatus = application.Status;
    var originalCompany = application.Company;

    application.UpdateDescription("Updated.");

    Assert.Equal(originalStatus, application.Status);
    Assert.Equal(originalCompany, application.Company);
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
