using CarRentalService.Domain.Rentals;
using CarRentalService.Infrastructure.Persistence;

namespace CarRentalService.Tests.Infrastructure;
public class InMemoryRentalRepositoryTests
{
    [Fact]
    public void Repository_AddGetAndRejectDuplicate()
    {
        var repo = new InMemoryRentalRepository();
        var rental = new Rental("B1", "p", VehicleCategory.SmallCar, new DateTime(2026,1,1), 10m);
        Assert.True(repo.Add(rental));
        Assert.False(repo.Add(rental));
        Assert.True(repo.TryGet("B1", out var stored));
        Assert.Same(rental, stored);
    }

    [Fact]
    public void Repository_IsolatedInstancesAndUpdate()
    {
        var repo = new InMemoryRentalRepository();
        var rental = new Rental("B1", "p", VehicleCategory.SmallCar, new DateTime(2026,1,1), 10m);
        Assert.False(repo.Update(rental));
        Assert.True(repo.Add(rental));
        Assert.True(repo.Update(rental));
        Assert.False(new InMemoryRentalRepository().TryGet("B1", out _));
    }
}
