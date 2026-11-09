using CarRentalService.Application.Rentals;
using CarRentalService.Application.Returns;
using CarRentalService.Domain.Common;
using CarRentalService.Domain.Pricing;
using CarRentalService.Domain.Rentals;
using CarRentalService.Infrastructure.Persistence;

namespace CarRentalService.Tests.Application;
public class RentalFlowTests
{
    private static readonly DateTime Start = new(2026, 1, 1, 10, 0, 0);

    [Theory]
    [InlineData(VehicleCategory.SmallCar, 1000)]
    [InlineData(VehicleCategory.Van, 1700)]
    [InlineData(VehicleCategory.Minibus, 2450)]
    public void A1ThenA2_ComputesAndPersistsCorrectReturn(VehicleCategory category, int expected)
    {
        var repo = new InMemoryRentalRepository();
        var registered = new RegisterRentalService(repo).Execute(new("B1", "any text", category, Start, 100m));
        Assert.Equal("any text", registered.Rental.Personnummer);
        var result = new RegisterVehicleReturnService(repo, new RentalPricingStrategySelector(), new RentalReturnCalculator())
            .Execute(new("B1", Start.AddHours(25), 200m));
        Assert.Equal(2, result.NumberOfDays);
        Assert.Equal(100m, result.DistanceKm);
        Assert.Equal(expected, result.TotalRentalPriceSek);
        Assert.True(repo.TryGet("B1", out var stored));
        Assert.True(stored!.HasReturn);
        Assert.Equal(result.TotalRentalPriceSek, stored.TotalRentalPriceSek);
    }

    [Fact]
    public void DuplicateBooking_DoesNotReplaceOriginal()
    {
        var repo = new InMemoryRentalRepository();
        var service = new RegisterRentalService(repo);
        service.Execute(new("B1", "original", VehicleCategory.Van, Start, 100m));
        var ex = Assert.Throws<DomainValidationException>(() =>
            service.Execute(new("B1", "replacement", VehicleCategory.SmallCar, Start, 200m)));
        Assert.Equal("DuplicateBookingNumber", ex.Error.Code);
        repo.TryGet("B1", out var stored);
        Assert.Equal("original", stored!.Personnummer);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void MissingPersonnummer_IsRejected(string personnummer)
    {
        var repo = new InMemoryRentalRepository();
        var ex = Assert.Throws<DomainValidationException>(() =>
            new RegisterRentalService(repo).Execute(new("B1", personnummer, VehicleCategory.Van, Start, 0m)));
        Assert.Equal("PersonnummerRequired", ex.Error.Code);
        Assert.False(repo.TryGet("B1", out _));
    }

    [Fact]
    public void UnsupportedCategory_IsRejected()
    {
        var repo = new InMemoryRentalRepository();
        var ex = Assert.Throws<DomainValidationException>(() =>
            new RegisterRentalService(repo).Execute(new("B1", "person", (VehicleCategory)999, Start, 0m)));
        Assert.Equal("UnsupportedVehicleCategory", ex.Error.Code);
        Assert.False(repo.TryGet("B1", out _));
    }

    [Fact]
    public void UnknownBooking_IsRejected()
    {
        var repo = new InMemoryRentalRepository();
        var ex = Assert.Throws<DomainValidationException>(() =>
            new RegisterVehicleReturnService(repo, new RentalPricingStrategySelector(), new RentalReturnCalculator())
                .Execute(new("missing", Start.AddDays(1), 100m)));
        Assert.Equal("BookingNotFound", ex.Error.Code);
    }

    [Theory]
    [InlineData(-1, 100, "ReturnBeforeBooking")]
    [InlineData(24, 99, "EndOdometerBelowStart")]
    public void InvalidReturn_DoesNotMutateRental(int hours, int endKm, string expectedCode)
    {
        var repo = new InMemoryRentalRepository();
        new RegisterRentalService(repo).Execute(new("B1", "person", VehicleCategory.Van, Start, 100m));
        var returns = new RegisterVehicleReturnService(repo, new RentalPricingStrategySelector(), new RentalReturnCalculator());
        var ex = Assert.Throws<DomainValidationException>(() =>
            returns.Execute(new("B1", Start.AddHours(hours), endKm)));
        Assert.Equal(expectedCode, ex.Error.Code);
        repo.TryGet("B1", out var stored);
        Assert.False(stored!.HasReturn);
        Assert.Null(stored.EndOdometerKm);
        Assert.Null(stored.NumberOfDays);
        Assert.Null(stored.DistanceKm);
        Assert.Null(stored.TotalRentalPriceSek);
    }

    [Fact]
    public void SecondReturn_IsRejectedAndPreservesFirstReturn()
    {
        var repo = new InMemoryRentalRepository();
        new RegisterRentalService(repo).Execute(new("B1", "person", VehicleCategory.Van, Start, 100m));
        var returns = new RegisterVehicleReturnService(repo, new RentalPricingStrategySelector(), new RentalReturnCalculator());
        var first = returns.Execute(new("B1", Start.AddDays(1), 200m));
        var ex = Assert.Throws<DomainValidationException>(() =>
            returns.Execute(new("B1", Start.AddDays(2), 300m)));
        Assert.Equal("AlreadyReturned", ex.Error.Code);
        repo.TryGet("B1", out var stored);
        Assert.Equal(first.Rental.ReturnDateTime, stored!.ReturnDateTime);
        Assert.Equal(200m, stored.EndOdometerKm);
        Assert.Equal(first.TotalRentalPriceSek, stored.TotalRentalPriceSek);
    }
}
