using System;
using System.Collections.Generic;
using System.Text;

namespace FormPublisher
{
    /// <summary>
    ///  PDF File information
    /// </summary>
    public class FormSettings
    {
        /// <summary>
        /// Number of rows in first page.
        /// </summary>
        public int FirstPageRowCount { get; set; }
        /// <summary>
        /// Number of rows on continuation page.  If first page is to be reused for 
        /// item overflow then first page will be used.
        /// </summary>
        public int ContinuationPageRowCount { get; set; }

        /// <summary>
        /// File path of first page.
        /// </summary>
        public string FirstPageFilePath { get; set; }

        /// <summary>
        /// File path of continuation page.
        /// </summary>
        public string ContinuationPageFilePath { get; set; }
    }
}
