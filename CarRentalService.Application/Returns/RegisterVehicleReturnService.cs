using CarRentalService.Application.Abstractions;
using CarRentalService.Domain.Common;
using CarRentalService.Domain.Pricing;
using CarRentalService.Domain.Rentals;

namespace CarRentalService.Application.Returns;

public sealed class RegisterVehicleReturnService
{
    private readonly IRentalRepository repository;
    private readonly RentalPricingStrategySelector pricingSelector;
    private readonly RentalReturnCalculator returnCalculator;

    public RegisterVehicleReturnService(
        IRentalRepository repository,
        RentalPricingStrategySelector pricingSelector,
        RentalReturnCalculator returnCalculator)
    {
        this.repository = repository;
        this.pricingSelector = pricingSelector;
        this.returnCalculator = returnCalculator;
    }

    public RegisterVehicleReturnResult Execute(RegisterVehicleReturnRequest request)
    {
        if (!repository.TryGet(request.BookingNumber, out var rental) || rental is null)
        {
            throw new DomainValidationException(
                new DomainValidationError("BookingNotFound", "The booking number does not identify an existing rental."));
        }

        var pricingStrategy = pricingSelector.Get(rental.VehicleCategory);
        var calculation = rental.RegisterReturn(
            request.ReturnDateTime,
            request.EndOdometerKm,
            returnCalculator,
            pricingStrategy);

        if (!repository.Update(rental))
        {
            throw new DomainValidationException(
                new DomainValidationError("RentalUpdateFailed", "The vehicle return could not be stored."));
        }

        return new RegisterVehicleReturnResult(
            rental,
            calculation.NumberOfDays,
            calculation.DistanceKm,
            calculation.TotalRentalPriceSek);
    }
}
