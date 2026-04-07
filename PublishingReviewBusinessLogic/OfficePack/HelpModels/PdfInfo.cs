// PublishingReviewBusinessLogic/OfficePack/HelpModels/PdfInfo.cs
using PublishingReviewBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;

namespace PublishingReviewBusinessLogic.OfficePack.HelpModels
{
    public class PdfInfo
    {
        public string FileName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public List<PublicationReportItem> Items { get; set; } = new();
    }
}
