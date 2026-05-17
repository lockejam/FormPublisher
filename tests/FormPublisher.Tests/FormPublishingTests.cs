namespace FormPublisher.Tests;

public sealed class FormPublishingTests
{
    [Fact]
    public void Publish_maps_model_values_to_pdf_fields()
    {
        var templatePath = TestPdfTemplates.CreateSimpleTemplate();
        var form = new SimpleFormModel(templatePath)
        {
            Title = "Supply Request",
            Alias = "FP-001",
            Date = new DateTime(2026, 5, 3),
            Choice = ["Blue"]
        };

        var pdfBytes = form.Publish();

        var fields = TestPdfTemplates.ReadFieldValues(pdfBytes);
        Assert.Equal("Supply Request", fields["Title"]);
        Assert.Equal("FP-001", fields["RENAMED"]);
        Assert.Equal("2026-05-03", fields["Date"]);
        Assert.Equal("Blue", fields["Choice"]);
    }

    [Fact]
    public void Publish_maps_bool_values_to_checkbox_states()
    {
        var templatePath = TestPdfTemplates.CreateCheckboxTemplate();
        var form = new CheckboxFormModel(templatePath)
        {
            DefaultChecked = true,
            CustomChecked = true,
            DefaultUnchecked = false
        };

        var pdfBytes = form.Publish();

        var fields = TestPdfTemplates.ReadFieldValues(pdfBytes);
        Assert.Equal("Yes", fields["DefaultChecked"]);
        Assert.Equal("On", fields["CustomChecked"]);
        Assert.Equal("Off", fields["DefaultUnchecked"]);
    }
}
