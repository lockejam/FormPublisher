# Repository Layout

Reviewed for the M3 packaging-readiness milestone.

## Decision

FormPublisher uses a conventional source/test layout before NuGet packaging:

```text
src/
  FormPublisher/
tests/
  FormPublisher.Tests/
docs/
```

The packageable library project lives at `src/FormPublisher/FormPublisher.csproj`.
The xUnit test project lives at `tests/FormPublisher.Tests/FormPublisher.Tests.csproj`
and references the library project directly.

## Rationale

- Keep packageable source separate from tests, docs, CI files, and generated artifacts.
- Give packaging, CI, and future example projects a stable project path before release workflows are added.
- Avoid coupling the layout decision to the later `PdfFormPublisher` rename.

## Deferred

- The project/package rename remains tracked separately under #66.
- Future example projects should be added under `examples/` when the M5 example work begins.
- The GitHub repository name can be decided separately from the source layout and package identity.
