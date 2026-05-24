namespace FormPublisher;

/// <summary>
/// Internal class that disseminates the individual model properties and their attributes into a collection.
/// </summary>
internal class DataLine
{
    /// <summary>
    /// If true will not assign value to field and will not increase the line number.
    /// </summary>
    public bool SkipLineNumber { get; init; }

    /// <summary>
    /// Collection of field objects.
    /// </summary>
    public IReadOnlyList<FormField> FormFields { get; init; } = [];
}
