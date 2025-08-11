using DocuSign.Models;

namespace DocuSign.Interface
{
    public interface IJobOfferService
    {
        Task<JobOfferResponse> CreateJobOfferAsync(JobOfferRequest request);
        Task<JobOfferResponse?> GetJobOfferAsync(int id);
        Task<JobOfferResponse> SendForSignatureAsync(int id);
        Task<SigningStatusResponse> GetSigningStatusAsync(int id);
        Task<List<JobOfferResponse>> GetAllJobOffersAsync();
        Task<bool> UpdateJobOfferStatusAsync(int id, string status);
    }
}
