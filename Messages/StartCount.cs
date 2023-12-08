using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorMapReduceWordCount.Messages
{
    public class StartCount
    {
        public readonly String FileName;

        public StartCount(String file) { FileName = file; }
    }
}