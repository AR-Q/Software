using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Media.Domain;
using Media.Contracts;

namespace Media.Application.Common
{
    public interface IMediaRepository
    {
        public Task<bool> Add(Files file);
        public int GetLastId();
        public Task<BsonDocument> GetDoc(int mediaId, string name);
        public FileVM CreateMedia(BsonDocument file);
        public Task<bool> Delete(int mediaId);
    }
}
