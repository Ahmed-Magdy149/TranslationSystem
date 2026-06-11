using Microsoft.Extensions.Logging;

namespace TMS.Infrastructure.Services;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName);
    Task<Stream?> DownloadFileAsync(string fileUrl);
    Task<bool> DeleteFileAsync(string fileUrl);
    Task<string> ExtractTextAsync(Stream fileStream, string fileName);
}

public class FileStorageService : IFileStorageService
{
    private readonly ILogger<FileStorageService> _logger;
    private readonly string _storagePath;

    public FileStorageService(ILogger<FileStorageService> logger)
    {
        _logger = logger;
        _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        Directory.CreateDirectory(_storagePath);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(_storagePath, uniqueFileName);

        using var outputStream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(outputStream);

        _logger.LogInformation("File uploaded: {FileName} -> {FilePath}", fileName, filePath);
        return uniqueFileName;
    }

    public Task<Stream?> DownloadFileAsync(string fileUrl)
    {
        var filePath = Path.Combine(_storagePath, fileUrl);
        if (!File.Exists(filePath))
        {
            _logger.LogWarning("File not found: {FileUrl}", fileUrl);
            return Task.FromResult<Stream?>(null);
        }

        Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return Task.FromResult<Stream?>(stream);
    }

    public Task<bool> DeleteFileAsync(string fileUrl)
    {
        var filePath = Path.Combine(_storagePath, fileUrl);
        if (!File.Exists(filePath))
            return Task.FromResult(false);

        File.Delete(filePath);
        _logger.LogInformation("File deleted: {FileUrl}", fileUrl);
        return Task.FromResult(true);
    }

    public async Task<string> ExtractTextAsync(Stream fileStream, string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
        {
            ".txt" => await ExtractFromTxtAsync(fileStream),
            ".docx" => ExtractFromDocx(fileStream),
            ".pdf" => ExtractFromPdf(fileStream),
            _ => throw new NotSupportedException($"File type '{extension}' is not supported.")
        };
    }

    private static async Task<string> ExtractFromTxtAsync(Stream stream)
    {
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    private static string ExtractFromDocx(Stream stream)
    {
        using var document = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(stream, false);
        var body = document.MainDocumentPart?.Document?.Body;
        return body?.InnerText ?? string.Empty;
    }

    private static string ExtractFromPdf(Stream stream)
    {
        using var pdfReader = new iText.Kernel.Pdf.PdfReader(stream);
        using var pdfDocument = new iText.Kernel.Pdf.PdfDocument(pdfReader);

        var text = new System.Text.StringBuilder();
        for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
        {
            var page = pdfDocument.GetPage(i);
            var pageText = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(page);
            text.AppendLine(pageText);
        }

        return text.ToString();
    }
}
