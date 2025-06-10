using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application.Common.Hash
{
    public interface IHasher
    {
        string Hash(string key);
        bool Compare(string key, string hashedPassword);
    }
}
