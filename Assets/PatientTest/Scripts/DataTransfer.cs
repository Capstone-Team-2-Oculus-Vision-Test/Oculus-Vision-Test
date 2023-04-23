using System;
using System.Collections.Generic;
using UnityEngine;
using static Utility.UtilityScripts;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PatientTest.Scripts
{
    public static class DataTransfer
    {
        public static TestResultsDTO resultsDTO = new();
        public static PatientDTO patientDTO = new();
        public static void PdfGenerate() //Placeholder PDF content for now
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(20));
    
                    page.Header()
                        .Text("Hello PDF!")
                        .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);
    
                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(20);
            
                            x.Item().Text(Placeholders.LoremIpsum());
                            x.Item().Image(Placeholders.Image(200, 100));
                        });
    
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf("hello.pdf");
        }
    }
    
    public class TestResultsDTO
    {
        
        public List<Vector4> Data;
        public EyeEnum EyeTested; //Left or right eye
        public TestEnum Test;
        public DateTime StartTime;
        public DateTime EndTime;
        public float Duration;
        public string Algorithm = "Specvis";
        public float StimulusSize;
        public Color StimulusColor;
        public int NumPoints;
        public float ErrorRate;
        public float ScalingFactor;
    }

    public class PatientDTO
    {
        //Patient info
        public string ID;
        public string FirstName;
        public string LastName;
        public string Age;
        public string Sex;

        public bool MissingData()
        {
            // if any fields empty, return true
            return ID == "" || FirstName == "" || LastName == "" || Age == "" || Sex == "";
        }
    }

    public class TestListData
    {
        public TestResultsDTO Results;
        public string Name;
        public string Date;
        public int ID;
    }
}
