namespace CarRentalService.Application.Returns;

public sealed record RegisterVehicleReturnRequest(
    string BookingNumber,
    DateTime ReturnDateTime,
    decimal EndOdometerKm);
