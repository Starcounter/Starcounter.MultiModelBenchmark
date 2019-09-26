using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Starcounter.Nova;
using Starcounter.Nova.Hosting;
using Starcounter.Nova.Abstractions;
using Starcounter.MultiModelBenchmark.Benchmark;

namespace Starcounter.MultiModelBenchmark
{
    public class Description : IDescription
    {
        private readonly ITransactor transactor;

        public Description(ITransactor transactor)
        {
            this.transactor = transactor;
        }

        public struct LoadRelationResultItem
        {
            [JsonProperty("lastdigit")]
            public int LastDigit;

            [JsonProperty("count")]
            public int Count;

            public LoadRelationResultItem(int lastDigit, int count)
            {
                this.LastDigit = lastDigit;
                this.Count = count;
            }
        }

        public struct AggregateResultItem
        {
            [JsonProperty("age")]
            public int? Age;

            [JsonProperty("count")]
            public int Count;

            public AggregateResultItem(int? age, int count)
            {
                this.Age = age;
                this.Count = count;
            }
        }

        public virtual string GetVersion()
        {
            return "Starcounter 3.0.0-alpha-*";
        }

        public virtual void DropCollection()
        {
            this.transactor.Transact(db =>
            {
                db.Sql("DELETE FROM Starcounter.MultiModelBenchmark.ProfileTemp");
            });
        }

        public virtual string LoadRelations()
        {
            string result = null;

            this.transactor.Transact(db =>
            {
                IEnumerable<Relation> relations = db.Sql<Relation>("SELECT r FROM Starcounter.MultiModelBenchmark.Relation r");
                var groups = relations.GroupBy(x =>
                {
                    int last = x.From.Key % 10;
                    return last;
                });

                result = JsonConvert.SerializeObject(groups.Select(x => new LoadRelationResultItem(x.Key, x.Count())));
            });

            return result;
        }

        public virtual string GetDocument(int key)
        {
            string result = null;

            this.transactor.Transact(db =>
            {
                Profile profile = db.Sql<Profile>("SELECT p FROM Starcounter.MultiModelBenchmark.Profile p WHERE p.Key = ?", key).FirstOrDefault();
                result = JsonConvert.SerializeObject(profile);
            });

            return result;
        }

        public virtual async Task<string> SaveDocument(string json)
        {
            string result = null;

            result = await this.transactor.TransactAsync(db =>
            {
                ProfileTemp temp = db.Insert<ProfileTemp>();
                JsonConvert.PopulateObject(json, temp);
                return JsonConvert.SerializeObject(temp);
            });

            return result;
        }

        public virtual string SaveDocumentSync(string json)
        {
            string result = null;

            this.transactor.Transact(db =>
            {
                ProfileTemp temp = db.Insert<ProfileTemp>();
                JsonConvert.PopulateObject(json, temp);
                result = JsonConvert.SerializeObject(temp);
            });

            return result;
        }

        public virtual string Aggregate()
        {
            string json = null;

            this.transactor.Transact(db =>
            {
                int minAge = db.Sql<int?>("SELECT p.Age FROM Starcounter.MultiModelBenchmark.Profile p WHERE p.Age IS NOT NULL ORDER BY p.Age FETCH ?", 1).First().Value;
                int maxAge = db.Sql<int?>("SELECT p.Age FROM Starcounter.MultiModelBenchmark.Profile p WHERE p.Age IS NOT NULL ORDER BY p.Age DESC FETCH ?", 1).First().Value;

                List<AggregateResultItem> result = new List<AggregateResultItem>();

                {
                    long count = db.Sql<long>("SELECT COUNT(p) FROM Starcounter.MultiModelBenchmark.Profile p WHERE p.Age IS NULL").First();

                    if (count > 0)
                    {
                        result.Add(new AggregateResultItem(null, (int)count));
                    }
                }

                for (int i = minAge; i <= maxAge; i++)
                {
                    long? count = db.Sql<long?>("SELECT COUNT(p) FROM Starcounter.MultiModelBenchmark.Profile p WHERE p.Age = ?", i).First();

                    if (count == null || count < 1)
                    {
                        continue;
                    }

                    result.Add(new AggregateResultItem(i, (int)count));
                }

#if false
                // Starcounter does not yet utilize IX_Starcounter_MultiModelBenchmark_Profile_Age index with GROUP BY query.
                IEnumerable<Starcounter.Query.Execution.RowImpl> rows = Db.SQL<Starcounter.Query.Execution.RowImpl>("SELECT p.Age, COUNT(p) FROM Starcounter.MultiModelBenchmark.Profile p GROUP BY p.Age");
                List<AggregateResultItem> list = rows.Select(x => new AggregateResultItem((int?)x.GetInt64(0), (int)x.GetInt64(1).Value)).ToList();
                result = JsonConvert.SerializeObject(list);
#endif
                json = JsonConvert.SerializeObject(result);
            });

            return json;
        }

