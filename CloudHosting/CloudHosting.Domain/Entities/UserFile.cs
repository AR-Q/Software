using System;

namespace CloudHosting.Domain.Entities
{
    public class UserFile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FileName { get; set; }
        public string StoragePath { get; set; }
        public string ContentType { get; set; }
        public long SizeInBytes { get; set; }
        public DateTime UploadedAt { get; set; }
        public bool IsPublic { get; set; }
    }
}