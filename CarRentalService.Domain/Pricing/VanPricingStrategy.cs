using CarRentalService.Domain.Rentals;

namespace CarRentalService.Domain.Pricing;

public sealed class VanPricingStrategy : IRentalPricingStrategy
{
    public VehicleCategory Category => VehicleCategory.Van;

    public decimal Calculate(int numberOfDays, decimal distanceKm) =>
        500m * numberOfDays * 1.2m + 5m * distanceKm;
}
