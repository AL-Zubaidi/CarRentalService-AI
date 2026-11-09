using CarRentalService.Domain.Rentals;

namespace CarRentalService.Domain.Pricing;

public sealed class SmallCarPricingStrategy : IRentalPricingStrategy
{
    public VehicleCategory Category => VehicleCategory.SmallCar;

    public decimal Calculate(int numberOfDays, decimal distanceKm) => 500m * numberOfDays;
}
