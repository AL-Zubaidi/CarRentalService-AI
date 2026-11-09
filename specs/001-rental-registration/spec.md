# Feature Specification: Rental Registration and Vehicle Return

**Feature Branch**: `001-rental-registration`

**Created**: 2026-11-08

**Status**: Draft — approved pricing decisions incorporated

**Input**: User description: Create the functional specification for CarRentalService based strictly on the approved constitution and the original Swedish assignment.

## Scope

CarRentalService supports exactly two use cases:

- **A1 — Register Rental (Registrering av uthyrning)**
- **A2 — Register Vehicle Return (Registrering av återlämning)**

The system is a console application for recording rental and return information. Rental records exist only for the lifetime of the running application and are not persisted between runs. The specification defines what the application must do; it does not prescribe internal implementation structure.

The following are explicitly out of scope: authentication, payments, cancellation, vehicle inventory, customer management, messaging, reporting, database persistence, and any use case other than A1 and A2.

## User Scenarios & Testing

### User Story 1 — Register a rental (Priority: P1)

As a rental clerk, I want to register a rental so that a vehicle booking and its starting rental information are available for a later return.

**Why this priority**: A rental must exist before a return can be registered.

**Independent Test**: Start with no rental records, register one valid rental, and verify that the entered booking, customer, category, booking timestamp, and starting odometer are available for A2.

**Acceptance Scenarios**:

1. **Given** no rental has the requested booking number, **When** the clerk registers a booking number, required personnummer, vehicle category, booking date and time, and starting odometer reading, **Then** the system records the rental with all submitted values.
2. **Given** a rental already has a booking number, **When** the clerk attempts to register another rental with that booking number, **Then** the system does not create a second rental with that number and reports that the booking number is not unique.
3. **Given** the personnummer is missing, **When** the clerk attempts to register the rental, **Then** the system does not complete A1 and identifies the personnummer as required.
4. **Given** a category is selected, **When** the rental is recorded, **Then** the category is exactly one of `SmallCar`, `Van`, or `Minibus`.
5. **Given** a personnummer is supplied, **When** the rental is registered, **Then** the system stores it as supplied and does not apply personnummer format, date, or checksum validation.

---

### User Story 2 — Register a vehicle return and calculate the rental (Priority: P1)

As a rental clerk, I want to register the return for an existing booking so that the rental duration, distance driven, and total rental price are calculated from the rental and return information.

**Why this priority**: Completing A2 closes the rental transaction and provides the required charge calculation.

**Independent Test**: Register a rental first, register its return with a return timestamp and ending odometer, and verify the calculated duration, distance, and price for each category formula.

**Acceptance Scenarios**:

1. **Given** a booking number identifies an existing rental, **When** the clerk enters a return date and time and ending odometer reading, **Then** the system records the return and calculates rental duration, distance driven, and total rental price.
2. **Given** a booking number does not identify an existing rental, **When** the clerk attempts to register a return, **Then** the system does not create a return and reports that the booking must already exist.
3. **Given** the ending odometer reading is entered, **When** distance driven is calculated, **Then** distance driven equals ending odometer reading minus starting odometer reading.
4. **Given** a valid rental's elapsed time is less than or equal to 24 hours, **When** the rental duration is calculated, **Then** `NumberOfDays` is 1.
5. **Given** a valid rental's elapsed time includes a partial 24-hour period, **When** the rental duration is calculated, **Then** that partial period is rounded up to the next whole day.
6. **Given** the category is `SmallCar`, **When** the price is calculated, **Then** total rental price equals `500 * NumberOfDays` SEK.
7. **Given** the category is `Van`, **When** the price is calculated, **Then** total rental price equals `500 * NumberOfDays * 1.2 + 5 * DistanceKm` SEK.
8. **Given** the category is `Minibus`, **When** the price is calculated, **Then** total rental price equals `500 * NumberOfDays * 1.7 + 5 * DistanceKm * 1.5` SEK.
9. **Given** a calculated price has more than two decimal places, **When** the final rental price is produced, **Then** it is rounded to two decimal places using MidpointRounding.AwayFromZero.
10. **Given** the ending odometer reading is less than the starting odometer reading, **When** the clerk attempts to register the return, **Then** the system rejects the return, displays a clear error message, and preserves the rental without modifying stored return information.
11. **Given** the return date and time is earlier than the booking date and time, **When** the clerk attempts to register the return, **Then** the system rejects the return, displays a clear error message, and preserves the rental without modifying stored return information.
12. **Given** the booking already has a registered return, **When** the clerk attempts to register another return for that booking, **Then** the system rejects the return, displays a clear error message, and preserves the existing rental and stored return information.

