using DocuSign.Interface;
using iText.Html2pdf;

namespace DocuSign.Services
{
    public class PdfService : IPdfService
    {
        private readonly string _pdfDirectory;
        private readonly IConfiguration _configuration;

        public PdfService(IWebHostEnvironment environment,IConfiguration configuration)
        {
            _pdfDirectory = Path.Combine(environment.ContentRootPath, "GeneratedPdfs");
            if (!Directory.Exists(_pdfDirectory))
            {
                Directory.CreateDirectory(_pdfDirectory);
            }

            _configuration = configuration;
        }

        public async Task<byte[]> GenerateJobOfferPdfAsync(string recipientName, string recipientEmail, string content)
        {
            try
            {
                using var memoryStream = new MemoryStream();

                var htmlContent = GenerateHtmlTemplate(recipientName, recipientEmail, content);
                var converterProperties = new ConverterProperties();

                HtmlConverter.ConvertToPdf(htmlContent, memoryStream, converterProperties);

                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PDF Generation Error] {ex}");
                throw;
            }
        }



        public string SavePdf(byte[] pdfBytes, string fileName)
        {
            var pdfFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "GeneratedPdfs");

            if (!Directory.Exists(pdfFolder))
            {
                Directory.CreateDirectory(pdfFolder);
            }

            var filePath = Path.Combine(pdfFolder, fileName);
            File.WriteAllBytes(filePath, pdfBytes);

            // Return ONLY the relative URL (not the file system path)
            return $"/GeneratedPdfs/{fileName}";
        }


        public byte[] GetPdf(string fileName)
        {
            var pdfFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "GeneratedPdfs");
            var filePath = Path.Combine(pdfFolder, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("PDF file not found", filePath);
            }

            return File.ReadAllBytes(filePath);
        }


        private string GenerateHtmlTemplate(string recipientName, string recipientEmail, string content)
        {
            var currentDate = DateTime.Now.ToString("MMMM dd, yyyy");

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ 
            font-family: Arial, sans-serif; 
            line-height: 1.6; 
            margin: 40px;
            color: #333;
        }}
        .header {{ 
            text-align: center; 
            border-bottom: 2px solid #0066cc;
            padding-bottom: 20px;
            margin-bottom: 30px;
        }}
        .company-name {{ 
            font-size: 24px; 
            font-weight: bold; 
            color: #0066cc;
            margin-bottom: 10px;
        }}
        .date {{ 
            margin: 20px 0; 
            text-align: right;
        }}
        .recipient {{ 
            margin: 20px 0;
        }}
        .content {{ 
            margin: 30px 0; 
            text-align: justify;
        }}
        .signature {{ 
            margin-top: 60px;
        }}
        .signature-line {{ 
            border-bottom: 1px solid #333;
            width: 300px;
            margin: 20px 0 5px 0;
        }}
    </style>
</head>
<body>
    <div class='header'>
        <div class='company-name'>Your Company Name</div>
        <div>Job Offer Letter</div>
    </div>
    
    <div class='date'>
        <strong>Date:</strong> {currentDate}
    </div>
    
    <div class='recipient'>
        <strong>To:</strong><br>
        {recipientName}<br>
        {recipientEmail}
    </div>
    
    <div class='content'>
        {content.Replace("\n", "<br>")}
    </div>
    
    <div class='signature'>
        <p>Sincerely,</p>
        <div class='signature-line'></div>
        <p>HR Manager<br>Your Company Name</p>
    </div>
</body>
</html>";
        }
    }

}
