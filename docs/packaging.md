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
| `PackageProjectUrl` | `https://github.com/lockejam/FormPublisher` |
| `RepositoryType` | `git` |
| `RepositoryUrl` | `https://github.com/lockejam/FormPublisher` |
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

## Versioning

Use semantic versioning.

- Keep the package on `0.x` while the public API is still settling.
- Use prerelease labels, such as `0.1.0-alpha.1`, for early published packages.
- Reserve `1.0.0` for the first release where the public API is considered stable.

## Deferred

- Release publishing automation remains tracked separately under #20.
- Consumer package-install smoke testing remains tracked separately under #21.
- Installation and upgrade guidance remains tracked separately under #22.
