using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using DocumentFormat;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Validation;

namespace IctectWordProcessing
{
    class DocumentProcessor
    {
        static void Main(string[] args)
        {
            //CreateWordDoc("C:/Users/Trevor/Documents/test.docx", "This is Text, ya ya ya.");
           // addSignatureFooter("C:/Users/Trevor/Documents/test.docx", "-by Nelson Mandela");

            //Test statistics
            Console.Write(basicStats("C:/Users/Trevor/Documents/testGAO.docx"));
            Console.ReadLine();
        }

        //Add footer with string signature to existing document
        public static void addSignatureFooter(string filepath, string signature)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Open(filepath, true))
            {
                MainDocumentPart mainDocPart = doc.MainDocumentPart;
                if (doc == null)
                {
                    mainDocPart = doc.AddMainDocumentPart();
                }

                if (mainDocPart.Document == null)
                {
                    mainDocPart.Document = new Document();
                }

                mainDocPart.DeleteParts(mainDocPart.FooterParts);
                FooterPart footerPart1 = mainDocPart.AddNewPart<FooterPart>("r98");

                Footer footer1 = new Footer();

                Paragraph paragraph1 = new Paragraph() { };



                Run run1 = new Run();
                Text text1 = new Text();
                text1.Text = signature;

                run1.Append(text1); 
                paragraph1.Append(run1);


                footer1.Append(paragraph1);
                footerPart1.Footer = footer1;

                SectionProperties sectionProperties1 = mainDocPart.Document.Body.Descendants<SectionProperties>().FirstOrDefault();
                if (sectionProperties1 == null)
                {
                    sectionProperties1 = new SectionProperties() { };
                    mainDocPart.Document.Body.Append(sectionProperties1);
                }
                FooterReference footerReference1 = new FooterReference() { Type = DocumentFormat.OpenXml.Wordprocessing.HeaderFooterValues.Default, Id = "r98" };

                sectionProperties1.InsertAt(footerReference1, 0);
            }

        }

        //Creates new document with text msg at filepath
        public static void CreateWordDoc(string filepath, string msg)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Create(filepath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            {
                // Add a main document part. 
                MainDocumentPart mainPart = doc.AddMainDocumentPart();

                // Create the document structure and add some text.
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());
                Paragraph para = body.AppendChild(new Paragraph());
                Run run = para.AppendChild(new Run());

                // String msg contains the text.
                run.AppendChild(new Text(msg));
            }
        }

        //Validate document is in the proper format, readable by word.
        public static void ValidateWordDocument(string filepath)
        {
            using (WordprocessingDocument wordprocessingDocument =
            WordprocessingDocument.Open(filepath, true))
            {
                try
                {
                    OpenXmlValidator validator = new OpenXmlValidator();
                    int count = 0;
                    foreach (ValidationErrorInfo error in
                        validator.Validate(wordprocessingDocument))
                    {
                        count++;
                        Console.WriteLine("Error " + count);
                        Console.WriteLine("Description: " + error.Description);
                        Console.WriteLine("ErrorType: " + error.ErrorType);
                        Console.WriteLine("Node: " + error.Node);
                        Console.WriteLine("Path: " + error.Path.XPath);
                        Console.WriteLine("Part: " + error.Part.Uri);
                        Console.WriteLine("-------------------------------------------");
                    }

                    Console.WriteLine("count={0}", count);
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                wordprocessingDocument.Close();
            }
        }

        //The following function can be used to test validation. It corrupts the input file and 
        //runs the same validation as above. 
        /*
        public static void ValidateCorruptedWordDocument(string filepath)
        {
            // Insert some text into the body, this would cause Schema Error
            using (WordprocessingDocument wordprocessingDocument =
            WordprocessingDocument.Open(filepath, true))
            {
                // Insert some text into the body, this would cause Schema Error
                Body body = wordprocessingDocument.MainDocumentPart.Document.Body;
                Run run = new Run(new Text("some text"));
                body.Append(run);

                try
                {
                    OpenXmlValidator validator = new OpenXmlValidator();
                    int count = 0;
                    foreach (ValidationErrorInfo error in
                        validator.Validate(wordprocessingDocument))
                    {
                        count++;
                        Console.WriteLine("Error " + count);
                        Console.WriteLine("Description: " + error.Description);
                        Console.WriteLine("ErrorType: " + error.ErrorType);
                        Console.WriteLine("Node: " + error.Node);
                        Console.WriteLine("Path: " + error.Path.XPath);
                        Console.WriteLine("Part: " + error.Part.Uri);
                        Console.WriteLine("-------------------------------------------");
                    }

                    Console.WriteLine("count={0}", count);
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        */

        //Return string with basic statistics for the file.
        public static string basicStats(string filepath) 
        {
            string stats = "";
            using (WordprocessingDocument doc = WordprocessingDocument.Open(filepath, false))
            {
                if (doc.ExtendedFilePropertiesPart != null)
                {
                    //There's a crap-ton of properties, each one can be accessed
                    //with the same syntax if we want more.  
                    var props = doc.ExtendedFilePropertiesPart.Properties;

                    if (props.Company != null)
                        stats += ("Company = " + props.Company.Text + "\n");

                    if (props.Manager != null)
                        stats += ("Manager = " + props.Manager.Text + "\n");

                    if (props.Lines != null)
                        stats += ("Lines = " + props.Lines.Text + "\n");

                    if (props.Pages != null)
                        stats += ("Pages = " + props.Pages.Text + "\n");

                    if (props.Words != null)
                        stats += ("Words = " + props.Pages.Text + "\n");

                    if (props.Characters != null)
                        stats += ("Characters = " + props.Pages.Text + "\n");

                    if (props.Paragraphs != null)
                        stats += ("Paragraphs = " + props.Pages.Text + "\n");

                    if (props.Pages != null)
                        stats += ("Pages = " + props.Pages.Text + "\n");


                }
            }

            return stats;
        }
    }

}
}