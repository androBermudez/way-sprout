using System.Collections.Immutable;

using WaySprout.Domain.Enums;
using WaySprout.Domain.Exceptions;

namespace WaySprout.Domain.Entities;

public class JobApplication
{
  public Guid Id { get; private set; }
  public Guid UserId { get; private set; }
  public string Company { get; private set; } = default!;

  public string Position { get; private set; } = default!;

  public string Description { get; private set; } = default!;
  public string? Url { get; private set; }
  public ApplicationStatus Status { get; private set; }
  public IList<string> Notes { get; private set; }
  public DateOnly AppliedOn { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }
  public DateTime UpdatedAtUtc { get; private set; }

  private JobApplication()
  {
    Notes = ImmutableList<string>.Empty;
  }

  public static JobApplication Create(
    Guid id,
    Guid userId,
    string company,
    string position,
    string description,
    DateOnly appliedOn,
    string? url = null)
  {
    if (id == Guid.Empty)
    {
      throw new DomainException("Id cannot be empty.");
    }
    if (userId == Guid.Empty)
    {
      throw new DomainException("UserId cannot be empty.");
    }
    if (string.IsNullOrWhiteSpace(company))
    {
      throw new DomainException("Company cannot be empty.");
    }
    if (string.IsNullOrWhiteSpace(position))
    {
      throw new DomainException("Position cannot be empty.");
    }

    var now = DateTime.UtcNow;

    return new JobApplication
    {
      Id = id,
      UserId = userId,
      Company = company,
      Position = position,
      Description = description,
      AppliedOn = appliedOn,
      Status = ApplicationStatus.Applied,
      CreatedAtUtc = now,
      UpdatedAtUtc = now,
      Url = url,
    };
  }


  public void UpdateDescription(string description)
  {
    Description = description;
  }

  public void Update(
    ApplicationStatus status,
    DateTime utcNow,
    string? url = null
    )
  {
    Status = status;
    UpdatedAtUtc = utcNow;
    Url = url;
  }

}