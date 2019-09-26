using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Text;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace Starcounter.MultiModelBenchmark.Benchmark
{
    public class Benchmarker
    {
        public virtual string PayloadDataFolder { get; protected set; }
        public virtual IDescription Description { get; protected set; }
        public virtual List<int> PayloadProfileIds { get; protected set; }
        public virtual List<string> PayloadProfileBodies { get; protected set; }
        public virtual List<KeyValuePair<int, int>> PayloadRelations { get; protected set; }
        public virtual int MaxParallelOperations { get; protected set; } = 16;

        public Benchmarker(string payloadDataFolder, IDescription description)
        {
            if (!Directory.Exists(payloadDataFolder))
            {
                throw new DirectoryNotFoundException(payloadDataFolder);
            }

            this.PayloadDataFolder = payloadDataFolder;
            this.Description = description ?? throw new ArgumentNullException(nameof(description));
            this.PayloadProfileIds = this.GetPayloadProfileIds();
            this.PayloadProfileBodies = this.GetPayloadProfileBodies();
            this.PayloadRelations = this.GetPayloadRelations();
        }

        public Benchmarker(string payloadDataFolder, IDescription description, int maxParallelOperations)
            : this(payloadDataFolder, description)
        {
            this.MaxParallelOperations = maxParallelOperations;
        }

        public virtual Dictionary<string, object> SingleRead()
        {
            int goal = 100000;
            List<int> ids = this.PayloadProfileIds.Take(goal).ToList();
            Stopwatch stopwatch = new Stopwatch();
            Dictionary<string, object> result = new Dictionary<string, object>();

            var benchBlock = new ActionBlock<int>(id =>
            {
                this.Description.GetDocument(id);
            },
            new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = this.MaxParallelOperations,
                EnsureOrdered = false
            });

            stopwatch.Start();

            foreach (int id in ids)
            {
                benchBlock.Post(id);
            }

            benchBlock.Complete();
            benchBlock.Completion.GetAwaiter().GetResult();
            stopwatch.Stop();

            decimal avg = (decimal)stopwatch.Elapsed.TotalMilliseconds / ids.Count;

            result.Add("Name", nameof(SingleRead));
            result.Add("Elapsed", stopwatch.Elapsed);
            result.Add("Elapsed (ms)", stopwatch.Elapsed.TotalMilliseconds);
            result.Add("Reads count", ids.Count);
            result.Add("Average per read", avg);

            return result;
        }

        public virtual async Task<Dictionary<string, object>> SingleWrite()
        {
            int goal = 100000;
            List<string> bodies = this.PayloadProfileBodies.Take(goal).ToList();
            Stopwatch stopwatch = new Stopwatch();
            Dictionary<string, object> result = new Dictionary<string, object>();

            this.Description.DropCollection();

            var benchBlock = new ActionBlock<string>(async body =>
            {
                await this.Description.SaveDocument(body);
            },
            new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = this.MaxParallelOperations,
                EnsureOrdered = false
            });

            stopwatch.Start();

            foreach (string body in bodies)
            {
                benchBlock.Post(body);
            }

            benchBlock.Complete();
            await benchBlock.Completion;
            stopwatch.Stop();

            decimal avg = (decimal)stopwatch.Elapsed.TotalMilliseconds / bodies.Count;

            result.Add("Name", nameof(SingleWrite));
            result.Add("Elapsed", stopwatch.Elapsed);
            result.Add("Elapsed (ms)", stopwatch.Elapsed.TotalMilliseconds);
            result.Add("Writes count", bodies.Count);
            result.Add("Average per write", avg);

            return result;
        }

        public virtual Dictionary<string, object> SingleWriteSync()
        {
            int goal = 100000;
            List<string> bodies = this.PayloadProfileBodies.Take(goal).ToList();
            Stopwatch stopwatch = new Stopwatch();
            Dictionary<string, object> result = new Dictionary<string, object>();

            this.Description.DropCollection();

            var benchBlock = new ActionBlock<string>(body =>
            {
                this.Description.SaveDocumentSync(body);
            },
            new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = this.MaxParallelOperations,
                EnsureOrdered = false
            });

            stopwatch.Start();

            foreach (string body in bodies)
            {
                benchBlock.Post(body);
            }

            benchBlock.Complete();
            benchBlock.Completion.GetAwaiter().GetResult();
            stopwatch.Stop();

            decimal avg = (decimal)stopwatch.Elapsed.TotalMilliseconds / bodies.Count;

            result.Add("Name", nameof(SingleWriteSync));
            result.Add("Elapsed", stopwatch.Elapsed);
            result.Add("Elapsed (ms)", stopwatch.Elapsed.TotalMilliseconds);
            result.Add("Writes count", bodies.Count);
            result.Add("Average per write", avg);

            return result;
        }

        public virtual Dictionary<string, object> Aggregation()
        {
            Stopwatch stopwatch = new Stopwatch();
            Dictionary<string, object> result = new Dictionary<string, object>();

            stopwatch.Start();
            this.Description.Aggregate().ToList();
            stopwatch.Stop();

            result.Add("Name", nameof(Aggregation));
            result.Add("Elapsed", stopwatch.Elapsed);
            result.Add("Elapsed (ms)", stopwatch.Elapsed.TotalMilliseconds);

            return result;
        }

        public virtual Dictionary<string, object> Neighbors()
        {
            int goal = 1000;
            long totalCount = 0;
            List<int> ids = this.PayloadProfileIds.Take(goal).ToList();
            Stopwatch stopwatch = new Stopwatch();
            Dictionary<string, object> result = new Dictionary<string, object>();

            var benchBlock = new TransformBlock<int, int>(id =>
            {
                return this.Description.Neighbors(id);
            },
            new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = this.MaxParallelOperations,
                EnsureOrdered = false
            });

            var aggregateBlock = new ActionBlock<int>(count =>
            {
                totalCount += count;
            });

            benchBlock.LinkTo(aggregateBlock, new DataflowLinkOptions() { PropagateCompletion = true });

            stopwatch.Start();

            foreach (int id in ids)
            {
                benchBlock.Post(id);
            }

            benchBlock.Complete();
            aggregateBlock.Completion.GetAwaiter().GetResult();
            stopwatch.Stop();

            decimal avg = (decimal)stopwatch.Elapsed.TotalMilliseconds / ids.Count;

            result.Add("Name", nameof(Neighbors));
            result.Add("Elapsed", stopwatch.Elapsed);
            result.Add("Elapsed (ms)", stopwatch.Elapsed.TotalMilliseconds);
            result.Add("Searches count", ids.Count);
            result.Add("Found nodes count", totalCount);
            result.Add("Average per search", avg);
            result.Add("IMPORTANT", "Arango calcs average \"per node found\", not per actual request, which is not correct.");

            return result;
        }

        public virtual Dictionary<string, object> Neighbors2()
        {
            int goal = 1000;
            long totalCount = 0;
            List<int> ids = this.PayloadProfileIds.Take(goal).ToList();
            Stopwatch stopwatch = new Stopwatch();
            Dictionary<string, object> result = new Dictionary<string, object>();

            var benchBlock = new TransformBlock<int, int>(id =>
            {
                return this.Description.Neighbors2(id);
            },
            new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = this.MaxParallelOperations,
                EnsureOrdered = false
            });

            var aggregateBlock = new ActionBlock<int>(count =>
            {
                totalCount += count;
            });

            benchBlock.LinkTo(aggregateBlock, new DataflowLinkOptions() { PropagateCompletion = true });

            stopwatch.Start();

            foreach (int id in ids)
            {
                benchBlock.Post(id);
            }

            benchBlock.Complete();
            aggregateBlock.Completion.GetAwaiter().GetResult();
            stopwatch.Stop();

            decimal avg = (decimal)stopwatch.Elapsed.TotalMilliseconds / ids.Count;

            result.Add("Name", nameof(Neighbors2));
            result.Add("Elapsed", stopwatch.Elapsed);
            result.Add("Elapsed (ms)", stopwatch.Elapsed.TotalMilliseconds);
            result.Add("Searches count", ids.Count);
            result.Add("Found nodes count", totalCount);
            result.Add("Average per search", avg);
            result.Add("IMPORTANT", "Arango calcs average \"per node found\", not per actual request, which is not correct.");

            return result;
        }

        public virtual Dictionary<string, object> Neighbors2Data()
        {
            int goal = 100;
            long totalCount = 0;
            List<int> ids = this.PayloadProfileIds.Take(goal).ToList();
            Stopwatch stopwatch = new Stopwatch();
            Dictionary<string, object> result = new Dictionary<string, object>();

            var benchBlock = new TransformBlock<int, int>(id =>
            {
                return this.Description.Neighbors2Data(id);
            },
            new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = this.MaxParallelOperations,
                EnsureOrdered = false
            });

            var aggregateBlock = new ActionBlock<int>(count =>
            {
                totalCount += count;
            });

            benchBlock.LinkTo(aggregateBlock, new DataflowLinkOptions() { PropagateCompletion = true });

            stopwatch.Start();

            foreach (int id in ids)
            {
                benchBlock.Post(id);
            }

            benchBlock.Complete();
            aggregateBlock.Completion.GetAwaiter().GetResult();
            stopwatch.Stop();

            decimal avg = (decimal)stopwatch.Elapsed.TotalMilliseconds / ids.Count;

            result.Add("Name", nameof(Neighbors2Data));
            result.Add("Elapsed", stopwatch.Elapsed);
            result.Add("Elapsed (ms)", stopwatch.Elapsed.TotalMilliseconds);
            result.Add("Searches count", ids.Count);
            result.Add("Found nodes count", totalCount);
            result.Add("Average per search", avg);
            result.Add("IMPORTANT", "Arango calcs average \"per node found\", not per actual request, which is not correct.");

            return result;
        }

        public virtual Dictionary<string, object> ShortestPath()
        {
            int goal = 1000;
            long totalCount = 0;
            List<KeyValuePair<int, int>> relations = this.PayloadRelations.Take(goal).ToList();
            Dictionary<string, object> result = new Dictionary<string, object>();

            Stopwatch stopwatch = new Stopwatch();
            List<int> results = new List<int>();
            var benchBlock = new TransformBlock<KeyValuePair<int, int>, int>((KeyValuePair<int, int> relation) =>
            {
                return this.Description.ShortestPath(relation.Key, relation.Value);
            },
            new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = this.MaxParallelOperations,
                EnsureOrdered = false
            });

            var aggregateBlock = new ActionBlock<int>(response =>
            {
                results.Add(response);
            });

            benchBlock.LinkTo(aggregateBlock, new DataflowLinkOptions() { PropagateCompletion = true });

            stopwatch.Start();

            foreach (var relation in relations)
            {
                benchBlock.Post(relation);
            }

            benchBlock.Complete();
            aggregateBlock.Completion.GetAwaiter().GetResult();
            stopwatch.Stop();

            totalCount = results.Sum();
            TimeSpan elapsed = stopwatch.Elapsed;

            decimal avg = (decimal)stopwatch.Elapsed.TotalMilliseconds / relations.Count;

            result.Add("Name", nameof(ShortestPath));
            result.Add("Elapsed", stopwatch.Elapsed);
            result.Add("Elapsed (ms)", stopwatch.Elapsed.TotalMilliseconds);
            result.Add("Searches count", relations.Count);
            result.Add("Total found edges", totalCount);
            result.Add("Average per search", avg);

            return result;
        }

        public virtual Dictionary<string, object> HardPath()
        {
            int from = 349622;
            int to = 1625331;
            Stopwatch stopwatch = new Stopwatch();
            Dictionary<string, object> result = new Dictionary<string, object>();

            stopwatch.Start();
            int length = this.Description.ShortestPath(from, to);
            stopwatch.Stop();

            result.Add("Name", nameof(HardPath));
            result.Add("Elapsed", stopwatch.Elapsed);
            result.Add("Elapsed (ms)", stopwatch.Elapsed.TotalMilliseconds);
            result.Add("Searches count", 1);
            result.Add("Total found edges", length);

            return result;
        }

        protected virtual List<int> GetPayloadProfileIds()
        {
            List<int> ids;
            List<string> pids;
            string path = Path.Combine(this.PayloadDataFolder, "ids100000.json");

            using (TextReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                pids = JsonConvert.DeserializeObject<List<string>>(json);
            }

            ids = pids.Select(x => int.Parse(x.Substring(1))).ToList();

            return ids;
        }

        public virtual List<string> GetPayloadProfileBodies()
        {
            List<string> bodies;
            List<object> profiles;
            string path = Path.Combine(this.PayloadDataFolder, "bodies100000.json");

            using (TextReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                profiles = JsonConvert.DeserializeObject<List<object>>(json);
            }

            bodies = profiles.Select(x => JsonConvert.SerializeObject(x)).ToList();

            return bodies;
        }

        public virtual List<KeyValuePair<int, int>> GetPayloadRelations()
        {
            List<Dictionary<string, string>> parced;
            List<KeyValuePair<int, int>> relations;
            string path = Path.Combine(this.PayloadDataFolder, "shortest1000.json");

            using (TextReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                parced = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
            }

            relations = parced.Select(x => new KeyValuePair<int, int>(int.Parse(x["from"].Substring(1)), int.Parse(x["to"].Substring(1)))).ToList();

            return relations;
        }
    }
}
