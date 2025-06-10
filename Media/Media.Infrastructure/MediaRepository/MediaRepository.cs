using Media.Application.Common;
using Media.Contracts;
using Media.Domain;
using Media.Infrastructure.MediaRepository.Mongo.Interface;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Media.Infrastructure.MediaRepository
{
    public class MediaRepository : IMediaRepository
    {
        private readonly IMongoService _mongoService;

        public MediaRepository(IMongoService mongoService)
        {
            _mongoService = mongoService;
        }
        public async Task<bool> Add(Files file)
        {
            var doc = await _mongoService.Create(file);
            try
            {
                await _mongoService.Insert(doc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        public FileVM CreateMedia(BsonDocument file)
        {
            FileVM fileVM = new FileVM();
            fileVM.Name = file[2].ToString();
            fileVM.FileName = file[3].ToString();
            fileVM.ContentType = file[4].ToString();
            byte[] stream = file[5].AsByteArray;
            fileVM.Content = new MemoryStream(stream);
            return fileVM;
        }

        public async Task<bool> Delete(int mediaId)
        {
            bool res = await _mongoService.DeleteAll(mediaId);
            return res;
        }

        public async Task<BsonDocument> GetDoc(int mediaId, string name)
        {
            BsonDocument doc = await _mongoService.Find(mediaId, name);
            return doc;
        }

        public int GetLastId()
        {
            var docs = _mongoService.GetAll();
            int max = 0;
            foreach (var VARIABLE in docs)
            {
                Console.WriteLine(VARIABLE[0]);
                if (VARIABLE[0].AsInt32 > max)
                {
                    max = VARIABLE[0].AsInt32;
                }

            }

            return max + 1;
        }
    }
}
