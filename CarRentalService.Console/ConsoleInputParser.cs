using CarRentalService.Domain.Rentals;

namespace CarRentalService.ConsoleApp;

public sealed class ConsoleInputParser
{
    private readonly TextReader input;
    private readonly TextWriter output;

    public ConsoleInputParser(TextReader input, TextWriter output)
    {
        this.input = input;
        this.output = output;
    }

    public string ReadRequired(string prompt)
    {
        output.Write(prompt);
        var value = input.ReadLine();
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new FormatException($"{prompt.Trim()} is required.");
        }

        return value;
    }

    public DateTime ReadDateTime(string prompt)
    {
        var value = ReadRequired(prompt);
        if (!DateTime.TryParse(value, out var result))
        {
            throw new FormatException("Enter a valid date and time.");
        }

        return result;
    }

    public decimal ReadDecimal(string prompt)
    {
        var value = ReadRequired(prompt);
        if (!decimal.TryParse(value, out var result))
        {
            throw new FormatException("Enter a valid numeric value.");
        }

        return result;
    }

    public VehicleCategory ReadVehicleCategory(string prompt)
    {
        var value = ReadRequired(prompt);
        if (!Enum.TryParse<VehicleCategory>(value, ignoreCase: true, out var result) ||
            !Enum.IsDefined(result))
        {
            throw new FormatException("Vehicle category must be SmallCar, Van, or Minibus.");
        }

        return result;
    }
}
