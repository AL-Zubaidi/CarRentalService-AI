using CarRentalService.Application.Abstractions;
using CarRentalService.Domain.Common;
using CarRentalService.Domain.Rentals;

namespace CarRentalService.Application.Rentals;

public sealed class RegisterRentalService
{
    private readonly IRentalRepository repository;

    public RegisterRentalService(IRentalRepository repository)
    {
        this.repository = repository;
    }

    public RegisterRentalResult Execute(RegisterRentalRequest request)
    {
        var rental = new Rental(
            request.BookingNumber,
            request.Personnummer,
            request.VehicleCategory,
            request.BookingDateTime,
            request.StartOdometerKm);

        if (!repository.Add(rental))
        {
            throw new DomainValidationException(
                new DomainValidationError("DuplicateBookingNumber", "The booking number is already in use."));
        }

        return new RegisterRentalResult(rental);
    }
}
