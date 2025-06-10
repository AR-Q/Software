using IAM.Application.Common.Hash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Infrastructure.Hash
{
    public class Hasher : IHasher
    {
        public bool Compare(string key, string hashedPassword)
        {
            if (Hash(key) == hashedPassword)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string Hash(string key)
        {
            //SHA256 sha256 = SHA256Managed.Create();
            //byte[] hashValue;
            //UTF8Encoding objUtf8 = new UTF8Encoding();
            //hashValue = sha256.ComputeHash(objUtf8.GetBytes(key));

            //return Convert.ToString(hashValue);

            var inputBytes = Encoding.UTF8.GetBytes(key);
            var inputHash = SHA256.HashData(inputBytes);
            return Convert.ToHexString(inputHash);
        }
    }
}
