using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application.Common.Repositories
{
    public interface ICachingContext
    {
        void Set(string phone, string code);
        string Get(string phone);
    }
}
