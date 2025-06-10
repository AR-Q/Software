using Media.Application.Common;
using Media.Application.MediaService.Save.Interface;
using Media.Contracts;
using Media.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Application.MediaService.Save
{
    public class Save : ISave
    {
        private readonly IMediaRepository _mediaRepository;
        public Save(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;
        }

        public async Task<string> handle(FileVM file, int mediaId)
        {
            var myUniqueFileName = string.Format(@"{0}", Guid.NewGuid());
            file.Name = myUniqueFileName;

            int max = _mediaRepository.GetLastId();
            // create media object to insert
            var media = new Files()
            {
                Content = file.Content,
                CreateDate = DateTime.Now,
                DataType = file.ContentType,
                MediaID = mediaId,
                FileName = file.FileName,
                Name = file.Name,
                Size = Convert.ToInt32(file.Content.Length),
                _id = max
            };
            // save the file to the correct table(using Content_Type attribute)
            bool res = await _mediaRepository.Add(media);
            if (res)
            {
                return myUniqueFileName; 
            }
            else
            {
                return "error";
            }
        }
    }
}
