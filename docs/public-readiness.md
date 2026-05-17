# Public Repository Readiness Checklist

Reviewed for the M2.5 public-readiness milestone.

## Status

The repository can be made public after the M2.5 public-readiness PR is merged and the final visibility change is confirmed in GitHub.

## Completed Checks

- A root `LICENSE` file is present.
- The repository license is documented as AGPL-3.0-only.
- iText dependency licensing is documented in `README.md` and `docs/dependencies.md`.
- `SECURITY.md` explains how security concerns should be reported.
- `CONTRIBUTING.md` explains local setup, test commands, PR expectations, and style notes.
- GitHub Actions build/test validation is configured.
- README states that the project is not packaged on NuGet yet.
- README states that the project may be renamed before the first NuGet package.
- Repository description and topics are set in GitHub.
- Tracked files were scanned for common secret names and generated artifact paths.

## Intentionally Deferred

- NuGet packaging and release metadata stay in M3.
- The project rename to `PdfFormPublisher` stays in M3 under #66.
- Remaining M2 test-foundation cleanup can be handled separately from public visibility.

## Final Visibility Step

After the public-readiness PR is merged, switch the repository visibility from private to public in GitHub.
