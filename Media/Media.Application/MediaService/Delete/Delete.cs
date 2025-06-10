using Media.Application.Common;
using Media.Application.MediaService.Delete.Intefrace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Application.MediaService.Delete
{
    public class Delete : IDelete
    {
        private readonly IMediaRepository _mediaRepository;

        public Delete(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        public async Task<bool> handle(int mediaId)
        {
            return await _mediaRepository.Delete(mediaId);
        }

        public Task<bool> handle(int mediaId, string name)
        {
            throw new NotImplementedException();
        }

    }
}
