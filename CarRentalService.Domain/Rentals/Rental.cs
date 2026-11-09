using CarRentalService.Domain.Common;
using CarRentalService.Domain.Pricing;

namespace CarRentalService.Domain.Rentals;

public sealed class Rental
{
    public Rental(
        string bookingNumber,
        string personnummer,
        VehicleCategory vehicleCategory,
        DateTime bookingDateTime,
        decimal startOdometerKm)
    {
        if (string.IsNullOrWhiteSpace(bookingNumber))
        {
            throw new DomainValidationException(
                new DomainValidationError("BookingNumberRequired", "A booking number is required."));
        }

        if (string.IsNullOrWhiteSpace(personnummer))
        {
            throw new DomainValidationException(
                new DomainValidationError("PersonnummerRequired", "A personnummer is required."));
        }

        if (!Enum.IsDefined(vehicleCategory))
        {
            throw new DomainValidationException(
                new DomainValidationError("UnsupportedVehicleCategory", "The vehicle category is not supported."));
        }

        BookingNumber = bookingNumber;
        Personnummer = personnummer;
        VehicleCategory = vehicleCategory;
        BookingDateTime = bookingDateTime;
        StartOdometerKm = startOdometerKm;
    }

    public string BookingNumber { get; }
    public string Personnummer { get; }
    public VehicleCategory VehicleCategory { get; }
    public DateTime BookingDateTime { get; }
    public decimal StartOdometerKm { get; }
    public DateTime? ReturnDateTime { get; private set; }
    public decimal? EndOdometerKm { get; private set; }
    public int? NumberOfDays { get; private set; }
    public decimal? DistanceKm { get; private set; }
    public decimal? TotalRentalPriceSek { get; private set; }
    public bool HasReturn => ReturnDateTime.HasValue;

    public RentalReturnCalculation RegisterReturn(
        DateTime returnDateTime,
        decimal endOdometerKm,
        RentalReturnCalculator calculator,
        IRentalPricingStrategy pricingStrategy)
    {
        if (HasReturn)
        {
            throw new DomainValidationException(
                new DomainValidationError("AlreadyReturned", "This booking already has a registered return."));
        }

        if (returnDateTime < BookingDateTime)
        {
            throw new DomainValidationException(
                new DomainValidationError("ReturnBeforeBooking", "The return date and time cannot be earlier than the booking date and time."));
        }

        if (endOdometerKm < StartOdometerKm)
        {
            throw new DomainValidationException(
                new DomainValidationError("EndOdometerBelowStart", "The ending odometer reading cannot be less than the starting odometer reading."));
        }

        var calculation = calculator.Calculate(
            BookingDateTime,
            returnDateTime,
            StartOdometerKm,
            endOdometerKm,
            pricingStrategy);

        ReturnDateTime = returnDateTime;
        EndOdometerKm = endOdometerKm;
        NumberOfDays = calculation.NumberOfDays;
        DistanceKm = calculation.DistanceKm;
        TotalRentalPriceSek = calculation.TotalRentalPriceSek;

        return calculation;
    }
}
