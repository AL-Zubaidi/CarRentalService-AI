using CarRentalService.Domain.Common;
using CarRentalService.Domain.Rentals;

namespace CarRentalService.Domain.Pricing;

public sealed class RentalPricingStrategySelector
{
    private readonly IReadOnlyDictionary<VehicleCategory, IRentalPricingStrategy> strategies;

    public RentalPricingStrategySelector()
    {
        strategies = new Dictionary<VehicleCategory, IRentalPricingStrategy>
        {
            [VehicleCategory.SmallCar] = new SmallCarPricingStrategy(),
            [VehicleCategory.Van] = new VanPricingStrategy(),
            [VehicleCategory.Minibus] = new MinibusPricingStrategy()
        };
    }

    public IRentalPricingStrategy Get(VehicleCategory category)
    {
        if (!strategies.TryGetValue(category, out var strategy))
        {
            throw new DomainValidationException(
                new DomainValidationError("UnsupportedVehicleCategory", "The vehicle category is not supported."));
        }

        return strategy;
    }
}
