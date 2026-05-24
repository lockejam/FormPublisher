namespace PdfFormPublisher;

/// <summary>
/// Defines the PDF templates and row counts used by a tabular form.
/// </summary>
public class FormSettings
{
    /// <summary>
    /// How many item rows fit on the first page template.
    /// </summary>
    public int FirstPageRowCount { get; set; }

    /// <summary>
    /// How many item rows fit on each continuation page template.
    /// </summary>
    /// <remarks>
    /// This value is required when the item list can overflow the first page.
    /// </remarks>
    public int ContinuationPageRowCount { get; set; }

    /// <summary>
    /// The file path to the PDF template used for the first page.
    /// </summary>
    public string? FirstPageFilePath { get; set; }

    /// <summary>
    /// The file path to the PDF template used for continuation pages.
    /// </summary>
    /// <remarks>
    /// This path is only needed when the item list can overflow the first page.
    /// </remarks>
    public string? ContinuationPageFilePath { get; set; }
}
