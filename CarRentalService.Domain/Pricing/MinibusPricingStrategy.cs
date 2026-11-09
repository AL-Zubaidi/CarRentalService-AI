using CarRentalService.Domain.Rentals;

namespace CarRentalService.Domain.Pricing;

public sealed class MinibusPricingStrategy : IRentalPricingStrategy
{
    public VehicleCategory Category => VehicleCategory.Minibus;

    public decimal Calculate(int numberOfDays, decimal distanceKm) =>
        500m * numberOfDays * 1.7m + 5m * distanceKm * 1.5m;
}
