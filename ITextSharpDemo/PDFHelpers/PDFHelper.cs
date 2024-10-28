using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.pdf.parser;
//using static System.Net.Mime.MediaTypeNames;

namespace ITextSharpDemo.PDFHelpers
{
    public static class PDFHelper
    {
        public static void CreatePDF()
        {
            // Define the output file path
            string filePath = "sample.pdf";

            // Create a document instance
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);

            // Use a FileStream to write the file
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                // Bind the PDF writer to the document and the stream
                PdfWriter writer = PdfWriter.GetInstance(document, stream);

                // Open the document to enable writing
                document.Open();

                // Add content to the PDF (a paragraph in this example)
                document.Add(new Paragraph("Hello, NameField !"));
                // Adding an image
                Image image = Image.GetInstance("D:\\dogs.jpg");
                //document.Add(image);

                // Adding a table with 3 columns
                //PdfPTable table = new PdfPTable(3);
                //table.AddCell("Cell 1");
                //table.AddCell("Cell 2");
                //table.AddCell("Cell 3");
                //document.Add(table);

                // Close the document
                document.Close();
            }
        }

        public static void CreateFormFieldsPDF() {
            string outputFilePath = "output_with_form_fields.pdf";

            // Create a new PDF document
            using (FileStream stream = new FileStream(outputFilePath, FileMode.Create))
            {
                Document document = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(document, stream);
                document.Open();

                // Add a Text Field
                TextField nameField = new TextField(writer, new iTextSharp.text.Rectangle(100, 700, 300, 730), "NameField");
                nameField.Text = "Enter your name here";
                writer.AddAnnotation(nameField.GetTextField());

                // Add a Checkbox
                RadioCheckField checkBoxField = new RadioCheckField(writer, new iTextSharp.text.Rectangle(100, 650, 120, 670), "CheckBoxField", "Yes");
                checkBoxField.CheckType = RadioCheckField.TYPE_CHECK;
                checkBoxField.Checked = true; // Set to true if you want the box checked by default
                writer.AddAnnotation(checkBoxField.CheckField);

                // Add a Radio Button Group
                RadioCheckField radioButton1 = new RadioCheckField(writer, new iTextSharp.text.Rectangle(100, 600, 120, 620), "RadioGroup", "Option1");
                radioButton1.CheckType = RadioCheckField.TYPE_CIRCLE;
                writer.AddAnnotation(radioButton1.RadioField);

                RadioCheckField radioButton2 = new RadioCheckField(writer, new iTextSharp.text.Rectangle(150, 600, 170, 620), "RadioGroup", "Option2");
                radioButton2.CheckType = RadioCheckField.TYPE_CIRCLE;
                writer.AddAnnotation(radioButton2.RadioField);

                
                document.Close();
            }

            Console.WriteLine("PDF with form fields created at " + outputFilePath);

        }

        public static void EditByOverLayingPDF()
        {
            string inputFilePath = "sample.pdf";
            string outputFilePath = "output.pdf";

            // Open the existing PDF document
            using (PdfReader reader = new PdfReader(inputFilePath))
            {
                // Create a FileStream to output the modified PDF
                using (FileStream stream = new FileStream(outputFilePath, FileMode.Create))
                {
                    // PdfStamper writes to an existing PDF
                    using (PdfStamper stamper = new PdfStamper(reader, stream))
                    {
                        // Get the content layer for editing (e.g., on the first page)
                        PdfContentByte content = stamper.GetOverContent(1);

                        // Set font and size for the added text
                        BaseFont font = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                        content.SetFontAndSize(font, 12);

                        // Set text position and add text to the PDF
                        content.BeginText();
                        content.SetTextMatrix(100, 500); // (x, y) coordinates
                        content.ShowText("This is added text on the first page!");
                        content.EndText();

                        // Example: Add an image (optional)
                        Image img = Image.GetInstance("D:\\user-pic.jpg");
                        img.SetAbsolutePosition(100, 600); // position on the page
                        content.AddImage(img);
                    }
                }
            }

            Console.WriteLine("PDF edited successfully and saved as " + outputFilePath);
        }

        public static void EditWithFormFieldPDF()
        {
            string inputFilePath = "P1.pdf";
            string outputFilePath = "output.pdf";

            // Open the existing PDF document
            using (PdfReader reader = new PdfReader(inputFilePath))
            {
                // Output stream for the modified PDF
                using (FileStream stream = new FileStream(outputFilePath, FileMode.Create))
                {
                    // PdfStamper to modify the PDF
                    using (PdfStamper stamper = new PdfStamper(reader, stream))
                    {
                        // Access the form fields
                        AcroFields formFields = stamper.AcroFields;

                        // Modify specific fields by their names
                        Image image = Image.GetInstance("D:\\dogs.jpg");
                        Image img = Image.GetInstance("D:\\user-pic.jpg");


                        //formFields.SetField("FlagT_A[0]", "Amr Salah");
                        PushbuttonField button = stamper.AcroFields.GetNewPushbuttonFromField("FlagT_A[0]");
                        button.Image = image;
                        stamper.AcroFields.ReplacePushbuttonField("FlagT_A[0]", button.Field);

                        PushbuttonField button2 = stamper.AcroFields.GetNewPushbuttonFromField("FlagT_B[0]");
                        button2.Image = img;
                        stamper.AcroFields.ReplacePushbuttonField("FlagT_B[0]", button2.Field);
                        //formFields.SetField("DateField", "2023-01-01");
                        //formFields.SetField("AmountField", "$1000");

                        // Optionally, set fields as read-only
                        stamper.FormFlattening = true;
                    }
                }
            }

            Console.WriteLine("PDF form filled and saved as " + outputFilePath);

        }
        
      
    }
}
