# Tasks: Rental Registration and Vehicle Return

**Feature**: Rental Registration and Vehicle Return  
**Specification**: [spec.md](./spec.md)  
**Plan**: [plan.md](./plan.md)  
**Status**: Implemented demo; automated tests and A1→A2 Minibus happy path passed; task-level closure pending  
**Scope**: A1 Register Rental and A2 Register Vehicle Return only

> **Verification note (2026-11-08):** 26/26 xUnit tests passed and the Minibus A1→A2 console flow passed. Checkboxes below remain open pending exact task-by-task evidence review; several tests were consolidated into different files from the paths prescribed here. See [verification.md](./verification.md).

## Task Conventions

- Every task is independently actionable and names its exact target path.
- `[P]` means the task can run in parallel with other `[P]` tasks in the same phase after its listed dependencies are complete.
- `[US1]` maps to A1 Register Rental. `[US2]` maps to A2 Register Vehicle Return.
- No task creates aggregates, aggregate roots, databases, APIs, additional use cases, authentication, payments, cancellation, inventory, customer management, messaging, or microservices.

## Phase 1: Solution Setup and Project References

**Goal**: Create the .NET 10 solution and the five requested projects with only the required inward dependency direction.

- [ ] T001 Create the .NET 10 solution file at `CarRentalService.sln`.
- [ ] T002 [P] Create the .NET 10 class-library project at `CarRentalService.Domain/CarRentalService.Domain.csproj`.
- [ ] T003 [P] Create the .NET 10 class-library project at `CarRentalService.Application/CarRentalService.Application.csproj`.
- [ ] T004 [P] Create the .NET 10 class-library project at `CarRentalService.Infrastructure/CarRentalService.Infrastructure.csproj`.
- [ ] T005 [P] Create the .NET 10 console project at `CarRentalService.Console/CarRentalService.Console.csproj`.
- [ ] T006 [P] Create the .NET 10 xUnit test project at `CarRentalService.Tests/CarRentalService.Tests.csproj` with the required xUnit test packages only.
- [ ] T007 Add `CarRentalService.Domain/CarRentalService.Domain.csproj`, `CarRentalService.Application/CarRentalService.Application.csproj`, `CarRentalService.Infrastructure/CarRentalService.Infrastructure.csproj`, `CarRentalService.Console/CarRentalService.Console.csproj`, and `CarRentalService.Tests/CarRentalService.Tests.csproj` to `CarRentalService.sln` and configure their inward project references.
- [ ] T008 Configure shared build settings in `Directory.Build.props` for nullable reference types, implicit usings, and the .NET 10 target without adding web, ORM, database, or messaging dependencies.

## Phase 2: Domain Entities, Invariants, and Pricing Strategies

**Goal**: Implement the storage-independent domain model, return invariants, and exactly three category pricing strategies.

- [ ] T009 Create the vehicle category definition with exactly `SmallCar`, `Van`, and `Minibus` in `CarRentalService.Domain/Rentals/VehicleCategory.cs`.
- [ ] T010 Create the `Rental` domain entity in `CarRentalService.Domain/Rentals/Rental.cs` with booking number, required personnummer, vehicle category, booking timestamp, starting odometer, optional return fields, calculated duration, distance, and price; do not model it as an aggregate or aggregate root.
- [ ] T011 Create domain validation result/error types in `CarRentalService.Domain/Common/DomainValidationError.cs` and `CarRentalService.Domain/Common/DomainValidationException.cs` for clear, testable validation outcomes without console-specific message handling.
- [ ] T012 Implement A1 rental invariants in `CarRentalService.Domain/Rentals/Rental.cs`: personnummer is required, the category is one of the three allowed values, and no personnummer format, date, or checksum validation is performed.
- [ ] T013 Implement return validation and state-preserving transition behavior in `CarRentalService.Domain/Rentals/Rental.cs`: reject an existing return, a return timestamp earlier than booking, and an ending odometer lower than starting; ensure a rejected transition does not modify any return field.
- [ ] T014 Create the pricing strategy contract in `CarRentalService.Domain/Pricing/IRentalPricingStrategy.cs` using `decimal` for monetary inputs and results.
- [ ] T015 [P] Implement the `SmallCar` strategy using exactly `500 * NumberOfDays` in `CarRentalService.Domain/Pricing/SmallCarPricingStrategy.cs`.
- [ ] T016 [P] Implement the `Van` strategy using exactly `500 * NumberOfDays * 1.2 + 5 * DistanceKm` in `CarRentalService.Domain/Pricing/VanPricingStrategy.cs`.
- [ ] T017 [P] Implement the `Minibus` strategy using exactly `500 * NumberOfDays * 1.7 + 5 * DistanceKm * 1.5` in `CarRentalService.Domain/Pricing/MinibusPricingStrategy.cs`.
- [ ] T018 Create the category-to-strategy selector in `CarRentalService.Domain/Pricing/RentalPricingStrategySelector.cs` with exactly the three implemented strategies and no additional category.
- [ ] T019 Implement shared duration, distance, and final-price calculation rules in `CarRentalService.Domain/Rentals/RentalReturnCalculator.cs`: elapsed time, partial 24-hour periods rounded up, minimum one chargeable day for a valid rental, distance subtraction, decimal arithmetic, and final two-decimal `MidpointRounding.AwayFromZero` rounding.