---

### Edge Cases

- A duplicate booking number must not overwrite or create a second rental.
- A return for an unknown booking number must not be recorded.
- A missing personnummer must prevent A1; a supplied personnummer must not be rejected for format, date, or checksum reasons.
- A vehicle category outside `SmallCar`, `Van`, or `Minibus` must not be accepted.
- A return with an ending odometer reading lower than the starting odometer reading must be rejected with a clear error message, without modifying the rental or stored return information.
- A return with a return date and time earlier than the booking date and time must be rejected with a clear error message, without modifying the rental or stored return information.
- A second return for a booking that already has a registered return must be rejected with a clear error message, without modifying the rental or stored return information.

## Functional Requirements

### A1 — Register Rental

- **FR-001**: The system MUST provide the Register Rental use case and no additional rental-creation use case.
- **FR-002**: A1 MUST record a booking number that is unique among rentals currently held by the application.
- **FR-003**: A1 MUST require and record the customer personnummer.
- **FR-004**: A1 MUST record the personnummer as supplied without format, date, or checksum validation.
- **FR-005**: A1 MUST record exactly one vehicle category: `SmallCar`, `Van`, or `Minibus`.
- **FR-006**: A1 MUST record the booking date and time.
- **FR-007**: A1 MUST record the starting odometer reading in kilometers.
- **FR-008**: A1 MUST reject an attempt that would violate the booking-number uniqueness requirement and MUST leave the existing rental unchanged.
- **FR-009**: A1 MUST reject an attempt that omits the required personnummer.

### A2 — Register Vehicle Return

- **FR-010**: The system MUST provide the Register Vehicle Return use case and no additional return use case.
- **FR-011**: A2 MUST accept only a booking number that identifies an existing rental.
- **FR-012**: A2 MUST record the return date and time.
- **FR-013**: A2 MUST record the ending odometer reading in kilometers.
- **FR-014**: A2 MUST calculate distance driven as `ending odometer reading - starting odometer reading`.
- **FR-015**: A2 MUST calculate elapsed rental time between the booking date and time and the return date and time.
- **FR-016**: A2 MUST calculate `NumberOfDays` by rounding any partial 24-hour period of elapsed rental time up to the next whole day, with a minimum chargeable duration of one day for a valid rental.
- **FR-017**: A2 MUST calculate the total rental price using the category formula for the rental's recorded category.
- **FR-018**: For `SmallCar`, A2 MUST calculate `500 * NumberOfDays`.
- **FR-019**: For `Van`, A2 MUST calculate `500 * NumberOfDays * 1.2 + 5 * DistanceKm`.
- **FR-020**: For `Minibus`, A2 MUST calculate `500 * NumberOfDays * 1.7 + 5 * DistanceKm * 1.5`.
- **FR-021**: `BaseDailyRate` MUST be 500 SEK per day and shared across all three categories.
- **FR-022**: `KmRate` MUST be 5 SEK per kilometer and shared across all three categories.
- **FR-023**: Monetary calculations MUST use decimal arithmetic.
- **FR-024**: A2 MUST round the final rental price to two decimal places using MidpointRounding.AwayFromZero.
- **FR-025**: A2 MUST reject a return whose booking number does not identify an existing rental.
- **FR-026**: The calculated result MUST identify the rental duration, distance driven in kilometers, and total rental price in SEK.
- **FR-027**: A2 MUST reject a return when `EndOdometerKm` is less than `StartOdometerKm`.
- **FR-028**: When rejecting a return because `EndOdometerKm` is less than `StartOdometerKm`, the system MUST display a clear error message and MUST preserve the existing rental without modifying stored return information.
- **FR-029**: A2 MUST reject a return when `ReturnDateTime` is earlier than `BookingDateTime`.
- **FR-030**: When rejecting a return because `ReturnDateTime` is earlier than `BookingDateTime`, the system MUST display a clear error message and MUST preserve the existing rental without modifying stored return information.
- **FR-031**: A2 MUST reject a second return for a booking that already has a registered return.
- **FR-032**: When rejecting a second return, the system MUST display a clear error message and MUST preserve the existing rental and stored return information.

