using CarRentalService.Application.Rentals;
using CarRentalService.Application.Returns;
using CarRentalService.Domain.Common;

namespace CarRentalService.ConsoleApp;

public sealed class ConsoleRentalWorkflow
{
    private readonly RegisterRentalService registerRentalService;
    private readonly RegisterVehicleReturnService registerVehicleReturnService;
    private readonly ConsoleInputParser parser;
    private readonly ConsoleErrorPresenter errors;
    private readonly TextReader input;
    private readonly TextWriter output;

    public ConsoleRentalWorkflow(
        RegisterRentalService registerRentalService,
        RegisterVehicleReturnService registerVehicleReturnService,
        ConsoleInputParser parser,
        ConsoleErrorPresenter errors,
        TextReader input,
        TextWriter output)
    {
        this.registerRentalService = registerRentalService;
        this.registerVehicleReturnService = registerVehicleReturnService;
        this.parser = parser;
        this.errors = errors;
        this.input = input;
        this.output = output;
    }

    public void Run()
    {
        while (true)
        {
            output.WriteLine();
            output.WriteLine("1. Register Rental");
            output.WriteLine("2. Register Vehicle Return");
            output.WriteLine("0. Exit");
            output.Write("Select an option: ");

            var option = input.ReadLine();
            if (option == "0" || option is null)
            {
                return;
            }

            try
            {
                switch (option)
                {
                    case "1":
                        RegisterRental();
                        break;
                    case "2":
                        RegisterReturn();
                        break;
                    default:
                        output.WriteLine("Error: Select 1, 2, or 0.");
                        break;
                }
            }
            catch (Exception exception) when (exception is DomainValidationException or FormatException)
            {
                errors.Show(exception);
            }
        }
    }

    private void RegisterRental()
    {
        var request = new RegisterRentalRequest(
            parser.ReadRequired("Booking number: "),
            parser.ReadRequired("Personnummer: "),
            parser.ReadVehicleCategory("Vehicle category (SmallCar, Van, Minibus): "),
            parser.ReadDateTime("Booking date and time: "),
            parser.ReadDecimal("Starting odometer km: "));

        registerRentalService.Execute(request);
        output.WriteLine("Rental registered successfully.");
    }

    private void RegisterReturn()
    {
        var request = new RegisterVehicleReturnRequest(
            parser.ReadRequired("Booking number: "),
            parser.ReadDateTime("Return date and time: "),
            parser.ReadDecimal("Ending odometer km: "));

        var result = registerVehicleReturnService.Execute(request);
        output.WriteLine("Vehicle return registered successfully.");
        output.WriteLine($"Rental duration: {result.NumberOfDays} day(s)");
        output.WriteLine($"Distance driven: {result.DistanceKm} km");
        output.WriteLine($"Total rental price: {result.TotalRentalPriceSek:0.00} SEK");
    }
}
