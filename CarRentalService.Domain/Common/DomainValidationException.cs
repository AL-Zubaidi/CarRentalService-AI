namespace CarRentalService.Domain.Common;

public sealed class DomainValidationException : Exception
{
    public DomainValidationException(DomainValidationError error)
        : base(error.Message)
    {
        Error = error;
    }

    public DomainValidationError Error { get; }
}
