using Media.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Application.MediaService.Save.Interface
{
    public interface ISave
    {
        Task<string> handle(FileVM file, int mediaId);
    }
}
