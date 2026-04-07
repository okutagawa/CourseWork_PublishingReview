// PublishingReviewBusinessLogic/OfficePack/HelpModels/PdfRowParameters.cs
using System.Collections.Generic;

namespace PublishingReviewBusinessLogic.OfficePack.HelpModels
{
    public class PdfRowParameters
    {
        public List<string> Texts { get; set; } = new();
        public string Style { get; set; } = string.Empty;
        public PdfParagraphAlignmentType ParagraphAlignment { get; set; }
    }
}
