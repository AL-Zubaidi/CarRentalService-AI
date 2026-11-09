using CarRentalService.Domain.Common;

namespace CarRentalService.ConsoleApp;

public sealed class ConsoleErrorPresenter
{
    private readonly TextWriter output;

    public ConsoleErrorPresenter(TextWriter output)
    {
        this.output = output;
    }

    public void Show(Exception exception)
    {
        var message = exception switch
        {
            DomainValidationException validationException => validationException.Error.Message,
            FormatException formatException => formatException.Message,
            _ => "The operation could not be completed."
        };

        output.WriteLine($"Error: {message}");
    }
}
