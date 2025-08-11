using DocuSign.Interface;
using DocuSign.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobOfferController : ControllerBase
    {
        private readonly IJobOfferService _jobOfferService;
        private readonly IPdfService _pdfService;
        private readonly ILogger<JobOfferController> _logger;

        public JobOfferController(
            IJobOfferService jobOfferService,
            IPdfService pdfService,
            ILogger<JobOfferController> logger)
        {
            _jobOfferService = jobOfferService;
            _pdfService = pdfService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<ActionResult<JobOfferResponse>> CreateJobOffer([FromBody] JobOfferRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _jobOfferService.CreateJobOfferAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job offer");
                return StatusCode(500, new { message = "An error occurred while creating the job offer." });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobOfferResponse>> GetJobOffer(int id)
        {
            try
            {
                var jobOffer = await _jobOfferService.GetJobOfferAsync(id);
                if (jobOffer == null)
                {
                    return NotFound(new { message = "Job offer not found." });
                }

                return Ok(jobOffer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job offer {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the job offer." });
            }
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GetJobOfferPdf(int id)
        {
            try
            {
                var jobOffer = await _jobOfferService.GetJobOfferAsync(id);
                if (jobOffer == null || string.IsNullOrEmpty(jobOffer.PdfFileName))
                {
                    return NotFound(new { message = "Job offer PDF not found." });
                }

                var pdfBytes = _pdfService.GetPdf(jobOffer.PdfFileName);
                return File(pdfBytes, "application/pdf", jobOffer.PdfFileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound(new { message = "PDF file not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving PDF for job offer {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the PDF." });
            }
        }

        [HttpPost("{id}/send-for-signature")]
        public async Task<ActionResult<JobOfferResponse>> SendForSignature(int id)
        {
            try
            {
                var response = await _jobOfferService.SendForSignatureAsync(id);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending job offer {Id} for signature", id);
                return StatusCode(500, new { message = "An error occurred while sending the job offer for signature." });
            }
        }

        [HttpGet("{id}/status")]
        public async Task<ActionResult<SigningStatusResponse>> GetSigningStatus(int id)
        {
            try
            {
                var status = await _jobOfferService.GetSigningStatusAsync(id);
                return Ok(status);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting signing status for job offer {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the signing status." });
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<JobOfferResponse>>> GetAllJobOffers()
        {
            try
            {
                var jobOffers = await _jobOfferService.GetAllJobOffersAsync();
                return Ok(jobOffers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all job offers");
                return StatusCode(500, new { message = "An error occurred while retrieving job offers." });
            }
        }

        [HttpPost("webhook/docusign")]
        public async Task<IActionResult> DocuSignWebhook([FromBody] object webhookData)
        {
            try
            {
                // This would handle real DocuSign webhook notifications
                // For simulation purposes, we'll just log it
                _logger.LogInformation("Received DocuSign webhook: {Data}", webhookData.ToString());
                return Ok(new { message = "Webhook received successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing DocuSign webhook");
                return StatusCode(500, new { message = "An error occurred while processing the webhook." });
            }
        }
    }
}