## Phase 3: In-Memory Repository and Dependency Injection

**Goal**: Provide the smallest storage boundary and compose dependencies without introducing a container or service locator.

- [ ] T020 Define the rental repository contract required by A1 and A2 in `CarRentalService.Application/Abstractions/IRentalRepository.cs` for unique insertion, booking lookup, and replacement of a successfully returned rental.
- [ ] T021 Implement the repository with `Dictionary<string, Rental>` in `CarRentalService.Infrastructure/Persistence/InMemoryRentalRepository.cs`, keeping storage lifetime limited to the running process.
- [ ] T022 Use `CarRentalService.Domain/Pricing/RentalPricingStrategySelector.cs` directly as the sole pricing selector dependency; do not create an application-level pricing selector abstraction or adapter, and do not duplicate pricing rules outside the Domain project.
- [ ] T023 Configure constructor-based dependency composition in `CarRentalService.Console/Program.cs` for the repository, pricing selector, and A1/A2 application services; do not add a dependency-injection framework.
- [ ] T024 [P] Add repository tests in `CarRentalService.Tests/Infrastructure/InMemoryRentalRepositoryTests.cs` covering unique booking insertion, lookup, replacement, and process-local state.

## Phase 4: A1 Rental Registration

**Goal**: Deliver the independently testable A1 Register Rental user story.

**User Story**: `[US1]` A rental clerk registers a unique booking with the required rental facts so it can later be returned.

**Independent Test**: Register a valid rental and retrieve it by booking number; verify duplicate booking and missing personnummer attempts do not create or replace a rental.

- [ ] T025 [US1] Create A1 request and result models in `CarRentalService.Application/Rentals/RegisterRentalRequest.cs` and `CarRentalService.Application/Rentals/RegisterRentalResult.cs` containing booking number, personnummer, category, booking timestamp, and starting odometer.
- [ ] T026 [US1] Implement the A1 registration service in `CarRentalService.Application/Rentals/RegisterRentalService.cs` to validate the request through the domain, reject duplicate booking numbers, and persist only valid rentals through `IRentalRepository`.
- [ ] T027 [US1] Add A1 application tests after T026 in `CarRentalService.Tests/Application/RegisterRentalServiceTests.cs` for valid registration, duplicate booking rejection, required personnummer, rejection of unsupported vehicle categories, allowed categories, and preservation of an existing rental after rejection.
- [ ] T028 [P] [US1] Add A1 domain tests in `CarRentalService.Tests/Domain/RentalRegistrationTests.cs` proving that a supplied personnummer is stored as supplied without format, date, or checksum validation.

## Phase 5: A2 Vehicle Return and Price Calculation

**Goal**: Deliver the independently testable A2 Register Vehicle Return user story, including validation, state preservation, duration, distance, and pricing.

**User Story**: `[US2]` A rental clerk registers a valid return for an existing booking and receives duration, distance, and total price.

**Independent Test**: Register a rental, register one valid return, and verify all calculated values; separately exercise every rejected-return rule and verify no stored state mutation.

