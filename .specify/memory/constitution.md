<!--
Sync Impact Report
- Version change: unratified scaffold → 1.0.0
- Modified principles: template placeholders → five CarRentalService principles
- Added sections: Technical Constraints; Scope and Delivery Workflow
- Removed sections: none
- Follow-up TODOs: ratification date; unspecified business decisions must be clarified during Specification
-->

# CarRentalService Constitution

## Core Principles

### I. Console-Only .NET Platform
CarRentalService MUST be implemented exclusively as a C# and .NET 10 Console Application.
Web APIs, Blazor, Razor Pages, desktop UI, graphical UI, and other presentation platforms are
prohibited. Console I/O MUST remain at the application boundary.

### II. Clean, Focused Domain Design
The solution MUST follow Clean Architecture, SOLID, and DDD-inspired domain modeling while
explicitly avoiding Aggregates and Aggregate Roots. Domain logic MUST be independent of both
console I/O and storage. Responsibilities MUST be clearly separated without introducing
abstractions that are not justified by the two supported use cases.

### III. Explicit Business Scope
The system MUST support only A1 Register Rental and A2 Register Vehicle Return. The three
original Swedish pricing formulas are the only pricing business rules. Authentication, payments,
cancellation, customer management, vehicle inventory, messaging, and all other features are out
of scope. Requirements that are not specified MUST be flagged for clarification during
Specification; they MUST NOT be invented by implementation activities.

### IV. Extensible Strategy-Based Pricing
Pricing MUST use the Strategy Pattern with exactly three implemented categories: SmallCar, Van,
and Minibus. The design MUST allow a future category to be added without changing unrelated
pricing or use-case logic, but no additional category may be implemented under this constitution.
The approved Swedish formulas MUST be applied as specified; this constitution does not create or
alter formula details.

### V. Correctness, Simplicity, and Testability
Monetary calculations MUST use `decimal`. Automated tests MUST use xUnit. The implementation
MUST apply DRY and KISS, favoring the simplest design that satisfies the stated scope. Domain
rules MUST be testable without console I/O or storage. Personnummer MUST be required, but the
system MUST NOT perform format, date, or checksum validation on it.

## Technical Constraints

The application MUST use simple in-memory storage only, preferably a
`Dictionary<string, Rental>`. Databases, Entity Framework Core, SQL Server, and external
infrastructure are prohibited. Storage concerns MUST remain separate from domain logic. No
application code is to be implemented during constitution work; this document is the only
artifact changed by the constitution workflow.

## Scope and Delivery Workflow

Specification MUST resolve or explicitly record every unspecified business decision, including
any details needed to apply the original Swedish pricing formulas and the two use cases. Plan,
Tasks, Analyze, Implement, and Verify activities MUST trace back to this constitution and MUST
reject work outside the defined scope. Implementation MUST include xUnit coverage for domain
rules and use-case behavior, while preserving the separation between domain, application,
storage, and console concerns.

## Governance

This constitution is the highest-level project guidance and supersedes conflicting development
practices for CarRentalService. Every specification, plan, task set, implementation, analysis,
and verification MUST be reviewed for compliance with these principles. Amendments MUST be made
through the constitution workflow, document the reason and impact in a Sync Impact Report, and
update the version according to semantic versioning: MAJOR for incompatible principle changes,
MINOR for new or materially expanded principles, and PATCH for clarifications or editorial fixes.
Complexity and exceptions MUST be justified in the relevant design artifact. Unspecified business
decisions remain clarification items and MUST NOT be silently resolved by later phases.

**Version**: 1.0.0 | **Ratified**: TODO(RATIFICATION_DATE): original adoption date is unknown | **Last Amended**: 2026-11-07
