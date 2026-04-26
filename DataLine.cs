using System.Collections.Generic;

namespace FormPublisher
{
    /// <summary>
    /// Internal class that disseminates the inidividual model properties and it's attributes into a collection.
    /// </summary>
    internal class DataLine
    {
        /// <summary>
        /// If true will not assign value to field and will not increase the line number.
        /// </summary>
        public bool SkipLineNumber { get; set; }
        
        /// <summary>
        /// Collection of field objects.
        /// </summary>
        public IEnumerable<FormField> FormFields { get; set; } = [];
    }
}
