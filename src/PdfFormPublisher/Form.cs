using PdfFormPublisher.Attributes;
using PdfFormPublisher.Interfaces;
using iText.Forms;
using iText.Kernel.Pdf;

namespace PdfFormPublisher;

/// <summary>
/// Base class for filling one existing fillable PDF form from a C# model.
/// </summary>
/// <remarks>
/// Inherit from this class and add public properties whose names match the PDF field names.
/// Use <see cref="Attributes.FormFieldAttribute"/> when a property needs a different
/// PDF field name, a display format, or should be ignored.
/// </remarks>
public class Form : IPdfFormPublisher, IPublish
{
    /// <summary>
    /// Creates a form model for the PDF template at the supplied file path.
    /// </summary>
    /// <param name="filePath">The path to the existing PDF form template.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is blank.</exception>
    public Form(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        FilePath = filePath;
    }

    /// <summary>
    /// The path to the existing PDF form template used by this model.
    /// </summary>
    [FormField(false)]
    public string FilePath { get; protected set; }

    /// <summary>
    /// Fills the PDF template with this model's public property values.
    /// </summary>
    /// <returns>The filled PDF as a byte array.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the template path is missing or the PDF does not contain an AcroForm.
    /// </exception>
    /// <exception cref="FileNotFoundException">Thrown when the PDF template file does not exist.</exception>
    public byte[] Publish()
    {
        var filePath = ValidateTemplatePath();

        // Get property information of this form.
        var fields = this.GetFormFields().ToList();

        using (var ms = new MemoryStream())
        {
            using (var reader = new PdfReader(filePath))
            {
                using (var document = new PdfDocument(reader, new PdfWriter(ms)))
                {
                    var acroForm = PdfAcroForm.GetAcroForm(document, false)
                        ?? throw new InvalidOperationException($"The PDF template '{filePath}' does not contain an AcroForm.");

                    foreach (var field in fields)
                    {
                        field.SetField(acroForm);
                    }
                }
            }

            return ms.ToArray();
        }
    }

    private string ValidateTemplatePath()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
        {
            throw new InvalidOperationException("FilePath must be set before publishing.");
        }

        if (!File.Exists(FilePath))
        {
            throw new FileNotFoundException($"The PDF template '{FilePath}' was not found.", FilePath);
        }

        return FilePath;
    }
}
