using FormPublisher.CustomAttributes;
using FormPublisher.Interfaces;
using iText.Forms;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;

namespace FormPublisher;

/// <summary>
/// Base class for models to inherit to enable the model to be read by the form publisher for tabular data.
/// </summary>
public class TabularForm : IFormPublisher, IPublish
{
    /// <summary>
    /// Pass in FormSettings object with PDF file details and location.
    /// </summary>
    /// <param name="settings"></param>
    public TabularForm(FormSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        Settings = settings;
    }

    /// <summary>
    /// PDF file details and location object passed during class construction.
    /// </summary>
    [FormField(false)]
    public FormSettings Settings { get; protected set; }

    /// <summary>
    /// List of tabular data
    /// </summary>
    [FormField(false)]
    public IEnumerable<IDataLine>? Items { get; set; }

    /// <summary>
    /// Method that reads model fields and iterates over those properties to assign
    /// to form fields and returns form as byte array.
    /// </summary>
    /// <returns></returns>
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
                var merger = new PdfMerger(document);

                foreach (var chunk in chunks)
                {
                    var bytes = CreateForm(fields, chunk, sheetNumber, chunks.Count, firstPass);

                    using (var byteStream = new MemoryStream(bytes))
                    {
                        using (var byteDoc = new PdfDocument(new PdfReader(byteStream)))
                        {
                            merger.Merge(byteDoc, 1, byteDoc.GetNumberOfPages());
                        }
                    }

                    firstPass = false;
                    sheetNumber++;
                }

                merger.Close();
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
