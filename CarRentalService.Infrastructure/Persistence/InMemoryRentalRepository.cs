using CarRentalService.Application.Abstractions;
using CarRentalService.Domain.Rentals;

namespace CarRentalService.Infrastructure.Persistence;

public sealed class InMemoryRentalRepository : IRentalRepository
{
    private readonly Dictionary<string, Rental> rentals = new(StringComparer.Ordinal);

    public bool TryGet(string bookingNumber, out Rental? rental) =>
        rentals.TryGetValue(bookingNumber, out rental);

    public bool Add(Rental rental) => rentals.TryAdd(rental.BookingNumber, rental);

    public bool Update(Rental rental)
    {
        if (!rentals.ContainsKey(rental.BookingNumber))
        {
            return false;
        }

        rentals[rental.BookingNumber] = rental;
        return true;
    }
}
