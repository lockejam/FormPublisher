# Dependency Notes

This project currently depends on `itext` for PDF form reading, field assignment, and PDF merging.

## Current package set

- `itext` `9.6.0`
- `itext.bouncy-castle-adapter` `9.6.0`

`ClosedXML` was removed because the repository does not currently contain any code paths that read or write Excel files.

## iText strategy

`FormPublisher` currently uses `itext` `9.6.0` with `itext.bouncy-castle-adapter` `9.6.0`.

Before implementing signature support, review the active iText package line and confirm that the chosen version is still the right fit for:

- visible signature placement
- certificate-based digital signing
- package maintenance status
- any API changes that would affect `FormPublisher`

## Licensing note

iText uses a dual-licensing model:

- AGPL for open-source use
- commercial licensing for use cases that cannot comply with AGPL obligations

That matters for `FormPublisher` because signature support is planned as a consumer-facing feature and may be used in closed-source or internal business applications.

Before shipping NuGet packages or signature examples, confirm that the intended distribution model is compatible with iText's license terms.

References:

- NuGet Gallery for `itext`: https://www.nuget.org/packages/itext/9.6.0
- NuGet Gallery for `itext.bouncy-castle-adapter`: https://www.nuget.org/packages/itext.bouncy-castle-adapter/9.6.0
- iText AGPL licensing overview: https://itextpdf.com/how-buy/AGPLv3-license
- iText licensing overview: https://itextpdf.com/how-buy
