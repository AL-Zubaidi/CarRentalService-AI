# Data Model: Rental Registration and Vehicle Return

## Rental

A `Rental` is the domain entity identified by a unique booking number. It represents both the registered rental and, after a successful A2 operation, its registered return information. It is not an aggregate or aggregate root.

| Field | Meaning | Required at A1 | Rules |
|---|---|---:|---|
| `BookingNumber` | Unique booking identifier | Yes | Must be unique among rentals currently held in memory. |
| `Personnummer` | Customer identifier | Yes | Must be present; no format, date, or checksum validation. |
| `VehicleCategory` | Vehicle pricing category | Yes | Exactly `SmallCar`, `Van`, or `Minibus`. |
| `BookingDateTime` | Rental start timestamp | Yes | Used as the start of elapsed rental duration. |
| `StartOdometerKm` | Odometer at rental start | Yes | Used as the distance calculation baseline. |
| `ReturnDateTime` | Vehicle return timestamp | No at A1 | Set only after a valid A2 operation. |
| `EndOdometerKm` | Odometer at vehicle return | No at A1 | Set only after a valid A2 operation and must not be less than `StartOdometerKm`. |
| `NumberOfDays` | Chargeable rental duration | No at A1 | Calculated from elapsed booking-to-return time; minimum 1 for a valid rental and partial 24-hour periods round up. |
| `DistanceKm` | Distance driven | No at A1 | Calculated as `EndOdometerKm - StartOdometerKm`. |
| `TotalRentalPriceSek` | Final rental price | No at A1 | Calculated with the category formula using decimal arithmetic and rounded to two decimals with `MidpointRounding.AwayFromZero`. |

The implementation may represent absent return fields with nullable values. The business model must expose whether a return exists without adding a separate use case.

## VehicleCategory

The complete category set is:

- `SmallCar`
- `Van`
- `Minibus`

No fourth category is implemented. A future category extension point may exist in the strategy selection design, but it must not be enabled as part of this feature.

## Pricing inputs and formulas

Shared inputs:

- `BaseDailyRate = 500 SEK` per day.
- `KmRate = 5 SEK` per kilometer.
- Monetary calculations use `decimal`.
- The final price is rounded to two decimal places using `MidpointRounding.AwayFromZero`.

Exact category strategies:

- `SmallCar = 500 * NumberOfDays`
- `Van = 500 * NumberOfDays * 1.2 + 5 * DistanceKm`
- `Minibus = 500 * NumberOfDays * 1.7 + 5 * DistanceKm * 1.5`

## Relationships

- One booking number identifies at most one `Rental` in the in-memory repository.
- A rental may have zero or one registered vehicle return.
- A vehicle return belongs to the existing rental identified by its booking number.
- The pricing strategy is selected from the rental's `VehicleCategory`; there is no separate customer, vehicle inventory, payment, or booking aggregate.

## State transitions

```text
Unreturned Rental
  ├─ valid A2 request ───────────────> Returned Rental
  ├─ unknown booking ────────────────> unchanged / rejected
  ├─ already returned ───────────────> unchanged / rejected
  ├─ return before booking ──────────> unchanged / rejected
  └─ end odometer below start ───────> unchanged / rejected
```

A1 creates an `Unreturned Rental`. A2 validates all return preconditions before changing or persisting any return field. Any rejected A2 attempt leaves every stored return field unchanged, including an already valid return.

## Validation rules

### A1

1. Booking number must not already exist.
2. Personnummer must be present.
3. Vehicle category must be one of the three allowed values.
4. Booking timestamp and starting odometer must be recorded.

### A2

1. Booking number must identify an existing rental.
2. The rental must not already have a return.
3. `ReturnDateTime` must not be earlier than `BookingDateTime`.
4. `EndOdometerKm` must not be less than `StartOdometerKm`.
5. Elapsed time determines `NumberOfDays`; any partial 24-hour period rounds up, with a minimum of one day for a valid rental.
6. Distance and price are calculated only after validation succeeds.
7. A rejected return produces a clear error at the console boundary and does not persist state.

## Data ownership and boundaries

- Domain owns the entity state rules and pricing calculations.
- Application owns request coordination and repository interaction.
- Infrastructure owns dictionary storage.
- Console owns input parsing, output, and clear error presentation.
- Tests may construct domain entities and application dependencies directly; no console interaction is required for domain or use-case tests.
