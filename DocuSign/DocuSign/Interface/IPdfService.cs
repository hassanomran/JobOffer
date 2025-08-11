namespace DocuSign.Interface
{
    public interface IPdfService
    {
        Task<byte[]> GenerateJobOfferPdfAsync(string recipientName, string recipientEmail, string content);
        string SavePdf(byte[] pdfBytes, string fileName);
        byte[] GetPdf(string fileName);
    }
}
