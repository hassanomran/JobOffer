namespace DocuSign.Models
{
    public class SigningStatusResponse
    {
        public string Status { get; set; } = string.Empty;
        public DateTime? SentAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? DocuSignEnvelopeId { get; set; }
    }
}
