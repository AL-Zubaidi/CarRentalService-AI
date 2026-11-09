using CarRentalService.Domain.Rentals;

namespace CarRentalService.Application.Returns;

public sealed record RegisterVehicleReturnResult(
    Rental Rental,
    int NumberOfDays,
    decimal DistanceKm,
    decimal TotalRentalPriceSek);
