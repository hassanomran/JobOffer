using DocuSign.Interface;
using DocuSign.Models;

namespace DocuSign.Services
{
    public class DocuSignService : IDocuSignService
    {
        private readonly ILogger<DocuSignService> _logger;
        private readonly Dictionary<string, string> _envelopeStatuses;

        public DocuSignService(ILogger<DocuSignService> logger)
        {
            _logger = logger;
            // Simulate envelope statuses in memory (in real app, this would be DocuSign API calls)
            _envelopeStatuses = new Dictionary<string, string>();
        }

        public async Task<DocuSignEnvelope> CreateEnvelopeAsync(string recipientEmail, string recipientName, byte[] pdfBytes, string fileName)
        {
            // Simulate DocuSign envelope creation
            await Task.Delay(500); // Simulate API call delay

            var envelopeId = $"env_{Guid.NewGuid().ToString("N")[..8]}";

            var envelope = new DocuSignEnvelope
            {
                EnvelopeId = envelopeId,
                Status = "created",
                CreatedDateTime = DateTime.UtcNow,
                EmailSubject = $"Job Offer - Please Review and Sign"
            };

            // Store the initial status
            _envelopeStatuses[envelopeId] = "created";

            _logger.LogInformation($"Created DocuSign envelope {envelopeId} for {recipientEmail}");

            return envelope;
        }

        public async Task<string> GetEnvelopeStatusAsync(string envelopeId)
        {
            // Simulate DocuSign status check
            await Task.Delay(200);

            if (_envelopeStatuses.TryGetValue(envelopeId, out var status))
            {
                // Simulate status progression for demo purposes
                if (status == "sent" && Random.Shared.Next(1, 100) > 70)
                {
                    // 30% chance to progress to completed after being sent
                    _envelopeStatuses[envelopeId] = "completed";
                    return "completed";
                }
                return status;
            }

            return "not_found";
        }

        public async Task<bool> SendEnvelopeAsync(string envelopeId)
        {
            // Simulate sending envelope for signature
            await Task.Delay(300);

            if (_envelopeStatuses.ContainsKey(envelopeId))
            {
                _envelopeStatuses[envelopeId] = "sent";
                _logger.LogInformation($"Sent DocuSign envelope {envelopeId} for signature");
                return true;
            }

            return false;
        }
    }
}