### Scope and operational constraints

- **FR-033**: The application MUST be usable as a console application.
- **FR-034**: Rental and return data MUST be held only in simple in-memory storage while the application is running.
- **FR-035**: The system MUST NOT require or use a database or EF Core.
- **FR-036**: The system MUST NOT implement aggregates or aggregate roots.
- **FR-037**: The system MUST not provide authentication, payments, cancellation, vehicle inventory, customer management, or any use case other than A1 and A2.

## Approved Pricing Rules

The approved pricing decisions are:

1. Rental duration is elapsed time between booking and return. Any partial 24-hour period is rounded up to the next whole day. A valid rental has a minimum chargeable duration of one day.
2. `BaseDailyRate` is 500 SEK per day and is shared across all categories.
3. `KmRate` is 5 SEK per kilometer and is shared across all categories.
4. Monetary calculations use decimal arithmetic. The final rental price is rounded to two decimal places using MidpointRounding.AwayFromZero.

The formulas remain exactly:

- `SmallCar = 500 * NumberOfDays`
- `Van = 500 * NumberOfDays * 1.2 + 5 * DistanceKm`
- `Minibus = 500 * NumberOfDays * 1.7 + 5 * DistanceKm * 1.5`

## Key Entities

- **Rental**: A registered rental identified by a unique booking number, with required customer personnummer, vehicle category, booking date and time, and starting odometer reading.
- **Vehicle Return**: The return information associated with an existing rental, including return date and time, ending odometer reading, calculated rental duration, calculated distance driven, and calculated total rental price.
- **Vehicle Category**: One of the three allowed values `SmallCar`, `Van`, or `Minibus`.
- **Pricing Inputs**: The shared `BaseDailyRate` of 500 SEK per day, shared `KmRate` of 5 SEK per kilometer, `NumberOfDays`, and `DistanceKm` used by the approved formulas.

## Success Criteria

### Measurable Outcomes

- **SC-001**: 100% of valid A1 attempts record all six required rental facts: booking number, personnummer, vehicle category, booking date, booking time, and starting odometer reading, associated as one rental.
- **SC-002**: 100% of A1 attempts with a duplicate booking number are rejected without changing the existing rental.
- **SC-003**: 100% of valid A2 attempts for an existing booking return a duration, distance, and price result using the applicable approved category formula.
- **SC-004**: 100% of A2 attempts for an unknown booking number are rejected without creating a return.
- **SC-005**: For every completed return, the reported distance equals ending odometer minus starting odometer, with no unexplained adjustment.
- **SC-006**: A rental clerk can complete each valid use case through console interaction without authentication, payment, or external service setup.
- **SC-007**: No scenario outside A1 and A2 is exposed as an available system capability.
- **SC-008**: 100% of final rental prices are calculated with decimal arithmetic and rounded to two decimal places using MidpointRounding.AwayFromZero.
- **SC-009**: 100% of returns with `EndOdometerKm < StartOdometerKm` are rejected with a clear error message and leave the rental and stored return information unchanged.
- **SC-010**: 100% of returns with `ReturnDateTime < BookingDateTime` are rejected with a clear error message and leave the rental and stored return information unchanged.
- **SC-011**: 100% of second-return attempts for already returned bookings are rejected with a clear error message and leave the existing rental and stored return information unchanged.

## Assumptions

- The system has one operational user role: a rental clerk. No authentication or role management is required.
- A booking number is the identifier used to connect A1 and A2; the assignment does not prescribe its representation or format beyond uniqueness.
- The three vehicle category names are the complete and exact category set for this scope.
- Data is available only during the current application run; restart data retention is not part of the assignment.
- The approved formulas and pricing decisions in this specification are authoritative. No discount, tax, deposit, or other pricing rule is included.
- No code is implemented during specification work.
- The constitution's future extensibility guidance is a design constraint for a later implementation, not an additional user-facing category or use case in this specification.
