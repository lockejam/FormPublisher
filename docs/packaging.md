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

## Versioning

Use semantic versioning.

- Keep the package on `0.x` while the public API is still settling.
- Use prerelease labels, such as `0.1.0-alpha.1`, for early published packages.
- Reserve `1.0.0` for the first release where the public API is considered stable.

## Deferred

- Symbols and source-link metadata remain tracked separately under #18.
- CI pack validation remains tracked separately under #19.
- Release publishing automation remains tracked separately under #20.
- Consumer package-install smoke testing remains tracked separately under #21.
- Installation and upgrade guidance remains tracked separately under #22.
