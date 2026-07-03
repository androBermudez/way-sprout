using WaySprout.Domain;
using WaySprout.Domain.Enums;
using WaySprout.Domain.Exceptions;

namespace WaySprout.Domain.Tests;

public class SalaryExpectationTests
{
  [Fact]
  public void Constructor_FixedAmount_SetsMinAndMaxEqual()
  {
    var expectation = Valid();

    Assert.Equal(4000m, expectation.Min);
    Assert.Equal(4000m, expectation.Max);
    Assert.Equal(Currency.USD, expectation.Currency);
  }

  [Fact]
  public void IsRange_FixedAmount_ReturnsFalse()
  {
    Assert.False(Valid().IsRange);
  }

  [Fact]
  public void Constructor_Range_SetsMinAndMax()
  {
    var expectation = new SalaryExpectation(3000m, 5000m, Currency.USD);

    Assert.Equal(3000m, expectation.Min);
    Assert.Equal(5000m, expectation.Max);
  }

  [Fact]
  public void IsRange_Range_ReturnsTrue()
  {
    var expectation = new SalaryExpectation(3000m, 5000m, Currency.USD);

    Assert.True(expectation.IsRange);
  }

  [Fact]
  public void Constructor_MaxLessThanMin_Throws()
  {
    Assert.Throws<DomainException>(() => new SalaryExpectation(5000m, 3000m, Currency.USD));
  }

  [Theory]
  [InlineData(-1, 100)]
  [InlineData(100, -1)]
  public void Constructor_NegativeAmount_Throws(decimal min, decimal max)
  {
    Assert.Throws<DomainException>(() => new SalaryExpectation(min, max, Currency.USD));
  }

  private static SalaryExpectation Valid() => new(4000m, Currency.USD);
}
