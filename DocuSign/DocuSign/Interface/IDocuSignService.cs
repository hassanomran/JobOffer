using DocuSign.Models;

namespace DocuSign.Interface
{
    public interface IDocuSignService
    {
        Task<DocuSignEnvelope> CreateEnvelopeAsync(string recipientEmail, string recipientName, byte[] pdfBytes, string fileName);
        Task<string> GetEnvelopeStatusAsync(string envelopeId);
        Task<bool> SendEnvelopeAsync(string envelopeId);
    }
}
