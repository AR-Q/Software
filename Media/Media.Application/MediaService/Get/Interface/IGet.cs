using Media.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Application.MediaService.Get.Interface
{
    public interface IGet
    {
        public Task<FileVM> handle(int mediaId, string name);
    }
}
