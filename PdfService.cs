using Edge.Services.Pdf;

public class PdfService
{
	private readonly IPdfConverter _pdfConverter;

	public PdfService(IPdfConverter pdfConverter)
	{
		_pdfConverter = pdfConverter;
	}

	public async Task<byte[]> GeneratePdfAsync(string htmlContent)
	{
		return await _pdfConverter.ConvertToPDFAsync(htmlContent);
	}
}