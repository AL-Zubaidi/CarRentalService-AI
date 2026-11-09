namespace CarRentalService.Domain.Common;

public sealed record DomainValidationError(string Code, string Message);
