using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Starcounter.MultiModelBenchmark
{
    public static class MemoryUtils
    {
        public static ulong[] ToUlongArray(this byte[] bytes)
        {
            ReadOnlySpan<ulong> values = MemoryMarshal.Cast<byte, ulong>(new ReadOnlySpan<byte>(bytes));
            return values.ToArray();
        }

        public static int[] ToIntArray(this byte[] bytes)
        {
            ReadOnlySpan<int> values = MemoryMarshal.Cast<byte, int>(new ReadOnlySpan<byte>(bytes));
            return values.ToArray();
        }

        public static bool FindUlongTarget(byte[] bytes, HashSet<ulong> visited, List<ulong> added, ulong target)
        {
            ReadOnlySpan<ulong> values = MemoryMarshal.Cast<byte, ulong>(new ReadOnlySpan<byte>(bytes));

            foreach (ulong value in values)
            {
                if (value == target)
                {
                    return true;
                }

                if (visited.Contains(value))
                {
                    continue;
                }

                visited.Add(value);
                added.Add(value);
            }

            return false;
        }
    }
}
