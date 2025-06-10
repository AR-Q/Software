using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Application.MediaService.Delete.Intefrace
{
    public interface IDelete
    {
        public Task<bool> handle(int mediaId);
        public Task<bool> handle(int mediaId, string name);

    }
}
