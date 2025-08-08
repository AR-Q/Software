using CloudHosting.Core.Interfaces;
using CloudHosting.Infrastructure.Data;

namespace CloudHosting.Infrastructure.Services
{
    public class SqlFileStorageService : IFileStorageService
    {
        private readonly CloudHostingDbContext _context;
        private readonly string _baseStoragePath;

        public SqlFileStorageService(CloudHostingDbContext context)
        {
            _context = context;
        }

        public SqlFileStorageService(CloudHostingDbContext context, IConfiguration configuration)
        {
            _context = context;
            _baseStoragePath = configuration["FileStorage:BasePath"];
        }

        public async Task<string> SaveFileAsync(IFormFile file, string directory)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(directory, fileName);

            Directory.CreateDirectory(directory);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        public string GetStoragePath(int userId, string fileType)
        {
            return Path.Combine(_baseStoragePath, userId.ToString(), fileType);
        }

        public async Task<Stream> GetFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

        public async Task DeleteFileAsync(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}