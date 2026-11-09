using CarRentalService.Domain.Rentals;

namespace CarRentalService.Application.Abstractions;

public interface IRentalRepository
{
    bool TryGet(string bookingNumber, out Rental? rental);

    bool Add(Rental rental);

    bool Update(Rental rental);
}
