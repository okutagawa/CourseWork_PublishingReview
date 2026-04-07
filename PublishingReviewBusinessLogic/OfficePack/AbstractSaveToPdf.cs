using PublishingReviewBusinessLogic.OfficePack.HelpModels;
using PublishingReviewBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;

namespace PublishingReviewBusinessLogic.OfficePack
{
    public abstract class AbstractSaveToPdf
    {
        public void CreateDoc(PdfInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            CreatePdf(info);

            CreateParagraph(new PdfParagraph
            {
                Text = info.Title,
                Style = "NormalTitle",
                ParagraphAligment = PdfParagraphAlignmentType.Center
            });

            CreateParagraph(new PdfParagraph
            {
                Text = $"Период: {info.DateFrom:dd.MM.yyyy} — {info.DateTo:dd.MM.yyyy}",
                Style = "Normal",
                ParagraphAligment = PdfParagraphAlignmentType.Center
            });

            CreateTable(new List<string> { "1.2cm", "5cm", "6cm", "5cm", "2.5cm", "1.8cm", "2.5cm" });

            CreateRow(new PdfRowParameters
            {
                Texts = new List<string> { "№", "Авторы", "Название издания", "Предмет", "Коэфф.", "Тираж", "Дата" },
                Style = "NormalTitle",
                ParagraphAlignment = PdfParagraphAlignmentType.Center
            });

            foreach (var item in info.Items)
            {
                CreateRow(new PdfRowParameters
                {
                    Texts = new List<string>
                    {
                        item.Id.ToString(),
                        item.Authors ?? string.Empty,
                        item.Title ?? string.Empty,
                        item.Subject ?? string.Empty,
                        item.ResourcesRate.ToString("0.##"),
                        item.Volume.ToString(),
                        item.Date?.ToString("dd.MM.yyyy") ?? string.Empty
                    },
                    Style = "Normal",
                    ParagraphAlignment = PdfParagraphAlignmentType.Center
                });
            }

            SavePdf(info);
        }

        protected abstract void CreatePdf(PdfInfo info);
        protected abstract void CreateParagraph(PdfParagraph paragraph);
        protected abstract void CreateTable(List<string> columns);
        protected abstract void CreateRow(PdfRowParameters rowParameters);
        protected abstract void SavePdf(PdfInfo info);
    }
}
