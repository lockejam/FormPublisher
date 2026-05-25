# Package Metadata

Reviewed for the M3 packaging-readiness milestone.

## Package Identity

The packageable library project is `src/PdfFormPublisher/PdfFormPublisher.csproj`.

Current NuGet metadata:

| Property | Value |
| --- | --- |
| `PackageId` | `PdfFormPublisher` |
| `VersionPrefix` | `0.1.0` |
| `Authors` | `lockejam` |
| `Description` | `A small, focused C# library for filling existing PDF forms from model objects.` |
| `PackageLicenseExpression` | `AGPL-3.0-only` |
| `PackageProjectUrl` | `https://github.com/lockejam/PdfFormPublisher` |
| `RepositoryType` | `git` |
| `RepositoryUrl` | `https://github.com/lockejam/PdfFormPublisher` |
| `PackageTags` | `pdf;forms;pdf-forms;acroform;itext` |
| `PackageReadmeFile` | `README.md` |

The root README is included in the package so NuGet can display package documentation.

## Debugging Metadata

Package builds produce a NuGet package and a symbol package:

- `IncludeSymbols` is enabled.
- `SymbolPackageFormat` is `snupkg`.
- `DebugType` is `portable`.
- `PublishRepositoryUrl` and `EmbedUntrackedSources` are enabled for Source Link metadata.
- `Microsoft.SourceLink.GitHub` is referenced as a private build dependency.
- `ContinuousIntegrationBuild` is enabled when building in GitHub Actions.

The symbol package is intended for NuGet.org's symbol server and contains portable PDBs for consumer debugging.

## CI Package Artifacts

The GitHub Actions build restores, builds, tests, and packs the library in Release configuration.
Package artifacts are written to `artifacts/packages` and uploaded as the
`pdf-form-publisher-packages` workflow artifact.

## Consumer Smoke Test

Run the package smoke test from the repository root:

```powershell
.\scripts\package-smoke-test.ps1
```

The smoke test packs `PdfFormPublisher`, restores a separate consumer console project
from the local package output plus NuGet.org, and runs that project against a generated
fillable PDF template.

## Installation And Upgrade Guidance

Consumer installation and upgrade guidance is documented in
[package-consumption.md](package-consumption.md). Until release publishing is
implemented, consumers should install from a local package output or CI package
artifact and keep NuGet.org configured for transitive dependencies.

## Stream-Based Consumers

Package consumers can publish without template file paths by passing readable template
streams to `Publish`. This supports ASP.NET, cloud storage, embedded resources, and
other workflows where the PDF template is not a local file.

Simple forms support:

```csharp
byte[] pdfBytes = form.Publish(templateStream);
form.Publish(templateStream, outputStream);
```

Tabular forms keep row counts in `TabularPdfFormSettings` and accept first-page and continuation
template streams:

```csharp
byte[] pdfBytes = form.Publish(firstPageTemplateStream, continuationTemplateStream);
form.Publish(firstPageTemplateStream, continuationTemplateStream, outputStream);
```

Caller-provided streams remain open. Template streams are read from their current
position, and output streams are written at their current position.

## Versioning

Use semantic versioning.

- Keep the package on `0.x` while the public API is still settling.
- Use prerelease labels, such as `0.1.0-alpha.1`, for early published packages.
- Reserve `1.0.0` for the first release where the public API is considered stable.

## Deferred

- Release publishing automation remains tracked separately under #20.
