# Quickstart Validation Guide: Rental Registration and Vehicle Return

This guide defines how the completed implementation will be validated. It is a run guide only; it does not contain application code or implementation tasks.

## Prerequisites

- .NET 10 SDK.
- The solution projects described in [plan.md](./plan.md).
- No database, EF Core installation, external service, or authentication setup.

## Build and test commands

From the repository root:

```powershell
dotnet restore
dotnet build
dotnet test
```

Expected result: the solution builds successfully and all xUnit tests pass.

## Console validation

Run the console project:

```powershell
dotnet run --project CarRentalService.Console
```

Use the console workflow to validate the following scenarios. The exact prompts may vary, but each outcome must be observable through the two approved use cases only.

### A1 — Register Rental

1. Register a rental with a unique booking number, a supplied personnummer, `SmallCar`, a booking date/time, and a starting odometer reading.
2. Confirm that the rental is accepted and available for a later A2 operation.
3. Attempt to reuse the booking number and confirm that the attempt is rejected without replacing the original rental.
4. Attempt to omit personnummer and confirm that the attempt is rejected. Supplying a personnummer with a non-standard format must not be rejected for format, date, or checksum reasons.

### A2 — Successful return and pricing

For each category, register a fresh rental and return it with a valid timestamp and ending odometer reading. Confirm:

- Distance equals `EndOdometerKm - StartOdometerKm`.
- A duration of up to 24 elapsed hours charges one day.
- A partial additional 24-hour period rounds up to another day.
- `SmallCar` uses `500 * NumberOfDays`.
- `Van` uses `500 * NumberOfDays * 1.2 + 5 * DistanceKm`.
- `Minibus` uses `500 * NumberOfDays * 1.7 + 5 * DistanceKm * 1.5`.
- The final price is decimal-based and rounded to two decimal places using `MidpointRounding.AwayFromZero`.

### A2 — Rejected returns preserve state

For an unreturned rental, attempt each invalid return independently:

1. Use an ending odometer lower than the starting odometer.
2. Use a return timestamp earlier than the booking timestamp.
3. Use an unknown booking number.

Confirm that each attempt displays a clear error, creates no return, and leaves the rental unchanged.

For a rental with a successful return, attempt a second return and confirm that it displays a clear error and leaves the original return date/time, ending odometer, duration, distance, and price unchanged.

## Automated test coverage expected

`dotnet test` must cover at least:

- All three pricing strategies and exact formulas.
- Decimal final-price rounding with midpoint values.
- Duration boundaries at one day and just over a whole-day boundary.
- Distance calculation.
- A1 required personnummer, allowed categories, and unique booking number.
- A2 unknown booking, invalid timestamp order, invalid odometer order, and second-return rejection.
- Preservation of rental state after every rejected return.
- Application behavior without console I/O.

See [data-model.md](./data-model.md) for field rules and state transitions.
