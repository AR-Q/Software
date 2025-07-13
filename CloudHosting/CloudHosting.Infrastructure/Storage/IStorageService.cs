using System;
using System.IO;
using System.Threading.Tasks;

namespace CloudHosting.Infrastructure.Storage
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
        Task<Stream> DownloadFileAsync(string filePath);
        Task DeleteFileAsync(string filePath);
        Task<bool> FileExistsAsync(string filePath);
    }
}