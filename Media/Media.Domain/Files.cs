using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Domain
{
    public class Files
    {
        public int _id { get; set; }

        public string Name { get; set; }

        public string FileName { get; set; }
        public string DataType { get; set; }
        public Stream Content { get; set; }
        public DateTime CreateDate { get; set; }
        public int Size { get; set; }


        //foreign key to media in core
        public int MediaID { get; set; }
    }
}
