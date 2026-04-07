using System;

namespace PublishingReviewRestApi.Models.Dto;

public class AddAuthorsDto
{
    public int PublicationId { get; set; }
    public int[] UserIds { get; set; } = Array.Empty<int>();
}
