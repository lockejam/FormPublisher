# FormPublisher

A project for writing to PDF files using a simple POCO as an interface.

## Basics

### Form.cs

Base class the model must inherit in order to be read by the form publisher.  
This class provides a few essentials:  

> `Property Name` | `Type`

 - `FilePath` | `string`  
    File path of the pdf document to produced.  
 - `Publish` | `Method()`  
    Method that reads model fields and iterates over those properties to 
    assign to form fields and returns form as byte array.  
    >`return File(yourFormObject.Publish(), "application/pdf");`


### TabularForm.cs

Like [`Form.cs`](#form.cs) the model must inherit in order to be read by the form publisher but with added functionality specific to forms with 
tabular data.  
This class provides a few essentials:  

> `Property Name` | `Type`

 - `Settings` | `FormSettings`  
    The details for the PDF file(s) to be read to.  More in the 
    [`FormSettings`](#formsettings.cs) section.
 - `Items` | `IEnumerable<IDataLine>`  
    List of tabular data.  More in the [`IDataLine`](#idataline.cs) section.
 - `Publish` | `Method()`  
    Method that reads model fields and iterates over those properties to 
    assign to form fields and returns form as byte array.  
    >`return File(yourFormObject.Publish(), "application/pdf");`

### FormSettings.cs

PDF file information.

> `Property Name` | `Type`

 - `FirstPageRowCount` | `int`  
    Number of rows for tabular data in first page.
 - `ContinuationPageRowCount` | `int`  
    Number of rows for tabular data on continuation page.  If first page
    is to be reused for item overflow then first page will be used.
 - `FirstPageFilePath` | `string`  
    File path of first page.
 - `ContinuationPageFilePath` | `string`  
    File path of the continuation page in the case of tabular data.


### IDataLine.cs

Interface for tabular data to be read by the PDF Form. This is required 
for tabular data that is assigned to the `Items` property for the `Form` 
object. Requires the implementation of `SkipLineNumber` property.

> `Property Name` | `Type` | default value 

 - `SkipLineNumber` | `bool` | false   
    Skip the line count for this row.  Usually this might pertain to 
    separation lines or lines intentionally left blank within your 
    tabular data.  This will ignore assigning the line count to the 
    property marked with the 
    [`FormFieldAttribute`](#formfieldattribute) with the `IsLineNumber`
    property set to `true`.  If there is no property marked as the line
    number field then this property can be set to true.

### FormFieldAttribute.cs

Mark fields to be read by the PDF reader.  All fields are automatically 
read but this allows for their specific roles to be attributed to each
field or for the field to be excluded altogether.

> `Property Name` | `Type` | default value 

 - `FieldName` | `string` | `null`  
    The name of the field the property relates to if property name is 
    different.  FieldName is case sensitive.  
    ```csharp
    [FormField(FieldName = "PDF_Field_Name")"
    public string PropertyName {get; set;
    ```  
 - `DataFormat` | `string` | `null`  
    Format for string conversion
    ```csharp
    FormField(DataFormat = "yyyyMMdd")]
    public DateTime? DATE_OF_ISSUE { get; set; }
    ```  
 - `IncludeField` | `bool` | `true`  
    Check whether property should be included in PDF output.  This is 
    set during class constructor.
    ```csharp
    [FormField(false)]
    public bool SkipLineNumber { get; set; }
    ```  


### DataLineAttribute.cs

Mark DataLine fields or Form fields related to DataLine fields with attributes that require special treatment.

> `Property Name` | `Type` | default value 

 - `IsInitial` | `bool` | `false`  
    Set to `true` if the field only appears on the first page. Failing 
    to do so may result in a `NullRefenceException`.
    ```csharp
    [FormField(IsInitial = true)]
    public string HAND_RECEIPT_NUMBER { get; set; }
    ```  
 - `SheetSum` | `string` | `null`  
    Name of the [`IDataLine`](#idataline.cs) field that should be used to 
    quantify this fields.
    ```csharp
    [FormField(SheetSum = "TOTAL_COST", DataFormat = "c")]
    public bool SHEET_TOTAL { get; set; }
    ```
    When `Publish` is called the `Form.Items` will be divided up 
    according to how many can fit on each sheet and `Sum` is called on 
    the list of items.
    **Important: type must be of type `decimal` for both fields!**  
 - `IsLineNumber` | `bool` | `false`  
    The line number for an item if not assigned to manually.  This is 
    calculated when the publisher iterates over items.   
    ```csharp
    [FormField(false)]
    public bool SkipLineNumber { get; set; }
    ```
 - `IsPageNumber` | `bool` | `false`  
    Page Number.  This will be assigned for each page produced.
    ```csharp
    [FormField(IsPageNumber = true)]
    public int PAGE_NUMBER { get; set; }
    ```
 - `IsNumberOfPages` | `bool` | `false`  
    Total number of pages.  The field marked will be assigned after 
    publisher calculates the number of pages based on number of items
    ```csharp
    [FormField(IsNumberOfPages = true)]
    public int NUMBER_OF_PAGES { get; set; }
    ```

## Simple Example 

Below is an example of how to use the FormPublisher project.

1. Build model that inherit from [`TabularForm`](#form.cs).
2. Build model for tablular data that implements the
   [`IDataLine`](#idataline.cs) `interface`.
3. Pass in [`FormSettings`](#formsettings.cs`) to model.
4. Assign fields.
5. Call `Publish()`.

### MyToyList.cs
```csharp
using FormPublisher;
using FormPublisher.CustomAttributes;
using System;

// We inherit from `TabularForm` and not `Form` because our object will contain a list of items.
public class MyToyList : TabularForm
{

    public MyPdfFile(FormSettings settings) : base(settings) {}

    // property name matches field in Pdf file
    public string List_Name { get; set; }

    // property name differs from field name in pdf file
    [FormField(FieldName = "List_Version_NO")"
    public int VersionNumber { get; set; }

    // field is calculated per sheet and sums the "TOY_COST" fields of our Items
    [DataLine(SheetSum = "TOY_COST")]
    [FormField(DataFormat = "c")]
    public bool SHEET_TOTAL { get; set; }

    // `Items` is inherited

    // `Publish` is inherited
}
```

### MyToy.cs
```csharp
using FormPublisher.CustomAttributes;
using FormPublisher.Interfaces;
using System;

public MyToy : IDataLine
{
    [DataLine(IsLineNumber = true)]
    public int? ITEM_NUMBER { get; set; }
    
    public string DESCRIPTION { get; set; }

    [FormField(DataFormat = "yyyyMMdd")]
    public DateTime? PURCHASE_DATE { get; set; }

    [FormField(DataFormat = "c")]
    public decimal? TOY_COST { get; set; }

    // must be implemented but doesn't appear in Pdf form
    [FormField(false)]
    public bool SkipLineNumber { get; set; }
}
```

### ExportController.cs
```csharp
public class FormController : Controller 
{
    public FileResult Export() 
    {
        // get file details
        var formSettings = new FormSettings  
        {
            FirstPageRowCount = 14,
            ContinuationPageRowCount = 18,
            FirstPageFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"\Documents\toys.pdf"),
            ContinuationPageFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"\Documents\toys_cont.pdf")
        }

        // create list of toys
        var toys = new List<MyDataLine>
        {
            new MyToy 
            { 
                DESCRIPTION = "Teddy Bear",
                PURCHASE_DATE = new DateTime(2008, 3, 1, 7, 0, 0),
                TOY_COST = 12.99
            },
            new MyToy 
            { 
                DESCRIPTION = "Space Ship",
                PURCHASE_DATE = new DateTime(2012, 6, 3, 8, 0, 0),
                TOY_COST = 19.99
            },
            new MyToy 
            { 
                DESCRIPTION = "Race Car",
                PURCHASE_DATE = new DateTime(2016, 11, 19, 4, 0, 0),
                TOY_COST = 8.99
            }
        }

        // create form object
        var form = new MyToyList 
        {
            List_Name = "Toy Box",
            VersionNumber = 2,
            Items = toys
        };
        
        // output pdf file 
        return File(MyToyList.Publish(), "application/pdf");
    }
}

```