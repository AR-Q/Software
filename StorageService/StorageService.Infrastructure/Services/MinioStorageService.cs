using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using StorageService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Infrastructure.Services
{
    public class MinioStorageService : ICloudStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private string GetUserBucket(string userId) => $"user-{userId}".ToLowerInvariant();


        public MinioStorageService(IConfiguration config)
        {
            var s3Config = new AmazonS3Config
            {
                ServiceURL = config["MinIO:ServiceURL"],
                ForcePathStyle = true
            };

            _s3Client = new AmazonS3Client(
                config["MinIO:AccessKey"],
                config["MinIO:SecretKey"],
                s3Config
            );
        }

        public async Task EnsureBucketExistsAsync(string userId)
        {
            var response = await _s3Client.ListBucketsAsync();
            if (!response.Buckets.Any(b => b.BucketName == GetUserBucket(userId)))
            {
                await _s3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = GetUserBucket(userId),
                    UseClientRegion = true
                });
            }
        }

        public async Task<Uri> UploadAsync(IFormFile file, string container, string userId, string? folder = "")
        {
            const long maxUserQuota = 200 * 1024 * 1024;
            var currentUsage = await GetBucketSizeAsync(userId);

            if (currentUsage + file.Length > maxUserQuota)
                throw new InvalidOperationException("User quota exceeded. Max 200MB per user.");

            var bucket = GetUserBucket(userId);
            await EnsureUserBucketExistsAsync(userId);

            var key = string.IsNullOrWhiteSpace(folder)
                ? file.FileName
                : $"{folder.TrimEnd('/')}/{file.FileName}";

            var request = new PutObjectRequest
            {
                BucketName = bucket,
                Key = key,
                InputStream = file.OpenReadStream()
            };

            await _s3Client.PutObjectAsync(request);

            return new Uri($"{_s3Client.Config.ServiceURL}/{bucket}/{key}");
        }



        public async Task<Stream> DownloadAsync(string fileName, string container, string userId)
        {
            var key = $"{fileName}";
            var response = await _s3Client.GetObjectAsync(GetUserBucket(userId), key);
            return response.ResponseStream;
        }

        public async Task<bool> DeleteAsync(string fileName, string container, string userId)
        {
            var key = $"{fileName}";
            var response = await _s3Client.DeleteObjectAsync(GetUserBucket(userId), key);
            return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
        }

        public async Task<IEnumerable<string>> ListAsync(string container, string userId)
        {
            var bucket = GetUserBucket(userId);

            var request = new ListObjectsV2Request
            {
                BucketName = bucket
            };

            var files = new List<string>();
            ListObjectsV2Response response;

            do
            {
                response = await _s3Client.ListObjectsV2Async(request);
                files.AddRange(response.S3Objects.Select(obj => obj.Key));
                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            return files;
        }

        public async Task<long> GetBucketSizeAsync(string userId = null)
        {
            var prefix = userId != null ? $"{userId}/" : "";

            var request = new ListObjectsV2Request
            {
                BucketName = GetUserBucket(userId),
                Prefix = prefix
            };

            long totalSize = 0;
            ListObjectsV2Response response;

            do
            {
                response = await _s3Client.ListObjectsV2Async(request);
                totalSize += response.S3Objects.Sum(obj => obj.Size);
                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            return totalSize;
        }

        public async Task EnsureUserBucketExistsAsync(string userId)
        {
            var bucketName = $"user-{userId}".ToLowerInvariant();
            var buckets = await _s3Client.ListBucketsAsync();

            if (!buckets.Buckets.Any(b => b.BucketName == bucketName))
            {
                await _s3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                });
            }
        }

        public async Task CreateFolderAsync(string folderName, string userId)
        {
            var bucket = GetUserBucket(userId);
            await EnsureUserBucketExistsAsync(userId);

            // Ensure folder name ends with slash
            if (!folderName.EndsWith("/"))
                folderName += "/";

            // Upload empty object to simulate folder
            var request = new PutObjectRequest
            {
                BucketName = bucket,
                Key = folderName,
                InputStream = new MemoryStream(new byte[0]),
                ContentType = "application/x-directory"
            };

            await _s3Client.PutObjectAsync(request);
        }

        public async Task<IEnumerable<string>> ListFoldersAsync(string userId)
        {
            var bucket = GetUserBucket(userId);
            await EnsureUserBucketExistsAsync(userId);

            var request = new ListObjectsV2Request
            {
                BucketName = bucket,
                Delimiter = "/", // this groups by folders
                Prefix = ""
            };

            var folders = new List<string>();
            ListObjectsV2Response response;

            do
            {
                response = await _s3Client.ListObjectsV2Async(request);
                folders.AddRange(response.CommonPrefixes);
                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            return folders;
        }

        public async Task MoveFileAsync(string userId, string oldPath, string newPath)
        {
            var bucket = GetUserBucket(userId);
            await EnsureUserBucketExistsAsync(userId);

            // Copy old to new
            var copyRequest = new CopyObjectRequest
            {
                SourceBucket = bucket,
                SourceKey = oldPath,
                DestinationBucket = bucket,
                DestinationKey = newPath
            };
            await _s3Client.CopyObjectAsync(copyRequest);

            // Delete old
            await _s3Client.DeleteObjectAsync(bucket, oldPath);
        }

        public async Task MoveFolderAsync(string userId, string oldFolder, string newFolder)
        {
            var bucket = GetUserBucket(userId);
            await EnsureUserBucketExistsAsync(userId);

            // Ensure both end with "/"
            oldFolder = oldFolder.TrimEnd('/') + "/";
            newFolder = newFolder.TrimEnd('/') + "/";

            var request = new ListObjectsV2Request
            {
                BucketName = bucket,
                Prefix = oldFolder
            };

            ListObjectsV2Response response;

            do
            {
                response = await _s3Client.ListObjectsV2Async(request);

                foreach (var obj in response.S3Objects)
                {
                    var oldKey = obj.Key;
                    var newKey = newFolder + oldKey.Substring(oldFolder.Length);

                    // Copy to new key
                    await _s3Client.CopyObjectAsync(new CopyObjectRequest
                    {
                        SourceBucket = bucket,
                        SourceKey = oldKey,
                        DestinationBucket = bucket,
                        DestinationKey = newKey
                    });

                    // Delete old key
                    await _s3Client.DeleteObjectAsync(bucket, oldKey);
                }

                request.ContinuationToken = response.NextContinuationToken;
            }
            while (response.IsTruncated);
        }

        public async Task DeleteFolderAsync(string userId, string folderPath)
        {
            var bucket = GetUserBucket(userId);
            await EnsureUserBucketExistsAsync(userId);

            folderPath = folderPath.TrimEnd('/') + "/";

            var request = new ListObjectsV2Request
            {
                BucketName = bucket,
                Prefix = folderPath
            };

            ListObjectsV2Response response;

            do
            {
                response = await _s3Client.ListObjectsV2Async(request);

                foreach (var obj in response.S3Objects)
                {
                    await _s3Client.DeleteObjectAsync(bucket, obj.Key);
                }

                request.ContinuationToken = response.NextContinuationToken;
            }
            while (response.IsTruncated);
        }


    }

}
