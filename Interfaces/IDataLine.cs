namespace FormPublisher.Interfaces
{
    /// <summary>
    /// Interface for tubular data to be read by PDF form.
    /// </summary>
    public interface IDataLine : IFormPublisher
    {
        /// <summary>
        /// Skip the line count for this row.  Usually this might pertain to 
        /// separation lines or lines intentionally left blank.
        /// </summary>
        bool SkipLineNumber { get; set; }
    }
}
