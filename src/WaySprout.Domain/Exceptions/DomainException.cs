namespace WaySprout.Domain.Exceptions;

public sealed class DomainException(string message) : Exception(message);