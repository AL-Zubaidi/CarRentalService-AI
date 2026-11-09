# Research: Rental Registration and Vehicle Return

## Scope

This research records planning decisions derived from the approved constitution, the approved specification, and the requested .NET 10 solution structure. It does not create implementation tasks or source code.

## Decision: Five-project Clean Architecture layout

**Decision**: Use `CarRentalService.Console`, `CarRentalService.Application`, `CarRentalService.Domain`, `CarRentalService.Infrastructure`, and `CarRentalService.Tests`.

**Rationale**: The split keeps console I/O at the boundary, keeps business rules independent of storage, gives the two use cases an application coordination layer, and isolates the mandated in-memory repository. It matches the requested structure without adding a web, database, or integration project.

**Alternatives considered**:

- A single project was rejected because it would mix console I/O, domain rules, use-case coordination, and storage.
- A web/API project was rejected by the constitution and explicit exclusions.
- Additional application abstractions or mediator infrastructure were rejected as unnecessary for two use cases.

## Decision: Domain entity without aggregates

**Decision**: Model `Rental` as a focused domain entity. Represent return registration and its invariants on the entity or a small domain service/value result where needed, without aggregate or aggregate-root terminology or behavior.

**Rationale**: The constitution requires DDD-inspired domain modeling while explicitly prohibiting aggregates and aggregate roots. The entity owns rules that must remain true for its own state, including personnummer presence, category membership, valid return ordering, non-decreasing odometer, and one-return-only behavior.

**Alternatives considered**:

- An aggregate root was rejected explicitly by the constitution.
- An anemic data-only model was rejected because validation and return invariants belong in the domain.
- A broad domain framework was rejected because it would violate KISS and add no value for the two use cases.

## Decision: Strategy Pattern for pricing

**Decision**: Define one pricing strategy contract and exactly three implementations: `SmallCar`, `Van`, and `Minibus`. Select a strategy from the recorded vehicle category through a small factory/registry boundary.

**Rationale**: This preserves the constitution's required Strategy Pattern and keeps category formulas isolated. The strategies use `decimal`, `NumberOfDays`, and `DistanceKm`, apply the approved formulas exactly, and return a final price rounded to two decimal places using `MidpointRounding.AwayFromZero`.

**Alternatives considered**:

- A conditional pricing switch was rejected because it would concentrate unrelated category formulas and not follow the required Strategy Pattern.
- A configurable pricing engine was rejected because rates and categories are fixed by the specification and no additional pricing rules are in scope.

## Decision: In-memory repository boundary

**Decision**: Define the smallest repository abstraction needed by A1 and A2 and implement it with `Dictionary<string, Rental>` in Infrastructure.

**Rationale**: A1 requires unique booking lookup and insertion; A2 requires lookup and replacement/update of the existing rental. The boundary keeps Application independent of storage while complying with the constitution's simple in-memory storage requirement.

**Alternatives considered**:

- Direct dictionary access from Application was rejected because it couples use cases to storage concerns.
- EF Core, SQL Server, files, or external persistence were rejected explicitly.
- A generic repository was rejected as unnecessary abstraction for one entity and two use cases.

## Decision: Constructor dependency injection at the composition boundary

**Decision**: Construct the repository, pricing strategy selector, and application use-case coordinators in the Console composition root and pass dependencies through constructors.

**Rationale**: This makes dependencies explicit, supports isolated xUnit tests, and avoids introducing a dependency-injection framework when the application only needs a small object graph.

**Alternatives considered**:

- Service locator/global state was rejected because it hides dependencies and harms testability.
- A full dependency-injection container was rejected because it is unnecessary for the requested scope.

## Decision: Validation and preservation order

**Decision**: A2 must load the rental, validate booking existence and return state, validate timestamp ordering and odometer ordering, calculate the result, and only then persist the changed rental. Rejected returns must not write to the repository.

**Rationale**: This directly satisfies the specification requirement that rejected returns preserve the rental and any existing return information. It also makes invalid return attempts deterministic and easy to test.

**Alternatives considered**:

- Persisting partial return data before validation was rejected because it could modify stored state after a rejection.
- Console-only validation was rejected because domain and application rules must be testable without console I/O.

## Decision: Console error handling

**Decision**: Console presentation translates validation failures into clear, user-readable error messages and allows the workflow to continue or return to its menu/input boundary without adding another use case.

**Rationale**: The specification requires clear messages but does not mandate exact wording. Keeping wording at the Console boundary prevents presentation concerns from entering domain logic while preserving testable typed validation outcomes underneath.

**Alternatives considered**:

- Exact message strings in the domain were rejected because they couple business rules to console presentation.
- Logging, messaging, or remote error reporting was rejected as out of scope.

## Decision: No external contracts directory

**Decision**: Do not create `contracts/` for this feature.

**Rationale**: The application is an internal console application with no public API, external integration, or inter-process contract. The console interaction and application request/result behavior can be documented in the plan, data model, and quickstart without inventing an external contract artifact.

## Remaining ambiguity report

No blocking business-rule ambiguity remains in `spec.md`. The specification requires clear error messages but does not prescribe exact text; the plan treats wording as a console presentation detail and requires that each message identify the rejected condition. No additional use case or business rule is introduced.
