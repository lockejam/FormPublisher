namespace FormPublisher.CustomAttributes;

/// <summary>
/// Mark fields to be read by the PDF reader.  All fields are automatically ready but
/// this allows for their specific roles to be attributed to each field or for the field
/// to be excluded altogether
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FormFieldAttribute : Attribute
{
    /// <summary>
    /// Constructor.  Specify if field should be included when reading to PDF output.
    /// </summary>
    /// <param name="include"></param>
    public FormFieldAttribute(bool include = true)
    {
        IncludeField = include;
    }

    /// <summary>
    /// The name of the field the property relates to if property name is different.
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// Format for string conversion.
    /// </summary>
    public string? DataFormat { get; set; }

    /// <summary>
    /// Check whether property should be included in PDF output.
    /// </summary>
    public bool IncludeField { get; protected set; }
}
