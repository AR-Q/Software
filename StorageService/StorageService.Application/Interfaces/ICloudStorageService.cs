using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Application.Interfaces
{
    public interface ICloudStorageService
    {
        Task<Stream> DownloadAsync(string fileName, string container, string userId);
        Task<bool> DeleteAsync(string fileName, string container, string userId);
        Task<IEnumerable<string>> ListAsync(string container, string userId);
        Task EnsureBucketExistsAsync(string userId);
        Task<long> GetBucketSizeAsync(string userId = null);
        Task EnsureUserBucketExistsAsync(string userId);
        Task CreateFolderAsync(string folderName, string userId);
        Task<IEnumerable<string>> ListFoldersAsync(string userId);

        Task<Uri> UploadAsync(IFormFile file, string container, string userId, string folder = null);
        Task MoveFileAsync(string userId, string oldPath, string newPath);

        Task MoveFolderAsync(string userId, string oldFolder, string newFolder);

        Task DeleteFolderAsync(string userId, string folderPath);

    }

}
