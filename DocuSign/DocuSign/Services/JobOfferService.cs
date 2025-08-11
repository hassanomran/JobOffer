using DocuSign.Data;
using DocuSign.Interface;
using DocuSign.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DocuSign.Services
{
    public class JobOfferService : IJobOfferService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPdfService _pdfService;
        private readonly IDocuSignService _docuSignService;
        private readonly ILogger<JobOfferService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JobOfferService(
            ApplicationDbContext context,
            IPdfService pdfService,
            IDocuSignService docuSignService,
            ILogger<JobOfferService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _pdfService = pdfService;
            _docuSignService = docuSignService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<JobOfferResponse> CreateJobOfferAsync(JobOfferRequest request)
        {
            var pdfBytes = await _pdfService.GenerateJobOfferPdfAsync(
                request.RecipientName,
                request.RecipientEmail,
                request.JobOfferContent);

            var fileName = $"job_offer_{DateTime.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString("N")[..8]}.pdf";
            var relativePath = _pdfService.SavePdf(pdfBytes, fileName);

            var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var pdfUrl = $"{baseUrl}{relativePath}";

            var jobOffer = new JobOffer
            {
                RecipientName = request.RecipientName,
                RecipientEmail = request.RecipientEmail,
                JobOfferContent = request.JobOfferContent,
                PdfFileName = fileName,
                Status = "draft",
                CreatedAt = DateTime.UtcNow
            };

            _context.JobOffers.Add(jobOffer);
            await _context.SaveChangesAsync();

            var response = MapToResponse(jobOffer);
            response.PdfUrl = pdfUrl; 

            _logger.LogInformation($"Created job offer {jobOffer.Id} for {request.RecipientName}");

            return response;
        }


        public async Task<JobOfferResponse?> GetJobOfferAsync(int id)
        {
            var jobOffer = await _context.JobOffers.FindAsync(id);
            return jobOffer != null ? MapToResponse(jobOffer) : null;
        }

        public async Task<JobOfferResponse> SendForSignatureAsync(int id)
        {
            var jobOffer = await _context.JobOffers.FindAsync(id);
            if (jobOffer == null)
                throw new ArgumentException($"Job offer with ID {id} not found");

            if (jobOffer.Status != "draft")
                throw new InvalidOperationException($"Job offer is already {jobOffer.Status}");

            // Get PDF bytes
            var pdfBytes = _pdfService.GetPdf(jobOffer.PdfFileName!);

            // Create DocuSign envelope
            var envelope = await _docuSignService.CreateEnvelopeAsync(
                jobOffer.RecipientEmail,
                jobOffer.RecipientName,
                pdfBytes,
                jobOffer.PdfFileName!);

            // Send envelope
            var sendSuccess = await _docuSignService.SendEnvelopeAsync(envelope.EnvelopeId);

            if (sendSuccess)
            {
                jobOffer.DocuSignEnvelopeId = envelope.EnvelopeId;
                jobOffer.Status = "sent";
                jobOffer.SentAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Sent job offer {id} for signature with envelope {envelope.EnvelopeId}");
            }
            else
            {
                throw new InvalidOperationException("Failed to send envelope for signature");
            }

            return MapToResponse(jobOffer);
        }

        public async Task<SigningStatusResponse> GetSigningStatusAsync(int id)
        {
            var jobOffer = await _context.JobOffers.FindAsync(id);
            if (jobOffer == null)
                throw new ArgumentException($"Job offer with ID {id} not found");

            var response = new SigningStatusResponse
            {
                Status = jobOffer.Status,
                SentAt = jobOffer.SentAt,
                CompletedAt = jobOffer.CompletedAt,
                DocuSignEnvelopeId = jobOffer.DocuSignEnvelopeId
            };

            // Check DocuSign status if envelope exists
            if (!string.IsNullOrEmpty(jobOffer.DocuSignEnvelopeId))
            {
                var docuSignStatus = await _docuSignService.GetEnvelopeStatusAsync(jobOffer.DocuSignEnvelopeId);

                // Update local status if DocuSign status has changed
                if (docuSignStatus == "completed" && jobOffer.Status != "completed")
                {
                    jobOffer.Status = "completed";
                    jobOffer.CompletedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    response.Status = "completed";
                    response.CompletedAt = jobOffer.CompletedAt;
                }
            }

            return response;
        }

        public async Task<List<JobOfferResponse>> GetAllJobOffersAsync()
        {
            var jobOffers = await _context.JobOffers
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return jobOffers.Select(MapToResponse).ToList();
        }

        public async Task<bool> UpdateJobOfferStatusAsync(int id, string status)
        {
            var jobOffer = await _context.JobOffers.FindAsync(id);
            if (jobOffer == null) return false;

            jobOffer.Status = status;
            if (status == "completed")
            {
                jobOffer.CompletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private static JobOfferResponse MapToResponse(JobOffer jobOffer)
        {
            return new JobOfferResponse
            {
                Id = jobOffer.Id,
                RecipientName = jobOffer.RecipientName,
                RecipientEmail = jobOffer.RecipientEmail,
                PdfFileName = jobOffer.PdfFileName,
                DocuSignEnvelopeId = jobOffer.DocuSignEnvelopeId,
                Status = jobOffer.Status,
                CreatedAt = jobOffer.CreatedAt,
                SentAt = jobOffer.SentAt,
                CompletedAt = jobOffer.CompletedAt
            };
        }
    }
}


