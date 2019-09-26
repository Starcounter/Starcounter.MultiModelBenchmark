using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Starcounter.Nova;
using Starcounter.Nova.Hosting;

namespace Starcounter.MultiModelBenchmark.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITransactor transactor;
        private readonly IDdlExecutor ddlExecutor;

        public HomeController(ITransactor transactor, IDdlExecutor ddlExecutor)
        {
            this.transactor = transactor;
            this.ddlExecutor = ddlExecutor;
        }

        public string Index()
        {
            return "Starcounter.MultiModelBenchmark app.";
        }

        [Route("/starcounter-mmb/status")]
        public string Status()
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            List<Task> tasks = new List<Task>();

            tasks.Add(transactor.TransactAsync(db =>
            {
                long count = db.Sql<long>("SELECT COUNT(p) FROM Starcounter.MultiModelBenchmark.Profile p").First();
                values.Add("Profiles", count);
            }));

            tasks.Add(transactor.TransactAsync(db =>
            {
                long count = db.Sql<long>("SELECT COUNT(p) FROM Starcounter.MultiModelBenchmark.ProfileTemp p").First();
                values.Add("ProfileTemps", count);
            }));

            tasks.Add(transactor.TransactAsync(db =>
            {
                long count = db.Sql<long>("SELECT COUNT(r) FROM Starcounter.MultiModelBenchmark.Relation r").First();
                values.Add("Relations", count);
            }));

            tasks.Add(transactor.TransactAsync(db =>
            {
                long count = 0;

                foreach (Profile p in db.Sql<Profile>("SELECT p FROM Starcounter.MultiModelBenchmark.Profile p"))
                {
                    count += p.FollowNos.Length / sizeof(ulong);
                }

                values.Add("FollowNos", count);
            }));

            Task.WaitAll(tasks.ToArray());

            string json = JsonConvert.SerializeObject(values, Formatting.Indented);

            return json;
        }

        [HttpPost("/starcounter-mmb/import-profiles/{*filePath}")]
        public string ImportProfiles(string filePath)
        {
            Importer importer = new Importer(this.transactor);
            int count = importer.ImportProfiles(filePath);

            return $"Imported {count} profile(s). Expected number of profiles: 1632803.";
        }

        [HttpPost("/starcounter-mmb/import-relations/{*filePath}")]
        public string ImportRelations(string filePath)
        {
            Importer importer = new Importer(this.transactor);
            int count = importer.ImportRelations(filePath);

            return $"Imported {count} relation(s). Expected number of relations: 30622564.";
        }

        [HttpPost("/starcounter-mmb/create-indexes")]
        public string CreateIndexes()
        {
            this.ddlExecutor.Execute("CREATE UNIQUE INDEX IX_Starcounter_MultiModelBenchmark_Profile_Key ON Starcounter.MultiModelBenchmark.Profile (Key)");
            this.ddlExecutor.Execute("CREATE INDEX IX_Starcounter_MultiModelBenchmark_Profile_Age ON Starcounter.MultiModelBenchmark.Profile (Age)");

            this.ddlExecutor.Execute("CREATE UNIQUE INDEX IX_Starcounter_MultiModelBenchmark_Relation_From_To ON Starcounter.MultiModelBenchmark.Relation (\"From\", To)");
            this.ddlExecutor.Execute("CREATE INDEX IX_Starcounter_MultiModelBenchmark_Relation_From ON Starcounter.MultiModelBenchmark.Relation (\"From\")");
            this.ddlExecutor.Execute("CREATE INDEX IX_Starcounter_MultiModelBenchmark_Relation_To ON Starcounter.MultiModelBenchmark.Relation (To)");

            return "Database Indexes have been successfully created.";
        }
    }
}