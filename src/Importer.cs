using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using Starcounter.Nova;
using Starcounter.Nova.Hosting;

namespace Starcounter.MultiModelBenchmark
{
    public class Importer
    {
        public virtual char SplitChar { get; protected set; } = '\t';
        public virtual int ProfilesPerTransaction { get; protected set; } = 1000;
        public virtual int RelationsPerTransaction { get; protected set; } = 100000;
        public virtual int UpdateProfilesPerTransaction { get; protected set; } = 100000;
        public virtual int MaxParallelStarcounterTransactions { get; protected set; } = 32;

        private readonly ITransactor transactor;

        public Importer(ITransactor transactor)
        {
            this.transactor = transactor;
        }

        public virtual int ImportProfiles(string filePath)
        {
            return this.ImportFile(filePath, this.ProfilesPerTransaction, this.ImportProfiles);
        }

        public virtual int ImportRelations(string filePath)
        {
            int result = this.ImportFile(filePath, this.RelationsPerTransaction, this.ImportRelations);

            this.UpdateProfileFollows();

            return result;
        }

        public virtual void UpdateProfileFollows()
        {
            List<ulong> buffer = new List<ulong>();
            List<Task> tasks = new List<Task>();

            this.transactor.Transact(db =>
            {
                foreach (Profile profile in db.Sql<Profile>("SELECT p FROM Starcounter.MultiModelBenchmark.Profile p"))
                {
                    if (tasks.Count >= this.MaxParallelStarcounterTransactions)
                    {
                        tasks.First().Wait();
                        tasks.RemoveAt(0);
                    }

                    buffer.Add(db.GetOid(profile));

                    if (buffer.Count >= this.UpdateProfilesPerTransaction)
                    {
                        List<ulong> taskBuffer = buffer;

                        buffer = new List<ulong>();

                        tasks.Add(Task.Run(() =>
                        {
                            this.UpdateProfileFollows(taskBuffer);
                        }));
                    }
                }

                if (buffer.Any())
                {
                    tasks.Add(Task.Run(() =>
                    {
                        this.UpdateProfileFollows(buffer);
                    }));
                }
            });

            Task.WaitAll(tasks.ToArray());
        }

        protected virtual int ImportFile(string filePath, int maxLinesPerTransation, Action<List<string[]>> transactionAction)
        {
            FileInfo fi = new FileInfo(filePath);

            if (!fi.Exists)
            {
                throw new FileNotFoundException(fi.FullName);
            }

            int totalCount = 0;
            TextReader reader = new StreamReader(fi.FullName, true);
            List<string[]> buffer = new List<string[]>();
            List<Task> tasks = new List<Task>();

            try
            {
                string line = reader.ReadLine();

                while (line != null)
                {
                    if (tasks.Count >= this.MaxParallelStarcounterTransactions)
                    {
                        tasks.First().Wait();
                        tasks.RemoveAt(0);
                    }

                    string[] values = line
                        .Split(this.SplitChar)
                        .Select(x => x == "null" ? null : x)
                        .ToArray();

                    buffer.Add(values);

                    if (buffer.Count >= maxLinesPerTransation)
                    {
                        List<string[]> taskBuffer = buffer;

                        totalCount += buffer.Count;
                        buffer = new List<string[]>();

                        tasks.Add(Task.Run(() =>
                        {
                            transactionAction(taskBuffer);
                        }));
                    }

                    line = reader.ReadLine();
                }

                if (buffer.Any())
                {
                    totalCount += buffer.Count;
                    tasks.Add(Task.Run(() =>
                    {
                        transactionAction(buffer);
                    }));
                }
            }
            finally
            {
                reader.Dispose();
            }

            Task.WaitAll(tasks.ToArray());

            return totalCount;
        }

