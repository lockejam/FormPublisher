using FormPublisher.CustomAttributes;
using FormPublisher.Interfaces;
using iText.Forms;
using iText.Kernel.Pdf;
using System;
using System.IO;

namespace FormPublisher
{
    /// <summary>
    /// Base class for models to inherit to enable the model to be read by the form publisher.
    /// </summary>
    public class Form : IFormPublisher, IPublish
    {
        /// <summary>
        /// Pass in FormSettings object with PDF file details and location.
        /// </summary>
        /// <param name="settings"></param>
        public Form(string  filePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
            FilePath = filePath;
        }

        /// <summary>
        /// PDF file details and location object passed during class construction.
        /// </summary>
        [FormField(false)]
        public string FilePath{ get; protected set; }

        /// <summary>
        /// Method that reads model fields and iterates over those properties to assign 
        /// to form fields and returns form as byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] Publish()
        {
            var filePath = ValidateTemplatePath();

            // Get property information of this form and of the form's items.
            var fields = this.GetFormFields();

            using (var ms = new MemoryStream())
            {
                using (var reader = new PdfReader(filePath))
                {
                    using (var document = new PdfDocument(reader, new PdfWriter(ms)))
                    {
                        var acroForm = PdfAcroForm.GetAcroForm(document, false)
                            ?? throw new InvalidOperationException($"The PDF template '{filePath}' does not contain an AcroForm.");

                        // iterate of form fields
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
}