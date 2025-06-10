using Media.Application.Common;
using Media.Application.MediaService.Get.Interface;
using Media.Contracts;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Application.MediaService.Get
{
    public class Get : IGet
    {
        private readonly IMediaRepository _mediaRepository;

        public Get(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }
        public async Task<FileVM> handle(int mediaId, string name)
        {
            BsonDocument? file = await _mediaRepository.GetDoc(mediaId, name);
            if (file is null)
            {
                return null;
            }
            // create media File
            FileVM mediaFile = _mediaRepository.CreateMedia(file);
            // return media file
            return mediaFile;
        }
    }
}
