using System.ComponentModel.DataAnnotations;

namespace DocuSign.Models
{
    public class JobOfferRequest
    {
        [Required(ErrorMessage = "Recipient name is required")]
        public string RecipientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Recipient email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string RecipientEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Job offer content is required")]
        public string JobOfferContent { get; set; } = string.Empty;
    }
}
