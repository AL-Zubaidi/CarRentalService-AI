using CarRentalService.Domain.Rentals;

namespace CarRentalService.Domain.Pricing;

public interface IRentalPricingStrategy
{
    VehicleCategory Category { get; }

    decimal Calculate(int numberOfDays, decimal distanceKm);
}
