using iText.Forms;
using iText.Forms.Fields;
using System;
using System.Collections.Generic;
using System.Text;

namespace FormPublisher
{
    /// <summary>
    /// Internal class that will break down property information to target and assign values to form fields.
    /// <see cref="CustomAttributes.FormFieldAttribute"></see>
    /// </summary>
    internal class FormField
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public bool IsInitial { get; set; }
        public string SheetSum { get; set; }
        public string DataFormat { get; set; }
        public bool IsLineNumber { get; set; }
        public bool IsPageNumber { get; set; }
        public bool IsNumberOfPages { get; set; }

        /// <summary>
        /// Set field value to associated PdfAcroForm field.  For data fields the index parameter 
        /// is required in order to target correct field.
        /// </summary>
        /// <param name="acroForm"></param>
        /// <param name="index"></param>
        public void SetField(PdfAcroForm acroForm, int? index = null)
        {

            if (index != null)
            {
                Name += $".{index}";
            }
            var acroField = acroForm.GetField(Name);

            string fieldValue;
            if (Value is DateTime)
            {
                fieldValue = ((DateTime)Value).ToString(DataFormat);
            }
            else if (Value is bool)
            {
                if (!(bool)Value)
                {
                    return;
                }

                fieldValue = "On";
            }
            else if (Value is decimal)
            {
                fieldValue = ((decimal)Value).ToString(DataFormat);
            }
            else
            {
                fieldValue = Value?.ToString() ?? "";
            }

            if (Value is string[])
            {
                ((PdfChoiceFormField)acroField).SetListSelected(new string[] { fieldValue });
            }
            else
            {
                acroField.SetValue(fieldValue);
            }
        }
    }
}
