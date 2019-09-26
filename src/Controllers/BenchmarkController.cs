using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Starcounter.MultiModelBenchmark.Benchmark;

namespace Starcounter.MultiModelBenchmark.Controllers
{
    public class BenchmarkController : Controller
    {
        private static bool IsBenchmarking = false;
        private static string LastBenchmark = null;
        private static object LastBenchmarkResult = null;
        private static DateTime? LastBenchmarkStartedAt = null;
        private static DateTime? LastBenchmarkCompletedAt = null;

        private IDescription description;

        public BenchmarkController(IDescription description)
        {
            this.description = description;
        }

        [Route("/starcounter-mmb/benchmark/status")]
        public string Status()
        {
            object status = new
            {
                IsBenchmarking,
                LastBenchmark,
                LastBenchmarkResult,
                LastBenchmarkStartedAt,
                LastBenchmarkCompletedAt
            };

            return JsonConvert.SerializeObject(status, Formatting.Indented);
        }

        [Route("/starcounter-mmb/benchmark/{name}/{maxParallelOperations}")]
        public string Run(string name, int maxParallelOperations)
        {
            if (IsBenchmarking)
            {
                return $"Other benchmark ({LastBenchmark}) is still running";
            }

            IsBenchmarking = true;
            LastBenchmark = $"{name} with {maxParallelOperations} max parallel operation(s).";
            LastBenchmarkStartedAt = DateTime.Now;
            LastBenchmarkCompletedAt = null;

            FileInfo fi = new FileInfo(typeof(Benchmarker).Assembly.Location);
            string payloadDataFolder = Path.Combine(fi.Directory.FullName, "../../../../data");

            Benchmarker benchmarker = new Benchmarker(payloadDataFolder, description, maxParallelOperations);

            Task.Run(async () =>
            {
                object result;

                try
                {
                    switch (name)
                    {
                        case nameof(Benchmarker.ShortestPath):
                            result = benchmarker.ShortestPath();
                            break;
                        case nameof(Benchmarker.HardPath):
                            result = benchmarker.HardPath();
                            break;
                        case nameof(Benchmarker.Neighbors):
                            result = benchmarker.Neighbors();
                            break;
                        case nameof(Benchmarker.Neighbors2):
                            result = benchmarker.Neighbors2();
                            break;
                        case nameof(Benchmarker.Neighbors2Data):
                            result = benchmarker.Neighbors2Data();
                            break;
                        case nameof(Benchmarker.SingleRead):
                            result = benchmarker.SingleRead();
                            break;
                        case nameof(Benchmarker.SingleWrite):
                            result = await benchmarker.SingleWrite();
                            break;
                        case nameof(Benchmarker.SingleWriteSync):
                            result = benchmarker.SingleWriteSync();
                            break;
                        case nameof(Benchmarker.Aggregation):
                            result = benchmarker.Aggregation();
                            break;
                        case "All":
                            List<object> results = new List<object>();

                            LastBenchmark = $"All -> {nameof(Benchmarker.ShortestPath)} with {maxParallelOperations} max parallel operation(s).";
                            results.Add(benchmarker.ShortestPath());

                            LastBenchmark = $"All -> {nameof(Benchmarker.Neighbors)} with {maxParallelOperations} max parallel operation(s).";
                            results.Add(benchmarker.Neighbors());

                            LastBenchmark = $"All -> {nameof(Benchmarker.Neighbors2)} with {maxParallelOperations} max parallel operation(s).";
                            results.Add(benchmarker.Neighbors2());

                            LastBenchmark = $"All -> {nameof(Benchmarker.Neighbors2Data)} with {maxParallelOperations} max parallel operation(s).";
                            results.Add(benchmarker.Neighbors2Data());

                            LastBenchmark = $"All -> {nameof(Benchmarker.SingleRead)} with {maxParallelOperations} max parallel operation(s).";
                            results.Add(benchmarker.SingleRead());

                            LastBenchmark = $"All -> {nameof(Benchmarker.SingleWrite)} with {maxParallelOperations} max parallel operation(s).";
                            results.Add(await benchmarker.SingleWrite());

                            LastBenchmark = $"All -> {nameof(Benchmarker.Aggregation)} with {maxParallelOperations} max parallel operation(s).";
                            results.Add(benchmarker.Aggregation());

                            LastBenchmark = $"All -> {nameof(Benchmarker.HardPath)} with {maxParallelOperations} max parallel operation(s).";
                            results.Add(benchmarker.HardPath());

                            LastBenchmark = $"All -> {nameof(Benchmarker.SingleWriteSync)} with {maxParallelOperations} max parallel operation(s).";
                            results.Add(benchmarker.SingleWriteSync());

                            result = results;
                            break;
                        default:
                            result = $"Invalid benchmark name. Choose one of the following: [All, {nameof(Benchmarker.ShortestPath)}, {nameof(Benchmarker.HardPath)}, {nameof(Benchmarker.Neighbors)}, {nameof(Benchmarker.Neighbors2)}, {nameof(Benchmarker.Neighbors2Data)}, {nameof(Benchmarker.SingleRead)}, {nameof(Benchmarker.SingleWrite)}, {nameof(Benchmarker.SingleWriteSync)}, {nameof(Benchmarker.Aggregation)}].";
                            break;
                    }
                }
                catch (Exception ex)
                {
                    result = new
                    {
                        ex.Message,
                        ex.StackTrace
                    };
                }

                LastBenchmarkResult = result;
                LastBenchmarkCompletedAt = DateTime.Now;
                IsBenchmarking = false;
            });

            return "Benchmark has started. Check result at /starcounter-mmb/benchmark/status.";
        }
    }
}