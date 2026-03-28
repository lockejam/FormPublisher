using System;
using System.Collections.Generic;
using System.Text;

namespace FormPublisher.CustomAttributes
{
    /// <summary>
    /// Mark DataLine fields or Form fields related to DataLine fields with attributes that require special treatment.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DataLineAttribute : Attribute
    {

        /// <summary>
        /// Set to <code>true</code> if the field only appears on the first page.
        /// Failing to do so may result in a NullRefenceException
        /// </summary>
        public bool IsInitial { get; set; }

        /// <summary>
        /// Name of the IDataLine field that should be used to quantify this fields.
        /// </summary>
        public string SheetSum { get; set; }

        /// <summary>
        /// The line number for an item.  This is calculated when the publisher iterates over items.
        /// </summary>
        public bool IsLineNumber { get; set; }

        /// <summary>
        /// Page Number.  This will be assigned for each page produced.
        /// </summary>
        public bool IsPageNumber { get; set; }

        /// <summary>
        /// Total number of pages.  The filed marked will be assigned after publisher calculates the 
        /// number of pages based on number of items.
        /// </summary>
        public bool IsNumberOfPages { get; set; }
    }
}
