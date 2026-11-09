# CarRentalService — Spec-Driven Development with GitHub Spec Kit

A small **.NET 10 console application** demonstrating a practical **Spec-Driven Development (SDD)** workflow using **GitHub Spec Kit**, GitHub Copilot, and independent code review/testing. The focus is traceability from requirements to implementation and verification—not feature count.

## What it does

* **A1 — Register rental:** store a unique booking number, required personnummer, vehicle category, booking date/time, and starting odometer.
* **A2 — Register vehicle return:** look up a booking, validate the return, calculate duration, distance and price, and persist the return in memory.
* Reject duplicate bookings, unknown bookings, repeated returns, backwards dates and decreasing odometer readings. Invalid returns must not change the saved rental.

Personnummer is required but **intentionally not format- or checksum-validated**, per the approved exercise scope. This is a demonstration, **not a production-ready rental system**.

## Architecture

|Project|Responsibility|
|-|-|
|`CarRentalService.Domain`|Rental entity, business invariants, pricing strategies and return calculations|
|`CarRentalService.Application`|A1/A2 use-case services and repository abstraction|
|`CarRentalService.Infrastructure`|In-memory `Dictionary<string, Rental>` repository|
|`CarRentalService.Console`|User interaction and manual dependency composition|
|`CarRentalService.Tests`|xUnit tests for pricing, validation, state and use-case flows|

The implementation follows **Clean Architecture** dependency boundaries and uses the **Strategy Pattern** for category-specific pricing. It deliberately avoids a database, API, authentication, messaging, and aggregate-root modeling because they are outside the approved scope.

### Pricing rules

|Category|Formula (SEK)|
|-|-|
|SmallCar|`500 × Days`|
|Van|`500 × Days × 1.2 + 5 × DistanceKm`|
|Minibus|`500 × Days × 1.7 + 5 × DistanceKm × 1.5`|

`Days` is the elapsed rental time rounded **up** to whole 24-hour periods (minimum 1). Money uses `decimal`, with final rounding to two decimal places using `MidpointRounding.AwayFromZero`.

## Run locally

Prerequisite: **.NET 10 SDK**.

```powershell
dotnet restore CarRentalService.sln
dotnet build CarRentalService.sln
dotnet test CarRentalService.sln --no-build
dotnet run --project CarRentalService.Console
```

### Example: Minibus rental

A rental starting **2026-01-02**, returned **2026-01-03**, with odometer changing from **100 km to 110 km**, was manually exercised through the console:

```text
Rental duration: 1 day(s)
Distance driven: 10 km
Total rental price: 925.00 SEK
```

Calculation: `500 × 1 × 1.7 + 5 × 10 × 1.5 = 925.00 SEK`.

## SDD workflow and evidence

This repository keeps the specification and planning artifacts alongside the source code:

|Stage|Artifact / evidence|
|-|-|
|Constitution|[`.specify/memory/constitution.md`](.specify/memory/constitution.md)|
|Specify / Clarify|[`spec.md`](specs/001-rental-registration/spec.md) and [`checklists/requirements.md`](specs/001-rental-registration/checklists/requirements.md)|
|Plan|[`plan.md`](specs/001-rental-registration/plan.md), [`research.md`](specs/001-rental-registration/research.md), [`data-model.md`](specs/001-rental-registration/data-model.md)|
|Tasks|[`tasks.md`](specs/001-rental-registration/tasks.md) — 48 planned tasks|
|Analyze|Spec Kit analysis performed before implementation; findings addressed during planning|
|Implement|Five .NET projects implementing A1/A2|
|Verify|[`quickstart.md`](specs/001-rental-registration/quickstart.md) and [`verification.md`](specs/001-rental-registration/verification.md)|

**Reported local test result (2026-11-08):** `26 passed, 0 failed, 0 skipped` using `dotnet test CarRentalService.sln --no-build`. The manual A1 → A2 Minibus happy path also passed. These are developer-reported results; see `verification.md` for checks still outstanding.

### Why this approach?

* **Specifications first:** define scope and acceptance criteria before implementation.
* **Clarification before coding:** resolve ambiguities such as day rounding and personnummer handling.
* **Architecture proportional to scope:** use clear layers and replaceable infrastructure without introducing unnecessary services.
* **Independent verification:** treat AI-generated code as code to review and test, not as automatically correct.
* **Traceability:** retain requirements, tasks, design decisions and verification evidence in the same repository.