        public virtual int Neighbors(int id)
        {
            int result = 0;

            this.transactor.Transact(db =>
            {
                Profile profile = db.Sql<Profile>("SELECT p FROM Starcounter.MultiModelBenchmark.Profile p WHERE p.Key = ?", id).First();
                result = this.SelectNeighborsLevel1(db, profile)
                    .Select(x => x.Key)
                    .Count();
            });

            return result;
        }

        public virtual int Neighbors2(int id)
        {
            int result = 0;

            this.transactor.Transact(db =>
            {
                Profile profile = db.Sql<Profile>("SELECT p FROM Starcounter.MultiModelBenchmark.Profile p WHERE p.Key = ?", id).First();
                result = this.SelectNeighborsLevel1(db, profile)
                    .Concat(this.SelectNeighborsLevel2(db, profile))
                    .Select(x => x.Key)
                    .Distinct()
                    .Count();
            });

            return result;
        }

        public virtual int Neighbors2Data(int id)
        {
            int result = 0;

            this.transactor.Transact(db =>
            {
                Profile profile = db.Sql<Profile>("SELECT p FROM Starcounter.MultiModelBenchmark.Profile p WHERE p.Key = ?", id).First();
                result = this.SelectNeighborsLevel1(db, profile)
                    .Concat(this.SelectNeighborsLevel2(db, profile))
                    .Select(x => x)
                    .Distinct()
                    .Count();
            });

            return result;
        }

        public virtual int Neighbors3(int id)
        {
            int result = 0;

            this.transactor.Transact(db =>
            {

                Profile profile = db.Sql<Profile>("SELECT p FROM Starcounter.MultiModelBenchmark.Profile p WHERE p.Key = ?", id).First();
                result = this.SelectNeighborsLevel1(db, profile)
                    .Concat(this.SelectNeighborsLevel2(db, profile))
                    .Concat(this.SelectNeighborsLevel3(db, profile))
                    .Select(x => x.Key)
                    .Distinct()
                    .Count();
            });

            return result;
        }

        public virtual int ShortestPath(int fromProfileId, int toProfileId)
        {
            int maxLevel = 20;

            return this.transactor.Transact<int>(db =>
            {
                int level = 0;
                Profile fromProfile = db.Sql<Profile>("SELECT p FROM Starcounter.MultiModelBenchmark.Profile p WHERE p.Key = ?", fromProfileId).First();
                Profile toProfile = db.Sql<Profile>("SELECT p FROM Starcounter.MultiModelBenchmark.Profile p WHERE p.Key = ?", toProfileId).First();
                ulong toProfileNo = db.GetOid(toProfile);

                if (fromProfile.FollowNos == null)
                {
                    return 0;
                }

                IEnumerable<Profile> current = new List<Profile>() { fromProfile };
                HashSet<ulong> visited = new HashSet<ulong>()
                {
                    db.GetOid(fromProfile)
                };

                while (level < maxLevel)
                {
                    level++;
                    List<ulong> added = new List<ulong>();

                    foreach (Profile profile in current)
                    {
                        byte[] bytes = profile.FollowNos.ToArray();
                        ulong[] nos = bytes.ToUlongArray();

                        foreach (ulong toNo in nos)
                        {
                            if (toNo == toProfileNo)
                            {
                                return level;
                            }

                            if (visited.Contains(toNo))
                            {
                                continue;
                            }

                            visited.Add(toNo);
                            added.Add(toNo);
                        }
                    }

                    if (!added.Any())
                    {
                        return 0;
                    }

                    current = added.Select(x => db.Get<Profile>(x));
                }

                return 0;
            });
        }
        public virtual int[] ShortestPathWithSteps(int fromProfileId, int toProfileId)
        {
            int[] result = this.transactor.Transact(db =>
            {
                Profile fromProfile = db.Sql<Profile>("SELECT p FROM Starcounter.MultiModelBenchmark.Profile p WHERE p.Key = ?", fromProfileId).First();
                Profile toProfile = db.Sql<Profile>("SELECT p FROM Starcounter.MultiModelBenchmark.Profile p WHERE p.Key = ?", toProfileId).First();

                return this.ShortestPathWithSteps(db, fromProfile, toProfile);
            });

            return result;
        }

