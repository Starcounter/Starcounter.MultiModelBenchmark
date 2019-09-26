using System;
using Newtonsoft.Json;
using Starcounter.Nova;

namespace Starcounter.MultiModelBenchmark
{
    [Database]
    public abstract class Profile : ProfileBase
    {
        [JsonIgnore]
        public abstract byte[] FollowNos { get; set; }
    }
}
