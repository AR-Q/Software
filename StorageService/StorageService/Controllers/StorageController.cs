using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StorageService.Application.Interfaces;

namespace StorageService.Presentation.Controllers
{
    [ApiController]
    [Route("api/storage")]
    [VerifyUser]
    public class StorageController : ControllerBase
    {
        private readonly ICloudStorageService _storage;

        public StorageController(ICloudStorageService storage)
        {
            _storage = storage;
        }

        //[HttpPost("upload")]
        //public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string userId)
        //{
        //    try
        //    {
        //        await _storage.EnsureUserBucketExistsAsync(userId);
        //        var uri = await _storage.UploadAsync(file, container: "", userId);
        //        return Ok(new { Url = uri.ToString() });
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(new { Message = ex.Message });
        //    }
        //}



        [HttpGet("download")]
        public async Task<IActionResult> Download(string fileName, string userId)
        {
            var stream = await _storage.DownloadAsync(fileName, container: "", userId);
            return File(stream, "application/octet-stream", fileName);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string fileName, string userId)
        {
            var result = await _storage.DeleteAsync(fileName, container: "", userId);
            return result ? Ok() : NotFound();
        }

        [HttpGet("list")]
        [VerifyUser]
        public async Task<IActionResult> List([FromQuery] string userId)
        {
            await _storage.EnsureUserBucketExistsAsync(userId);
            var files = await _storage.ListAsync(container: "", userId);
            return Ok(files);
        }


        [HttpGet("usage")]
        public async Task<IActionResult> GetBucketUsage([FromQuery] string userId = null)
        {
            var sizeInBytes = await _storage.GetBucketSizeAsync(userId);
            var sizeInMB = Math.Round(sizeInBytes / 1024.0 / 1024.0, 2);

            return Ok(new
            {
                UserId = userId ?? "all",
                SizeInBytes = sizeInBytes,
                SizeInMB = sizeInMB
            });
        }

        [HttpPost("create-folder")]
        public async Task<IActionResult> CreateFolder([FromQuery] string folderName, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(folderName) || string.IsNullOrWhiteSpace(userId))
                return BadRequest("Folder name and user ID are required.");

            await _storage.CreateFolderAsync(folderName, userId);
            return Ok(new { Message = $"Folder '{folderName}' created for user {userId}" });
        }

        [HttpGet("folders")]
        public async Task<IActionResult> ListFolders([FromQuery] string userId)
        {
            var folders = await _storage.ListFoldersAsync(userId);
            return Ok(folders);
        }

        [HttpPost("upload")]
      
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string userId, [FromQuery] string? folder = "")
        {
            try
            {
                await _storage.EnsureUserBucketExistsAsync(userId);
                var uri = await _storage.UploadAsync(file, container: "", userId, folder);
                return Ok(new { Url = uri.ToString() });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("move")]
        public async Task<IActionResult> MoveFile([FromQuery] string userId, [FromQuery] string oldPath, [FromQuery] string newPath)
        {
            if (string.IsNullOrWhiteSpace(oldPath) || string.IsNullOrWhiteSpace(newPath))
                return BadRequest("Both oldPath and newPath are required.");

            await _storage.MoveFileAsync(userId, oldPath, newPath);
            return Ok(new { Message = $"File moved from '{oldPath}' to '{newPath}'" });
        }

        [HttpPost("move-folder")]
        public async Task<IActionResult> MoveFolder([FromQuery] string userId, [FromQuery] string oldFolder, [FromQuery] string newFolder)
        {
            if (string.IsNullOrWhiteSpace(oldFolder) || string.IsNullOrWhiteSpace(newFolder))
                return BadRequest("Both oldFolder and newFolder are required.");

            await _storage.MoveFolderAsync(userId, oldFolder, newFolder);
            return Ok(new { Message = $"Folder moved from '{oldFolder}' to '{newFolder}'" });
        }

        [HttpDelete("delete-folder")]
        public async Task<IActionResult> DeleteFolder([FromQuery] string userId, [FromQuery] string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                return BadRequest("Folder path is required.");

            await _storage.DeleteFolderAsync(userId, folderPath);
            return Ok(new { Message = $"Folder '{folderPath}' deleted for user {userId}" });
        }

    }

}
