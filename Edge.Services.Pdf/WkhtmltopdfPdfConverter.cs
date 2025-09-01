using System.Diagnostics;
using System.Text;

namespace Edge.Services.Pdf
{
    public class WkhtmltopdfPdfConverter : IPdfConverter
    {
        private readonly string _wkhtmltopdfPath;

        public WkhtmltopdfPdfConverter(string wkhtmltopdfPath = "wkhtmltopdf")
        {
            _wkhtmltopdfPath = wkhtmltopdfPath;
        }

        public async Task<byte[]> ConvertToPDFAsync(string html)
        {
            if (string.IsNullOrEmpty(html))
                throw new ArgumentException("HTML content cannot be null or empty", nameof(html));

            var tempHtmlFile = Path.GetTempFileName() + ".html";
            var tempPdfFile = Path.GetTempFileName() + ".pdf";

            try
            {
                // Write HTML content to temporary file
                await File.WriteAllTextAsync(tempHtmlFile, html, Encoding.UTF8);

                // Prepare wkhtmltopdf process
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = _wkhtmltopdfPath,
                    Arguments = $"--page-size A4 --orientation Portrait --margin-top 0.75in --margin-right 0.75in --margin-bottom 0.75in --margin-left 0.75in \"{tempHtmlFile}\" \"{tempPdfFile}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = new Process();
                process.StartInfo = processStartInfo;
                
                process.Start();
                
                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();
                
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException($"wkhtmltopdf failed with exit code {process.ExitCode}. Error: {error}");
                }

                if (!File.Exists(tempPdfFile))
                {
                    throw new InvalidOperationException("PDF file was not created successfully");
                }

                // Read the generated PDF file
                return await File.ReadAllBytesAsync(tempPdfFile);
            }
            finally
            {
                // Clean up temporary files
                if (File.Exists(tempHtmlFile))
                    File.Delete(tempHtmlFile);
                if (File.Exists(tempPdfFile))
                    File.Delete(tempPdfFile);
            }
        }
    }
}