- [ ] T029 [US2] Create A2 request and result models in `CarRentalService.Application/Returns/RegisterVehicleReturnRequest.cs` and `CarRentalService.Application/Returns/RegisterVehicleReturnResult.cs` for booking number, return timestamp, ending odometer, duration, distance, and final SEK price.
- [ ] T030 [US2] Implement the A2 return service in `CarRentalService.Application/Returns/RegisterVehicleReturnService.cs` to load the existing rental, validate all return preconditions before persistence, calculate the result, and save only a successful return.
- [ ] T031 [US2] Add A2 validation tests after T030 in `CarRentalService.Tests/Application/RegisterVehicleReturnValidationTests.cs` for unknown booking, ending odometer below starting odometer, return timestamp earlier than booking, and second return attempts; assert a clear validation outcome and unchanged rental state for each rejection.
- [ ] T032 [P] [US2] Add duration and distance tests in `CarRentalService.Tests/Domain/RentalReturnCalculationTests.cs` for exactly 24 elapsed hours, partial additional 24-hour periods, minimum one-day charging, and `EndOdometerKm - StartOdometerKm`.
- [ ] T033 [P] [US2] Add SmallCar pricing tests after T015 and T019 in `CarRentalService.Tests/Domain/SmallCarPricingStrategyTests.cs` for the exact `500 * NumberOfDays` formula and two-decimal final rounding.
- [ ] T034 [P] [US2] Add Van pricing tests after T016 and T019 in `CarRentalService.Tests/Domain/VanPricingStrategyTests.cs` for the exact `500 * NumberOfDays * 1.2 + 5 * DistanceKm` formula.
- [ ] T035 [P] [US2] Add Minibus pricing tests after T017 and T019 in `CarRentalService.Tests/Domain/MinibusPricingStrategyTests.cs` for the exact `500 * NumberOfDays * 1.7 + 5 * DistanceKm * 1.5` formula.
- [ ] T036 [P] [US2] Add monetary behavior tests after T018 and T019 in `CarRentalService.Tests/Domain/RentalPriceRoundingTests.cs` proving decimal arithmetic and final two-decimal `MidpointRounding.AwayFromZero` behavior, including midpoint cases.
- [ ] T037 [US2] Add successful A2 application tests after T030 in `CarRentalService.Tests/Application/RegisterVehicleReturnServiceTests.cs` verifying stored return fields, duration, distance, category price, and one-return-only behavior after success.

## Phase 6: Console Interaction

**Goal**: Provide console-only input/output for A1 and A2 while keeping all domain and application rules testable without console I/O.

- [ ] T038 Implement the console workflow in `CarRentalService.Console/ConsoleRentalWorkflow.cs` with commands limited to Register Rental and Register Vehicle Return; do not add menus or commands for any other use case.
- [ ] T039 [P] Implement console input parsing and required-field prompts in `CarRentalService.Console/ConsoleInputParser.cs` for booking data, personnummer, category, date/time, and odometer values without personnummer format/date/checksum validation.
- [ ] T040 [P] Implement clear console error presentation in `CarRentalService.Console/ConsoleErrorPresenter.cs` for duplicate booking, missing personnummer, unknown booking, invalid odometer order, invalid timestamp order, and second-return rejection.
- [ ] T041 Connect the console workflow to the A1 and A2 application services in `CarRentalService.Console/Program.cs`, ensuring rejected returns do not trigger repository writes and no console-only business rule bypasses the application/domain validation.
- [ ] T042 Add console boundary tests, where practical without coupling domain tests to I/O, in `CarRentalService.Tests/Console/ConsoleRentalWorkflowTests.cs` for A1/A2 command routing and clear error display; keep business-rule assertions in Domain/Application tests.

## Phase 7: xUnit Tests and Final Verification

**Goal**: Prove the complete feature against the approved specification and prevent scope or architecture regressions.

