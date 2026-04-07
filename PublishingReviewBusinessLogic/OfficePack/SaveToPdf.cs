// PublishingReviewBusinessLogic/OfficePack/SaveToPdf.cs
using Microsoft.VisualBasic;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PublishingReviewBusinessLogic.OfficePack.HelpModels;
using PublishingReviewBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using static System.Collections.Specialized.BitVector32;

namespace PublishingReviewBusinessLogic.OfficePack
{
    public class SaveToPdf : AbstractSaveToPdf
    {
        private Document? _document;
        private Section? _section;
        private Table? _table;

        private static ParagraphAlignment GetParagraphAlignment(PdfParagraphAlignmentType type)
        {
            return type switch
            {
                PdfParagraphAlignmentType.Center => ParagraphAlignment.Center,
                PdfParagraphAlignmentType.Left => ParagraphAlignment.Left,
                PdfParagraphAlignmentType.Right => ParagraphAlignment.Right,
                _ => ParagraphAlignment.Justify
            };
        }

        private static void DefineStyles(Document document)
        {
            var style = document.Styles["Normal"];
            style.Font.Name = "Arial";
            style.Font.Size = 11;
            var title = document.Styles.AddStyle("NormalTitle", "Normal");
            title.Font.Bold = true;
            title.ParagraphFormat.SpaceAfter = "0.2cm";
        }

        protected override void CreatePdf(PdfInfo info)
        {
            _document = new Document();
            DefineStyles(_document);

            _section = _document.AddSection();
            _section.PageSetup = _document.DefaultPageSetup.Clone();
            _section.PageSetup.PageFormat = PageFormat.A4;
            _section.PageSetup.Orientation = Orientation.Landscape;
            _section.PageSetup.LeftMargin = "1cm";
            _section.PageSetup.PageWidth = "29.7cm";
            _section.PageSetup.PageHeight = "21cm";
        }

        protected override void CreateParagraph(PdfParagraph pdfParagraph)
        {
            if (_section == null) return;
            var paragraph = _section.AddParagraph(pdfParagraph.Text);
            paragraph.Format.SpaceAfter = "0.3cm";
            paragraph.Format.Alignment = GetParagraphAlignment(pdfParagraph.ParagraphAligment);
            paragraph.Style = pdfParagraph.Style;
        }

        protected override void CreateTable(List<string> columns)
        {
            if (_document == null || _section == null) return;
            _table = _section.AddTable();
            foreach (var col in columns)
            {
                _table.AddColumn(col);
            }
            _table.Borders.Width = 0.5;
        }

        protected override void CreateRow(PdfRowParameters rowParameters)
        {
            if (_table == null) return;
            var row = _table.AddRow();
            for (int i = 0; i < rowParameters.Texts.Count; ++i)
            {
                var cell = row.Cells[i];
                cell.AddParagraph(rowParameters.Texts[i]);
                if (!string.IsNullOrEmpty(rowParameters.Style))
                    cell.Style = rowParameters.Style;
                Unit borderWidth = 0.5;
                cell.Borders.Left.Width = borderWidth;
                cell.Borders.Right.Width = borderWidth;
                cell.Borders.Top.Width = borderWidth;
                cell.Borders.Bottom.Width = borderWidth;
                cell.Format.Alignment = GetParagraphAlignment(rowParameters.ParagraphAlignment);
                cell.VerticalAlignment = VerticalAlignment.Center;
            }
        }

        protected override void SavePdf(PdfInfo info)
        {
            if (_document == null) return;
            var renderer = new PdfDocumentRenderer(true) { Document = _document };
            renderer.RenderDocument();
            renderer.PdfDocument.Save(info.FileName);
        }
    }
}
