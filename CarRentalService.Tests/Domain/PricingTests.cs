using CarRentalService.Domain.Pricing;
using CarRentalService.Domain.Rentals;
using CarRentalService.Domain.Common;

namespace CarRentalService.Tests.Domain;
public class PricingTests
{
    [Theory]
    [InlineData(1, 0, 500)]
    [InlineData(2, 100, 1000)]
    public void SmallCar_UsesDailyRate(int days, int km, int expected) =>
        Assert.Equal(expected, new SmallCarPricingStrategy().Calculate(days, km));

    [Theory]
    [InlineData(1, 100, 1100)]
    [InlineData(2, 100, 1700)]
    public void Van_UsesApprovedFormula(int days, int km, int expected) =>
        Assert.Equal(expected, new VanPricingStrategy().Calculate(days, km));

    [Theory]
    [InlineData(1, 100, 1600)]
    [InlineData(2, 100, 2450)]
    public void Minibus_UsesApprovedFormula(int days, int km, int expected) =>
        Assert.Equal(expected, new MinibusPricingStrategy().Calculate(days, km));

    [Fact]
    public void Selector_RejectsUnsupportedCategory()
    {
        var error = Assert.Throws<DomainValidationException>(() =>
            new RentalPricingStrategySelector().Get((VehicleCategory)999));
        Assert.Equal("UnsupportedVehicleCategory", error.Error.Code);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(24, 1)]
    [InlineData(25, 2)]
    [InlineData(48, 2)]
    [InlineData(49, 3)]
    public void Duration_RoundsUpToFullDays(int hours, int expectedDays)
    {
        var start = new DateTime(2026, 1, 1);
        var result = new RentalReturnCalculator().Calculate(start, start.AddHours(hours), 100m, 125m, new SmallCarPricingStrategy());
        Assert.Equal(expectedDays, result.NumberOfDays);
        Assert.Equal(25m, result.DistanceKm);
        Assert.Equal(500m * expectedDays, result.TotalRentalPriceSek);
    }

    [Fact]
    public void FinalPrice_UsesAwayFromZeroMidpointRounding()
    {
        var calc = new RentalReturnCalculator();
        var result = calc.Calculate(new DateTime(2026,1,1), new DateTime(2026,1,2), 0m, 1m,
            new FixedPriceStrategy(1.005m));
        Assert.Equal(1.01m, result.TotalRentalPriceSek);
    }

    private sealed class FixedPriceStrategy(decimal amount) : IRentalPricingStrategy
    {
        public VehicleCategory Category => VehicleCategory.SmallCar;
        public decimal Calculate(int days, decimal km) => amount;
    }
}
