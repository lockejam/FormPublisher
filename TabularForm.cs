using FormPublisher.CustomAttributes;
using FormPublisher.Interfaces;
using iText.Forms;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FormPublisher
{
    /// <summary>
    /// Base class for models to inherit to enable the model to be read by the form publisher for tabular data.
    /// </summary>
    public class TabularForm : IFormPublisher, IPublish
    {
        /// <summary>
        /// Pass in FormSettings object with PDF file details and location.
        /// </summary>
        /// <param name="settings"></param>
        public TabularForm(FormSettings settings)
        {
            Settings = settings;
        }

        /// <summary>
        /// PDF file details and location object passed during class construction.
        /// </summary>
        [FormField(false)]
        public FormSettings Settings { get; protected set; }

        /// <summary>
        /// List of tabular data
        /// </summary>
        [FormField(false)]
        public IEnumerable<IDataLine> Items { get; set; }

        /// <summary>
        /// Method that reads model fields and iterates over those properties to assign 
        /// to form fields and returns form as byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] Publish()
        {
            // Get property information of this form and of the form's items.
            var fields = this.GetFormFields();
            var itemFields = Items.GetFormFields();

            // Get first page items
            var page1Items = itemFields.Take(Settings.FirstPageRowCount);

            // Divide remaining pages into chunks equal to the continuation page count
            var chunks = itemFields.Skip(Settings.FirstPageRowCount)
                                   .Select((item, index) => new { item, index })
                                   .GroupBy(x => x.index / Settings.ContinuationPageRowCount)
                                   .Select(x => x.Select(v => v.item))
                                   .ToList();

            // Insert first page of items at beginning
            chunks.Insert(0, page1Items);

            var sheetNumber = 1;
            var firstPass = true;
            using (var ms = new MemoryStream())
            {
                using (var document = new PdfDocument(new PdfWriter(ms)))
                {
                    var merger = new PdfMerger(document);

                    foreach (var chunk in chunks)
                    {
                        var bytes = CreateForm(fields, chunk, sheetNumber, chunks.Count, firstPass);

                        using (var byteStream = new MemoryStream(bytes))
                        {
                            using (var byteDoc = new PdfDocument(new PdfReader(byteStream)))
                            {
                                merger.Merge(byteDoc, 1, byteDoc.GetNumberOfPages());
                            }
                        }

                        firstPass = false;
                        sheetNumber++;
                    }

                    merger.Close();
                }

                return ms.ToArray();
            }
        }

        private byte[] CreateForm(IEnumerable<FormField> fields, IEnumerable<DataLine> dataLines, int sheetNumber, int numberOfSheets, bool firstPass)
        {
            using (var ms = new MemoryStream())
            {
                using (var reader = new PdfReader(firstPass ? Settings.FirstPageFilePath : Settings.ContinuationPageFilePath))
                {
                    using (var document = new PdfDocument(reader, new PdfWriter(ms)))
                    {
                        var acroForm = PdfAcroForm.GetAcroForm(document, false);

                        // iterate of form fields
                        foreach (var field in fields.Where(f => firstPass || (firstPass == f.IsInitial)))
                        {
                            if (field.IsNumberOfPages)
                            {
                                field.Value = numberOfSheets;
                            }

                            if (field.IsPageNumber)
                            {
                                field.Value = sheetNumber;
                            }

                            if (!string.IsNullOrEmpty(field.SheetSum))
                            {
                                field.Value = dataLines.SelectMany(line => line.FormFields)
                                                       .Where(f => f.Name == field.SheetSum && f.Value != null)
                                                       .Select(f => (decimal)f.Value)
                                                       .Sum();
                            }

                            field.SetField(acroForm);
                        }

                        // iterate over data fields
                        var index = 0;
                        var lineNumber = 1;
                        foreach (DataLine line in dataLines)
                        {
                            foreach (var field in line.FormFields)
                            {
                                if (field.IsLineNumber && !line.SkipLineNumber)
                                {
                                    field.Value = (lineNumber);
                                }

                                field.SetField(acroForm, index);
                            }

                            if (!line.SkipLineNumber)
                            {
                                lineNumber++;
                            }

                            index++;
                        }

                        foreach (var field in acroForm.GetFormFields())
                        {
                            field.Value.SetFieldName($"{field.Key}_sheet({sheetNumber})");
                        }
                    }
                }

                return ms.ToArray();
            }
        }


    }
}
