using System;
using Starcounter.Nova;

namespace Starcounter.MultiModelBenchmark
{
    [Database]
    public abstract class Relation
    {
        public abstract Profile From { get; set; }
        public abstract Profile To { get; set; }
    }
}
