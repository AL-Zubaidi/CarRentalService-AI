using CarRentalService.Domain.Pricing;

namespace CarRentalService.Domain.Rentals;

public sealed record RentalReturnCalculation(
    int NumberOfDays,
    decimal DistanceKm,
    decimal TotalRentalPriceSek);

public sealed class RentalReturnCalculator
{
    public RentalReturnCalculation Calculate(
        DateTime bookingDateTime,
        DateTime returnDateTime,
        decimal startOdometerKm,
        decimal endOdometerKm,
        IRentalPricingStrategy pricingStrategy)
    {
        var elapsed = returnDateTime - bookingDateTime;
        var numberOfDays = Math.Max(1, (int)Math.Ceiling(elapsed.TotalHours / 24d));
        var distanceKm = endOdometerKm - startOdometerKm;
        var totalPrice = pricingStrategy.Calculate(numberOfDays, distanceKm);
        var roundedPrice = decimal.Round(totalPrice, 2, MidpointRounding.AwayFromZero);

        return new RentalReturnCalculation(numberOfDays, distanceKm, roundedPrice);
    }
}
