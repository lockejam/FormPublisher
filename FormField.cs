using iText.Forms;
using iText.Forms.Fields;

namespace FormPublisher
{
    /// <summary>
    /// Internal class that will break down property information to target and assign values to form fields.
    /// <see cref="CustomAttributes.FormFieldAttribute"></see>
    /// </summary>
    internal class FormField
    {
        public string Name { get; set; } = string.Empty;
        public object? Value { get; set; }
        public bool IsInitial { get; set; }
        public string? SheetSum { get; set; }
        public string? DataFormat { get; set; }
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
            ArgumentNullException.ThrowIfNull(acroForm);

            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new InvalidOperationException("Form field name must be set before assignment.");
            }

            var targetFieldName = GetTargetFieldName(index);
            var acroField = acroForm.GetField(targetFieldName)
                ?? throw new InvalidOperationException($"The PDF field '{targetFieldName}' was not found.");

            if (Value is string[] selectedValues)
            {
                SetChoiceFieldValue(acroField, targetFieldName, selectedValues);
                return;
            }

            if (!TryGetScalarFieldValue(out var fieldValue))
            {
                return;
            }

            acroField.SetValue(fieldValue);
        }

        private string GetTargetFieldName(int? index)
        {
            return index is int value ? $"{Name}.{value}" : Name;
        }

        private bool TryGetScalarFieldValue(out string fieldValue)
        {
            if (Value is bool boolValue)
            {
                fieldValue = "On";
                return boolValue;
            }

            if (Value is IFormattable formattable)
            {
                fieldValue = formattable.ToString(DataFormat, null);
                return true;
            }

            fieldValue = Value?.ToString() ?? string.Empty;
            return true;
        }

        private static void SetChoiceFieldValue(PdfFormField acroField, string targetFieldName, string[] selectedValues)
        {
            if (acroField is not PdfChoiceFormField choiceField)
            {
                throw new InvalidOperationException($"The PDF field '{targetFieldName}' must be a choice field when assigning list selections.");
            }

            choiceField.SetListSelected(selectedValues);
        }
    }
}