        protected virtual void ImportProfiles(List<string[]> buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            this.transactor.Transact(db =>
            {
                foreach (string[] values in buffer)
                {
                    Profile p = db.Insert<Profile>();
                    int i = 0;

                    p.Key = int.Parse(values[i++]);
                    p.Public = int.TryParse(values[i++], out int @public) ? (int?)@public : null;
                    p.CompletionPercentage = int.TryParse(values[i++], out int completionPercentage) ? (int?)completionPercentage : null;
                    p.Gender = int.TryParse(values[i++], out int gender) ? (int?)gender : null;
                    p.Region = values[i++];
                    p.LastLogin = DateTime.TryParse(values[i++], out DateTime lastLogin) ? (DateTime?)lastLogin : null;
                    p.Registration = DateTime.TryParse(values[i++], out DateTime registratoin) ? (DateTime?)registratoin : null;
                    p.Age = int.TryParse(values[i++], out int age) ? (int?)age : null;
                    p.Body = values[i++];
                    p.WorkFieldDescription = values[i++];
                    p.SpokenLanguages = values[i++];
                    p.Hobbies = values[i++];
                    p.EnjoyFoodAt = values[i++];
                    p.Pets = values[i++];
                    p.BodyType = values[i++];
                    p.EyeSight = values[i++];
                    p.EyeColor = values[i++];
                    p.HairColor = values[i++];
                    p.HairType = values[i++];
                    p.CompletedEducationLevel = values[i++];
                    p.FavoriteColor = values[i++];
                    p.SmokingAttitude = values[i++];
                    p.AlcoholAttitude = values[i++];
                    p.ZodiacSign = values[i++];
                    p.LookingFor = values[i++];
                    p.LoveAttitude = values[i++];
                    p.RandomSexAttitude = values[i++];
                    p.WantedPartnerDescription = values[i++];
                    p.MaritalStatus = values[i++];
                    p.Children = values[i++];
                    p.ChildrenAttitude = values[i++];
                    p.LikeMovies = values[i++];
                    p.EnjoyMoviesAt = values[i++];
                    p.LikeMusic = values[i++];
                    p.EnjoyMusicAt = values[i++];
                    p.GoodEveningDescription = values[i++];
                    p.LikeKitchenFrom = values[i++];
                    p.Fun = values[i++];
                    p.ConcertsAttitude = values[i++];
                    p.ActiveSports = values[i++];
                    p.PassiveSports = values[i++];
                    p.Profession = values[i++];
                    p.LikeBooks = values[i++];
                    p.LifeStyle = values[i++];
                    p.Music = values[i++];
                    p.Cars = values[i++];
                    p.Politics = values[i++];
                    p.Relationships = values[i++];
                    p.ArtCulture = values[i++];
                    p.Interests = values[i++];
                    p.ScienceTechnologies = values[i++];
                    p.ComputersInternet = values[i++];
                    p.Education = values[i++];
                    p.Sports = values[i++];
                    p.Movies = values[i++];
                    p.Travelling = values[i++];
                    p.Health = values[i++];
                    p.CompaniesBrands = values[i++];
                    p.More = values[i++];
                }
            });
        }

        protected virtual void ImportRelations(List<string[]> buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            this.transactor.Transact(db =>
            {
                foreach (string[] values in buffer)
                {
                    int fromKey = int.Parse(values[0]);
                    int toKey = int.Parse(values[1]);
                    Profile from = db.Sql<Profile>("SELECT p FROM Starcounter.MultiModelBenchmark.Profile p WHERE p.Key = ?", fromKey).First();
                    Profile to = db.Sql<Profile>("SELECT p FROM Starcounter.MultiModelBenchmark.Profile p WHERE p.Key = ?", toKey).First();
                    Relation r = db.Insert<Relation>();
                    r.From = from;
                    r.To = to;
                }
            });
        }

        protected virtual void UpdateProfileFollows(List<ulong> buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            this.transactor.Transact(db =>
            {
                foreach (ulong id in buffer)
                {
                    Profile profile = db.Get<Profile>(id);
                    List<Relation> relations = db.Sql<Relation>("SELECT r FROM Starcounter.MultiModelBenchmark.Relation r WHERE r.\"From\" = ?", profile).ToList();

                    {
                        ulong[] follows = relations.Select(x => db.GetOid(x.To)).ToArray();

                        byte[] result = new byte[follows.Length * sizeof(ulong)];
                        Buffer.BlockCopy(follows, 0, result, 0, result.Length);

                        profile.FollowNos = result;
                    }
                }
            });
        }
    }
}
