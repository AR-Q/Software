using IAM.Application.Common.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Infrastructure.Code
{
    public class CodeGenerator : ICodeGenerator
    {
        public string GenerateCode()
        {
            Random random = new Random();
            int num = random.Next(100000, 999999);
            return Convert.ToString(num);
        }
    }
}
