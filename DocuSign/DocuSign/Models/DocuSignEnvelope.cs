namespace DocuSign.Models
{
    public class DocuSignEnvelope
    {
        public string EnvelopeId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDateTime { get; set; }
        public string EmailSubject { get; set; } = string.Empty;
    }
}
