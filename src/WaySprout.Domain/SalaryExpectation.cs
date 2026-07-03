using WaySprout.Domain.Enums;
using WaySprout.Domain.Exceptions;

namespace WaySprout.Domain;

public record SalaryExpectation
{
    public decimal Min { get; }
    public decimal? Max { get; }
    public Currency Currency { get; }

    public SalaryExpectation(decimal amount, Currency currency) : this(amount, amount, currency)
    {

    }

    public SalaryExpectation(decimal min, decimal max, Currency currency)
    {
        if (max < min)
        {
            throw new DomainException("Max must be greater than or equal to Min.");
        }
        if (max < 0 || min < 0)
        {
            throw new DomainException("Amounts must be positive.");
        }
        Min = min;
        Max = max;
        Currency = currency;
    }

    public bool IsRange => Min != Max;
}