        public virtual int[] ShortestPathWithSteps(IDatabaseContext db, Profile fromProfile, Profile toProfile)
        {
            int level = 0;
            int maxLevel = 20;
            ulong toProfileNo = db.GetOid(toProfile);

            Dictionary<ulong, List<ulong>> visited = new Dictionary<ulong, List<ulong>>()
            {
                {
                    db.GetOid(fromProfile),
                    new List<ulong>() { db.GetOid(fromProfile) }
                }
            };

            IEnumerable<KeyValuePair<Profile, List<ulong>>> current = new List<KeyValuePair<Profile, List<ulong>>>()
            {
                new KeyValuePair<Profile, List<ulong>>(fromProfile, visited[db.GetOid(fromProfile)])
            };

            while (level < maxLevel)
            {
                level++;
                List<ulong> added = new List<ulong>();

                foreach (KeyValuePair<Profile, List<ulong>> pair in current)
                {
                    byte[] bytes = pair.Key.FollowNos.ToArray();
                    ulong[] nos = bytes.ToUlongArray();

                    foreach (ulong toNo in nos)
                    {
                        if (toNo == toProfileNo)
                        {
                            pair.Value.Add(toNo);

                            int[] steps = pair.Value
                                .Select(no => db.Get<Profile>(no).Key)
                                .ToArray();

                            return steps;
                        }

                        if (visited.ContainsKey(toNo))
                        {
                            continue;
                        }

                        List<ulong> path = pair.Value.ToList();
                        path.Add(toNo);

                        visited.Add(toNo, path);
                        added.Add(toNo);
                    }
                }

                if (!added.Any())
                {
                    return new int[0];
                }

                current = added.Select(x => new KeyValuePair<Profile, List<ulong>>(db.Get<Profile>(x), visited[x]));
            }

            return new int[0];
        }

        protected virtual IEnumerable<Profile> SelectNeighborsLevel1(IDatabaseContext db, Profile profile)
        {
            return profile.FollowNos
                .ToArray()
                .ToUlongArray()
                .Select(x => db.Get<Profile>(x));
        }

        protected virtual IEnumerable<Profile> SelectNeighborsLevel2(IDatabaseContext db, Profile profile)
        {
            return this.SelectNeighborsLevel1(db, profile)
                .SelectMany(x => x.FollowNos.ToArray().ToUlongArray())
                .Select(x => db.Get<Profile>(x));
        }

        protected virtual IEnumerable<Profile> SelectNeighborsLevel3(IDatabaseContext db, Profile profile)
        {
            return this.SelectNeighborsLevel2(db, profile)
                .SelectMany(x => x.FollowNos.ToArray().ToUlongArray())
                .Select(x => db.Get<Profile>(x));
        }


        #region Starcounter.MultiModelBenchmark.Benchmark.IDescription
        string IDescription.GetVersion()
        {
            return this.GetVersion();
        }

        void IDescription.DropCollection()
        {
            this.DropCollection();
        }

        string IDescription.LoadRelations()
        {
            return this.LoadRelations();
        }

        string IDescription.GetDocument(int key)
        {
            return this.GetDocument(key);
        }

        async Task<string> IDescription.SaveDocument(string json)
        {
            return await this.SaveDocument(json);
        }

        string IDescription.SaveDocumentSync(string json)
        {
            return this.SaveDocumentSync(json);
        }

        string IDescription.Aggregate()
        {
            return this.Aggregate();
        }

        int IDescription.Neighbors(int id)
        {
            return this.Neighbors(id);
        }

        int IDescription.Neighbors2(int id)
        {
            return this.Neighbors2(id);
        }

        int IDescription.Neighbors2Data(int id)
        {
            return this.Neighbors2Data(id);
        }

        int IDescription.Neighbors3(int id)
        {
            return this.Neighbors3(id);
        }

        int IDescription.ShortestPath(int fromProfileId, int toProfileId)
        {
            return this.ShortestPath(fromProfileId, toProfileId);
        }

        int[] IDescription.ShortestPathWithSteps(int fromProfileId, int toProfileId)
        {
            return this.ShortestPathWithSteps(fromProfileId, toProfileId);
        }
        #endregion
    }
}
