namespace DocuSign.Models
{
    public class JobOfferResponse
    {
        public int Id { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string RecipientEmail { get; set; } = string.Empty;
        public string? PdfFileName { get; set; }
        public string? PdfUrl { get; set; }
        public string? DocuSignEnvelopeId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
