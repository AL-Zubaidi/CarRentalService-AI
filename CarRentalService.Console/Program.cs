using CarRentalService.Application.Rentals;
using CarRentalService.Application.Returns;
using CarRentalService.ConsoleApp;
using CarRentalService.Domain.Pricing;
using CarRentalService.Domain.Rentals;
using CarRentalService.Infrastructure.Persistence;

var repository = new InMemoryRentalRepository();
var pricingSelector = new RentalPricingStrategySelector();
var returnCalculator = new RentalReturnCalculator();
var registerRentalService = new RegisterRentalService(repository);
var registerVehicleReturnService = new RegisterVehicleReturnService(
    repository,
    pricingSelector,
    returnCalculator);
var parser = new ConsoleInputParser(Console.In, Console.Out);
var errors = new ConsoleErrorPresenter(Console.Out);
var workflow = new ConsoleRentalWorkflow(
    registerRentalService,
    registerVehicleReturnService,
    parser,
    errors,
    Console.In,
    Console.Out);

workflow.Run();
