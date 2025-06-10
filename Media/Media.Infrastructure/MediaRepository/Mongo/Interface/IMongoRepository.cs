using Media.Domain;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Infrastructure.MediaRepository.Mongo.Interface
{
    public interface IMongoService
    {
        Task Insert(BsonDocument document);
        Task<BsonDocument> Create(Files file);
        List<BsonDocument> GetAll();
        Task<BsonDocument> Find(int mediaId, string name);
        Task<bool> Delete(int mediaId, string name);
        Task<bool> DeleteAll(int mediaId);
    }
}
