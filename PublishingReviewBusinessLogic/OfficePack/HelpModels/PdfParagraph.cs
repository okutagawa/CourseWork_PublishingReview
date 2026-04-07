// PublishingReviewBusinessLogic/OfficePack/HelpModels/PdfParagraph.cs
namespace PublishingReviewBusinessLogic.OfficePack.HelpModels
{
    public class PdfParagraph
    {
        public string Text { get; set; } = string.Empty;
        public string Style { get; set; } = string.Empty;
        public PdfParagraphAlignmentType ParagraphAligment { get; set; }
    }
}
