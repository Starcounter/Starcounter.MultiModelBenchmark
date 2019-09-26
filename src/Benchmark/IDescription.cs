using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Starcounter.MultiModelBenchmark.Benchmark
{
    public interface IDescription
    {
        string GetVersion();
        void DropCollection();
        string LoadRelations();
        string GetDocument(int key);
        Task<string> SaveDocument(string json);
        string SaveDocumentSync(string json);
        string Aggregate();
        int Neighbors(int id);
        int Neighbors2(int id);
        int Neighbors2Data(int id);
        int Neighbors3(int id);
        int ShortestPath(int fromProfileId, int toProfileId);
        int[] ShortestPathWithSteps(int fromProfileId, int toProfileId);
    }
}
