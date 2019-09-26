using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Starcounter.Nova;
using Starcounter.Nova.Hosting;
using Starcounter.MultiModelBenchmark.Benchmark;

namespace Starcounter.MultiModelBenchmark.Controllers
{
    [Route("/starcounter-mmb/description")]
    [ApiController]
    public class DescriptionController : ControllerBase
    {
        private readonly IDescription description;

        public DescriptionController(IDescription description)
        {
            this.description = description;
        }

        [HttpGet("get-version")]
        public string GetVersion()
        {
            return "Starcounter.3.0.0-*";
        }

        [HttpPost("drop-collection")]
        public void DropCollection()
        {
            description.DropCollection();
        }

        [HttpGet("load-relations")]
        public string LoadRelations()
        {
            string result = description.LoadRelations();
            return result;
        }

        [HttpGet("get-document/{key}")]
        public string GetDocument(int key)
        {
            string result = description.GetDocument(key);
            return result;
        }

        // ArangoDB Benchmark.js specifies profile keys as "P360247".
        [HttpGet("get-document-p/{pKey}")]
        public string GetDocument(string pKey)
        {
            var key = int.Parse(pKey.Substring(1));
            string result = description.GetDocument(key);
            return result;
        }

        [HttpPost("save-document")]
        public async Task<string> SaveDocument()
        {
            TextReader reader = new StreamReader(Request.Body);
            string body = await reader.ReadToEndAsync();
            reader.Dispose();

            string result = await description.SaveDocument(body);
            return result;
        }

        [HttpPost("save-document-sync")]
        public async Task<string> SaveDocumentSync()
        {
            TextReader reader = new StreamReader(Request.Body);
            string body = await reader.ReadToEndAsync();
            reader.Dispose();

            string result = description.SaveDocumentSync(body);
            return result;
        }

        [HttpGet("aggregate")]
        public string Aggregate()
        {
            string result = description.Aggregate();
            return result;
        }

        [HttpGet("neighbors/{key}")]
        public int Neighbors(int key)
        {
            int result = description.Neighbors(key);
            return result;
        }

        // ArangoDB Benchmark.js specifies profile keys as "P360247".
        [HttpGet("neighbors-p/{pKey}")]
        public int Neighbors(string pKey)
        {
            var key = int.Parse(pKey.Substring(1));
            int result = description.Neighbors(key);
            return result;
        }

        [HttpGet("neighbors-2/{key}")]
        public int Neighbors2(int key)
        {
            int result = description.Neighbors2(key);
            return result;
        }

        // ArangoDB Benchmark.js specifies profile keys as "P360247".
        [HttpGet("neighbors-p-2/{pKey}")]
        public int Neighbors2(string pKey)
        {
            var key = int.Parse(pKey.Substring(1));
            int result = description.Neighbors2(key);
            return result;
        }

        [HttpGet("neighbors-2-data/{key}")]
        public int Neighbors2Data(int key)
        {
            int result = description.Neighbors2Data(key);
            return result;
        }

        // ArangoDB Benchmark.js specifies profile keys as "P360247".
        [HttpGet("neighbors-p-2-data/{pKey}")]
        public int Neighbors2Data(string pKey)
        {
            var key = int.Parse(pKey.Substring(1));
            int result = description.Neighbors2Data(key);
            return result;
        }

        [HttpGet("neighbors-3/{key}")]
        public int Neighbors3(int key)
        {
            int result = description.Neighbors3(key);
            return result;
        }

        [HttpGet("shortest-path/{fromKey}/{toKey}")]
        public int ShortestPath(int fromKey, int toKey)
        {
            int result = description.ShortestPath(fromKey, toKey);
            return result;
        }

        // ArangoDB Benchmark.js specifies profile keys as "P360247".
        [HttpGet("shortest-path-p/{pFromKey}/{pToKey}")]
        public int ShortestPath(string pFromKey, string pToKey)
        {
            var fromKey = int.Parse(pFromKey.Substring(1));
            var toKey = int.Parse(pToKey.Substring(1));
            int result = description.ShortestPath(fromKey, toKey);
            return result;
        }

        [HttpGet("shortest-path-with-steps/{fromKey}/{toKey}")]
        public int[] ShortsetPathWithSteps(int fromKey, int toKey)
        {
            int[] result = description.ShortestPathWithSteps(fromKey, toKey);
            return result;
        }
    }
}
