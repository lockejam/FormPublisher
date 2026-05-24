namespace FormPublisher.CustomAttributes;

/// <summary>
/// Customizes how a model property maps to a PDF form field.
/// </summary>
/// <remarks>
/// Properties are included by default. Use this attribute to ignore a property, point it
/// at a differently named PDF field, or format its value before publishing.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public class FormFieldAttribute : Attribute
{
    /// <summary>
    /// Creates a field mapping attribute.
    /// </summary>
    /// <param name="include">
    /// <see langword="true"/> to include the property in PDF output; <see langword="false"/> to ignore it.
    /// </param>
    public FormFieldAttribute(bool include = true)
    {
        IncludeField = include;
    }

    /// <summary>
    /// The PDF field name to use when it differs from the property name.
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// The format string used for dates, numbers, and other formattable values.
    /// </summary>
    /// <remarks>
    /// For example, use <c>yyyy-MM-dd</c> to write a date in year-month-day format.
    /// </remarks>
    public string? DataFormat { get; set; }

    /// <summary>
    /// Whether the property should be written to the PDF form.
    /// </summary>
    public bool IncludeField { get; protected set; }
}
