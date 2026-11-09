using CarRentalService.Domain.Rentals;

namespace CarRentalService.Application.Rentals;

public sealed record RegisterRentalRequest(
    string BookingNumber,
    string Personnummer,
    VehicleCategory VehicleCategory,
    DateTime BookingDateTime,
    decimal StartOdometerKm);
