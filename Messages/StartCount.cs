﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorMapReduceWordCount.Messages;
public class StartCount(String file)
{
    public readonly String FileName = file;
}