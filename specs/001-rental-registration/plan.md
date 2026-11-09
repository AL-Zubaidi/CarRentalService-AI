# Implementation Plan: Rental Registration and Vehicle Return

**Branch**: `001-rental-registration` | **Date**: 2026-11-08 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `specs/001-rental-registration/spec.md`

## Summary

CarRentalService will support only A1 Register Rental and A2 Register Vehicle Return. The design separates domain rules, application orchestration, in-memory storage, and console I/O across five projects. Domain pricing uses exactly three Strategy Pattern implementations and decimal arithmetic; application services coordinate validation, persistence, and return calculations.

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: C# / .NET 10

**Primary Dependencies**: .NET 10 base class libraries; xUnit in the test project. No web framework, ORM, database provider, messaging library, or external infrastructure.

**Storage**: In-memory `Dictionary<string, Rental>` repository for the application lifetime only.

**Testing**: xUnit unit tests for domain rules, pricing strategies, validation, application use cases, and console-independent behavior.

**Target Platform**: .NET 10 console execution on the supported development/runtime environment.

**Project Type**: Console application with four supporting class-library projects and one xUnit test project.

**Performance Goals**: No numeric performance target is specified; operations should remain simple in-process dictionary lookups and deterministic calculations.

**Constraints**: Console-only; in-memory only; no database or EF Core; no aggregates or aggregate roots; exactly A1 and A2; decimal monetary arithmetic; exactly SmallCar, Van, and Minibus strategies; console I/O only at the boundary; no authentication or other excluded features.

**Scale/Scope**: A single running process and a small in-memory rental set for the two specified use cases.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] Console-only C#/.NET 10 application; no web, desktop, or graphical presentation.
- [x] Clean Architecture dependency direction: Domain has no outer-layer dependency; Application depends on Domain; Infrastructure implements application-facing storage contracts; Console is the composition and I/O boundary.
- [x] Domain logic is independent of console I/O and storage.
- [x] No aggregates or aggregate roots are introduced; `Rental` is a domain entity with focused invariants.
- [x] Scope contains only A1 Register Rental and A2 Register Vehicle Return.
- [x] Pricing contains exactly SmallCar, Van, and Minibus strategies and preserves all approved formulas.
- [x] Monetary calculations use `decimal`; automated tests use xUnit.
- [x] Storage is a simple in-memory `Dictionary<string, Rental>` repository.
- [x] No authentication, payments, cancellation, inventory, customer management, database, EF Core, messaging, or microservices.
- [x] No unresolved business-rule requirements remain in the specification. Exact clear-error wording is a presentation detail to be kept user-readable and consistent.

## Project Structure

### Documentation (this feature)

```text
specs/001-rental-registration/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
└── checklists/          # Existing specification quality checklist
```

### Source Code (repository root)

```text
CarRentalService.Console/
├── Program.cs
└── ConsoleRentalWorkflow.cs
CarRentalService.Application/
├── Abstractions/
├── Rentals/
└── Returns/
CarRentalService.Domain/
├── Rentals/
├── Pricing/
└── Common/
CarRentalService.Infrastructure/
└── Persistence/
	└── InMemoryRentalRepository.cs
CarRentalService.Tests/
├── Domain/
├── Application/
└── Infrastructure/
```

**Structure Decision**: Use the five projects requested by the user. The source tree is the planned layout; implementation projects and source files are not created by this planning phase.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | The requested structure and repository boundary comply with the constitution. | No exception or unnecessary abstraction is required. |