- [ ] T043 Run the complete xUnit suite for `CarRentalService.sln` and resolve failures without weakening any approved validation or pricing rule.
- [ ] T044 Add an end-to-end application-slice test in `CarRentalService.Tests/Application/RentalUseCaseFlowTests.cs` covering A1 followed by successful A2 for all three categories without console I/O.
- [ ] T045 Add rejection-state regression tests in `CarRentalService.Tests/Application/RejectedReturnStateTests.cs` confirming every rejected return leaves booking data and all existing return fields unchanged.
- [ ] T046 Review project references in `CarRentalService.sln` and source files under `CarRentalService.Domain/`, `CarRentalService.Application/`, `CarRentalService.Infrastructure/`, and `CarRentalService.Console/` for prohibited scope and dependency violations; remove any aggregate, database, EF Core, API, authentication, or additional-use-case code.
- [ ] T047 Execute the validation scenarios in `specs/001-rental-registration/quickstart.md` and record the results in the implementation verification record at `specs/001-rental-registration/verification.md`.
- [ ] T048 Run `dotnet build CarRentalService.sln` and `dotnet test CarRentalService.sln --no-build`, then confirm the final solution contains only A1 and A2 and all tests pass.

## Dependencies and Execution Order

### Phase Dependencies

- Phase 1 must complete before every later phase.
- Phase 2 depends on Phase 1 and blocks application use-case work.
- Phase 3 depends on the domain types from Phase 2 and blocks A1/A2 application integration.
- Phase 4 depends on Phases 2 and 3.
- Phase 5 depends on Phases 2 and 3; it may begin in parallel with Phase 4 after the foundation is complete, but final integration should use the completed A1 repository/application boundary.
- Phase 6 depends on Phases 4 and 5.
- Phase 7 depends on Phases 4, 5, and 6.

### Key Task Dependencies

- T007 depends on T001–T006.
- T010–T019 depend on T007.
- T020–T023 depend on T007 and the relevant domain contracts from T009–T019.
- T024 depends on T021.
- T025–T026 depend on T010, T011, T012, and T020–T021; T027 additionally depends on T026; T028 depends on T010–T012.
- T029–T037 depend on T010, T013–T019, and T020–T021.
- T031 and T037 additionally depend on T030.
- T033 additionally depends on T015 and T019; T034 additionally depends on T016 and T019; T035 additionally depends on T017 and T019; T036 additionally depends on T018 and T019.
- T038–T042 depend on T023, T026, and T030.
- T043–T048 depend on all preceding implementation and test tasks relevant to the verification being run.

### Parallel Opportunities

- T002–T006 can run in parallel after T001.
- T015–T017 can run in parallel after T014.
- T024 can run in parallel with T022–T023 after T021.
- T027 and T028 can run in parallel only after T026 is complete for T027 and T010–T012 are complete for T028.
- T032 can run in parallel with T033–T036 after each task's stated implementation dependencies are complete.
- T039–T040 can run in parallel after the console project exists; T042 follows the workflow wiring.
- T033–T036 are independent category/rounding test files and can be implemented in parallel after T015–T019 are complete.

## Independent Test Criteria

- **A1 / US1**: A valid rental is retrievable by its unique booking number with all six required rental facts; duplicate bookings and missing personnummer are rejected without mutation; non-standard personnummer text is accepted without format/date/checksum validation.
- **A2 / US2**: A valid existing booking receives exactly one return with calculated duration, distance, and category price; unknown bookings, invalid timestamp order, invalid odometer order, and second returns produce clear errors and preserve stored state.
- **Pricing**: SmallCar, Van, and Minibus formulas match the approved expressions exactly; rates are 500 SEK/day and 5 SEK/km; final prices use decimal arithmetic and AwayFromZero rounding to two decimals.
- **Scope**: The console exposes only A1 and A2; no prohibited infrastructure, aggregate model, or additional use case exists.

## Implementation Strategy

### MVP

1. Complete Phases 1–3.
2. Complete Phase 4 and validate A1 independently.
3. Complete the domain/application portions of Phase 5 and validate A2 with tests.
4. Add Phase 6 console interaction.
5. Complete Phase 7 before considering the feature finished.

### Incremental delivery

A1 is the first independently demonstrable slice. A2 can be developed in parallel after the foundation but is the required second slice before console completion. Each slice must retain the exact A1/A2 boundary and pass its independent tests before proceeding.
