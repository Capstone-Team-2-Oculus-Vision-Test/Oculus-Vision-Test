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
        public static void PdfGenerate()
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Background(Colors.Grey.Lighten1)
                        .Text(" Visual Field Analysis")
                        .SemiBold().FontSize(20).FontColor(Colors.Black);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(8);
                            column.Item().Row(row =>
                            {
                                //row.ConstantItem(100).Image("tree.jpg");
                                row.RelativeItem().Column(innerColumn =>
                                {
                                    innerColumn.Spacing(8);
                                    innerColumn.Item().Text($"Patient Name: {patientDTO.FirstName} {patientDTO.LastName}");
                                    innerColumn.Item().Text($"ID: {patientDTO.ID}");
                                });
                                row.ConstantItem(100).Column(innerColumn =>
                                {
                                    innerColumn.Spacing(8);
                                    innerColumn.Item().Text("Sex: " + patientDTO.Sex);
                                    innerColumn.Item().Text("Eye: " + resultsDTO.EyeTested.ToString());
                                });
                            });
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Text("fsdg");
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                        });
                });
                /*
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Background(Colors.Grey.Lighten1)
                        .Text(" Visual Field Analysis")
                        .SemiBold().FontSize(20).FontColor(Colors.Black);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(20);

                            column.Item().Text("Here's some more details (not really)");
                            column.Item().Image(Placeholders.Image(200,100));
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                        });
                        
                });*/
            }).GeneratePdf("example.pdf");
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
