using PdfFormPublisher.Attributes;
using PdfFormPublisher.Interfaces;
using iText.Forms;
using iText.Kernel.Pdf;

namespace PdfFormPublisher;

/// <summary>
/// Base class for filling existing PDF forms that contain repeating rows.
/// </summary>
/// <remarks>
/// Inherit from this class for forms that have a first-page template and optional continuation
/// pages. Put row data in <see cref="Items"/> and configure row counts and template paths with
/// <see cref="FormSettings"/>.
/// </remarks>
public class TabularForm : IPdfFormPublisher, IPublish
{
    /// <summary>
    /// Creates a tabular form model with the PDF template and row settings to use.
    /// </summary>
    /// <param name="settings">The template paths and row counts used while publishing.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="settings"/> is null.</exception>
    public TabularForm(FormSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        Settings = settings;
    }

    /// <summary>
    /// The template paths and row counts used while publishing.
    /// </summary>
    [FormField(false)]
    public FormSettings Settings { get; protected set; }

    /// <summary>
    /// The rows that should be written to the PDF form.
    /// </summary>
    /// <remarks>
    /// Each item represents one row. The item's public properties are matched to row field names
    /// in the PDF, such as <c>Description.0</c>, <c>Description.1</c>, and so on.
    /// </remarks>
    [FormField(false)]
    public IEnumerable<IDataLine>? Items { get; set; }

    /// <summary>
    /// Fills the configured PDF templates with the model values and row data.
    /// </summary>
    /// <returns>The filled PDF as a byte array.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required settings are missing, row counts are invalid, or a template does not contain an AcroForm.
    /// </exception>
    /// <exception cref="FileNotFoundException">Thrown when a required PDF template file does not exist.</exception>
    public byte[] Publish()
    {
        var itemFields = GetValidatedItemFields();
        ValidatePublishState(itemFields.Count);

        // Get property information of this form and of the form's items.
        var fields = this.GetFormFields().ToList();

        List<IEnumerable<DataLine>> chunks =
        [
            itemFields.Take(Settings.FirstPageRowCount)
        ];

        if (itemFields.Count > Settings.FirstPageRowCount)
        {
            chunks.AddRange(itemFields.Skip(Settings.FirstPageRowCount)
                                      .Chunk(Settings.ContinuationPageRowCount));
        }

        var sheetNumber = 1;
        var firstPass = true;
        using (var ms = new MemoryStream())
        {
            using (var document = new PdfDocument(new PdfWriter(ms)))
            {
                var formCopier = new PdfPageFormCopier();

                foreach (var chunk in chunks)
                {
                    var bytes = CreateForm(fields, chunk, sheetNumber, chunks.Count, firstPass);

                    using (var byteStream = new MemoryStream(bytes))
                    {
                        using (var byteDoc = new PdfDocument(new PdfReader(byteStream)))
                        {
                            byteDoc.CopyPagesTo(1, byteDoc.GetNumberOfPages(), document, formCopier);
                        }
                    }

                    firstPass = false;
                    sheetNumber++;
                }
            }

            return ms.ToArray();
        }
    }

    private byte[] CreateForm(IEnumerable<FormField> fields, IEnumerable<DataLine> dataLines, int sheetNumber, int numberOfSheets, bool firstPass)
    {
        var templatePath = firstPass ? Settings.FirstPageFilePath : Settings.ContinuationPageFilePath;

        using (var ms = new MemoryStream())
        {
            using (var reader = new PdfReader(templatePath))
            {
                using (var document = new PdfDocument(reader, new PdfWriter(ms)))
                {
                    var acroForm = PdfAcroForm.GetAcroForm(document, false)
                        ?? throw new InvalidOperationException($"The PDF template '{templatePath}' does not contain an AcroForm.");

                    // iterate of form fields
                    foreach (var field in fields.Where(f => firstPass || (firstPass == f.IsInitial)))
                    {
                        if (field.IsNumberOfPages)
                        {
                            field.Value = numberOfSheets;
                        }

                        if (field.IsPageNumber)
                        {
                            field.Value = sheetNumber;
                        }

                        if (field.SheetSum is string sheetSumFieldName && sheetSumFieldName.Length > 0)
                        {
                            field.Value = CalculateSheetSum(dataLines, sheetSumFieldName);
                        }

                        field.SetField(acroForm);
                    }

                    // iterate over data fields
                    var index = 0;
                    var lineNumber = 1;
                    foreach (var line in dataLines)
                    {
                        foreach (var field in line.FormFields)
                        {
                            if (field.IsLineNumber && !line.SkipLineNumber)
                            {
                                field.Value = lineNumber;
                            }

                            field.SetField(acroForm, index);
                        }

                        if (!line.SkipLineNumber)
                        {
                            lineNumber++;
                        }

                        index++;
                    }

                    foreach (var field in acroForm.GetAllFormFields())
                    {
                        field.Value.SetFieldName($"{field.Key}_sheet({sheetNumber})");
                    }
                }
            }

            return ms.ToArray();
        }
    }

    private List<DataLine> GetValidatedItemFields()
    {
        if (Items is null)
        {
            throw new InvalidOperationException("Items must be set before publishing.");
        }

        return Items.GetFormFields().ToList();
    }

    private void ValidatePublishState(int itemCount)
    {
        if (Settings is null)
        {
            throw new InvalidOperationException("Settings must be set before publishing.");
        }

        if (Settings.FirstPageRowCount < 0)
        {
            throw new InvalidOperationException("Settings.FirstPageRowCount cannot be negative.");
        }

        if (Settings.ContinuationPageRowCount < 0)
        {
            throw new InvalidOperationException("Settings.ContinuationPageRowCount cannot be negative.");
        }

        ValidateTemplatePath(Settings.FirstPageFilePath, nameof(Settings.FirstPageFilePath));

        if (itemCount > Settings.FirstPageRowCount)
        {
            if (Settings.ContinuationPageRowCount <= 0)
            {
                throw new InvalidOperationException("Settings.ContinuationPageRowCount must be greater than zero when items exceed the first-page row count.");
            }

            ValidateTemplatePath(Settings.ContinuationPageFilePath, nameof(Settings.ContinuationPageFilePath));
        }
    }

    private static decimal CalculateSheetSum(IEnumerable<DataLine> dataLines, string sheetSumFieldName)
    {
        decimal total = 0;

        foreach (var formField in dataLines.SelectMany(line => line.FormFields).Where(f => f.Name == sheetSumFieldName))
        {
            if (formField.Value is null)
            {
                continue;
            }

            if (formField.Value is not decimal value)
            {
                throw new InvalidOperationException($"SheetSum field '{sheetSumFieldName}' must contain decimal values.");
            }

            total += value;
        }

        return total;
    }

    private static void ValidateTemplatePath(string? filePath, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new InvalidOperationException($"{propertyName} must be set before publishing.");
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The PDF template '{filePath}' was not found.", filePath);
        }
    }
}
