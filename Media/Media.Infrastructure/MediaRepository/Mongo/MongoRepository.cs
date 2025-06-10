using Media.Domain;
using Media.Infrastructure.MediaRepository.Mongo.Interface;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Infrastructure.MediaRepository.Mongo
{
    public class MongoService : IMongoService
    {

        private static MongoClient client = new MongoClient("mongodb://127.0.0.1:27017/?directConnection=true&serverSelectionTimeoutMS=2000&appName=mongosh+2.3.4");
        private static IMongoDatabase database = client.GetDatabase("Turbo");
        private static IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("Files");
        public async Task<BsonDocument> Create(Files file)
        {
            var memory = new MemoryStream();
            await file.Content.CopyToAsync(memory);
            byte[] image = memory.ToArray();
            BsonDocument document = new BsonDocument
            {
                {"_id",file._id},
                {"MediaID",file.MediaID},
                {"Name",file.Name},
                {"File_name",file.FileName},
                {"DataType",file.DataType},
                {"Content",image},
                {"Size",image.Length},
                {"CreateDate",DateTime.Now},
            };
            return document;
        }

        public async Task<bool> Delete(int mediaId, string name)
        {
            var filter1 = Builders<BsonDocument>.Filter.Eq("MediaID", mediaId);
            var filter2 = Builders<BsonDocument>.Filter.Eq("Name", name);
            var combined = Builders<BsonDocument>.Filter.And(filter1, filter2);
            await collection.DeleteManyAsync(combined);
            return true;
        }

        public async Task<bool> DeleteAll(int mediaId)
        {
            var deletefilter = Builders<BsonDocument>.Filter.Eq("mediaID", mediaId);
            await collection.DeleteManyAsync(deletefilter);
            return true;
        }

        public async Task<BsonDocument> Find(int mediaId, string name)
        {
            var filter1 = Builders<BsonDocument>.Filter.Eq("MediaID", mediaId);
            var filter2 = Builders<BsonDocument>.Filter.Eq("Name", name);
            var combined = Builders<BsonDocument>.Filter.And(filter1,filter2);
            BsonDocument doc = collection.Find(combined).FirstOrDefault().ToBsonDocument();
            return doc;
        }

        public List<BsonDocument> GetAll()
        {
            var docs = collection.Find(new BsonDocument()).ToList();
            return docs;
        }

        public async Task Insert(BsonDocument document)
        {
            await collection.InsertOneAsync(document);
        }
    }
}
