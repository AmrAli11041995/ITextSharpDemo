using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.pdf.parser;
//using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;
using System.Reflection;
using System.Collections;
//using static System.Net.Mime.MediaTypeNames;
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

        public static void CreateFormFieldsPDF()
        {
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


            // Sample data object
            var data = new
            {
                HomeTeamName = "AlAhly",
                AwayTeamName = "Zamalek",
                HomeScore = "6",
                AwayScore = "1",

            };



            var obj = new DataTeams() { 
                TeamA= new Team(){ 
                    color="red",
                    Name = "AlAhly" , 
                    Players = new List<Player> { 
                        new Player() { Name = "Ahmed",Position="GK" , ShirtNumber = "1"}, 
                        new Player() { Name = "Ali",Position="DF" , ShirtNumber = "10"} 
                    },
                    Img_FlagPath="D:\\ahly.png" 
                } , 
                TeamB = new Team (){
                    color="white",
                    Name = "Zamalek" ,
                    Players = new List<Player>{
                        new Player() { Name = "Gabaski",Position="GK" , ShirtNumber = "1"},
                        new Player() { Name = "Omar Gaber",Position="DF" , ShirtNumber = "4"}
                    },
                    Img_FlagPath="D:\\zamalek.png"
                }
            };

            var flattened = Flatten(obj);

            foreach (var kvp in flattened)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }

            // Define a dictionary to map form field names to property names
            var fieldMapping2 = new Dictionary<string, string>
        {
            { "TeamA_N[0]", "HomeTeamName" },
            { "TeamB_N[0]", "AwayTeamName" },
            { "TeamA1_Score[0]", "HomeScore" },
            { "TeamB_Score[0]", "AwayScore" }
        };
            FillPdfForm(inputFilePath, outputFilePath, data, fieldMapping2);

            Console.WriteLine("PDF form filled and saved as " + outputFilePath);

        }
        public static Dictionary<string, object> Flatten(object obj, string parentKey = "", string separator = "_")
        {
            var result = new Dictionary<string, object>();

            if (obj == null) return result;

            // If the object is a list or array, handle each element with an index
            if (obj is IEnumerable enumerable && !(obj is string))
            {
                int index = 0;
                foreach (var item in enumerable)
                {
                    var indexedKey = $"{parentKey}{separator}{index}";
                    foreach (var innerKvp in Flatten(item, indexedKey, separator))
                    {
                        result[innerKvp.Key] = innerKvp.Value;
                    }
                    index++;
                }
            }
            else
            {
                // Handle properties of a single object
                foreach (PropertyInfo property in obj.GetType().GetProperties())
                {
                    var propValue = property.GetValue(obj);
                    var propName = string.IsNullOrEmpty(parentKey) ? property.Name : parentKey + separator + property.Name;

                    if (propValue != null && !property.PropertyType.IsPrimitive && property.PropertyType != typeof(string) && property.PropertyType.IsClass)
                    {
                        // Recursively flatten nested objects or collections
                        foreach (var innerKvp in Flatten(propValue, propName, separator))
                        {
                            result[innerKvp.Key] = innerKvp.Value;
                        }
                    }
                    else
                    {
                        result[propName] = propValue;
                    }
                }
            }

            return result;
        }
     
        public static void FillPdfForm(string inputFilePath, string outputFilePath, object data, Dictionary<string, string> fieldMapping)
        {
            using (PdfReader reader = new PdfReader(inputFilePath))
            {
                using (FileStream output = new FileStream(outputFilePath, FileMode.Create))
                {
                    using (PdfStamper stamper = new PdfStamper(reader, output))
                    {
                        Image image = Image.GetInstance("D:\\ahly.png");
                        Image img = Image.GetInstance("D:\\zamalek.png");

                        AcroFields formFields = stamper.AcroFields;
                        var x = formFields.Fields.First().GetType();

                        //formFields.SetField("FlagT_A[0]", "Amr Salah");
                        PushbuttonField button = stamper.AcroFields.GetNewPushbuttonFromField("FlagT_A[0]");
                        button.Image = image;
                        stamper.AcroFields.ReplacePushbuttonField("FlagT_A[0]", button.Field);

                        PushbuttonField button2 = stamper.AcroFields.GetNewPushbuttonFromField("FlagT_B[0]");
                        button2.Image = img;
                        stamper.AcroFields.ReplacePushbuttonField("FlagT_B[0]", button2.Field);
                        // Iterate over the field mapping
                        foreach (var entry in fieldMapping)
                        {
                            string fieldName = entry.Key;     // Form field name in PDF
                            string propertyName = entry.Value; // Property name in the object

                            // Get the property info from the data object
                            PropertyInfo prop = data.GetType().GetProperty(propertyName);
                            if (prop != null)
                            {
                                object value = prop.GetValue(data, null);
                                if (value != null && formFields.Fields.ContainsKey(fieldName))
                                {
                                    formFields.SetField(fieldName, value.ToString());
                                }
                            }
                        }

                        stamper.FormFlattening = true; // Flatten form to prevent further editing
                    }
                }
            }

        }
    }

    public class DataTeams
    {
        public Team TeamA { get; set; }
        public Team TeamB { get; set; }
    }
    public class Team
    {
        public string Img_FlagPath { get; set; }
        public string Name { get; set; }
        public string color { get; set; }
        public List<Player> Players { get; set; }
    }

  
    public class Player
    {
        public string Name { get; set; }
        public string ShirtNumber { get; set; }
        public string Position { get; set; }
    }
   
}
