using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PublishingReviewBusinessLogic.ViewModels
{
    public class ReportModel
    {
        public string Title { get; set; } = "Отчёт по публикациям";
        public DateTime DateFrom { get; set; } = DateTime.UtcNow.AddDays(-7);
        public DateTime DateTo { get; set; } = DateTime.UtcNow;

        public List<PublicationReportItemModel> Items { get; set; } = new();

    public IDictionary<string, object>? Summary { get; set; }
}
}
