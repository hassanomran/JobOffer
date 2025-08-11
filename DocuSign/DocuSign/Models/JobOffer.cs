namespace DocuSign.Models
{
    public class JobOffer
    {
        public int Id { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string RecipientEmail { get; set; } = string.Empty;
        public string JobOfferContent { get; set; } = string.Empty;
        public string? PdfFileName { get; set; }
        public string? DocuSignEnvelopeId { get; set; }
        public string Status { get; set; } = "draft"; // draft, sent, completed, declined
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SentAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

}
