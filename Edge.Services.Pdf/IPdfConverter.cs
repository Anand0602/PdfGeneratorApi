namespace Edge.Services.Pdf
{
    public interface IPdfConverter
    {
        Task<byte[]> ConvertToPDFAsync(string html);
    }
}