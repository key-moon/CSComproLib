using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static partial class Verify
{
    public static void SucVB_Measure(int iterate, int vbSize, int vbIterate)
    {
        DateTime start = new DateTime(), end = new DateTime();
        TimeSpan[] spans = new TimeSpan[3];
        for (int i = 0; i < iterate; i++)
        {
            //int n = 131070;
            bool[] s = Enumerable.Repeat(0, vbSize).Select(_ => RNG.NextBool()).ToArray();
            int count0 = 0;
            int count1 = 0;
            for (int k = 0; k < vbIterate; k++)
            {
                if (s[k]) count1++;
                else count0++;
            }
            var sucVB = new SuccinctBitVector(s);

            Console.WriteLine($"start : {i}");

            start = DateTime.Now;
            for (int k = 0; k < vbIterate; k++) sucVB.Access(k);
            end = DateTime.Now;
            spans[0] += end - start;

            start = DateTime.Now;
            for (int k = 0; k < vbIterate; k++) sucVB.Rank(k);
            end = DateTime.Now;
            spans[1] += end - start;

            start = DateTime.Now;
            for (int k = 0; k < count0; k++) sucVB.Select0(k);
            for (int k = 0; k < count1; k++) sucVB.Select1(k);
            end = DateTime.Now;
            spans[2] += end - start;
        }
        Console.WriteLine($"要素{vbSize}個\n{vbIterate}回合計({iterate}回平均)");
        Console.WriteLine($"access : {(spans[0].Ticks / (iterate))}μs");
        Console.WriteLine($"  rank : {(spans[1].Ticks / (iterate))}μs");
        Console.WriteLine($"select : {(spans[2].Ticks / (iterate))}μs");
    }
    public static void SucVB_Verify(int iterate, int vbMaxSize)
    {
        for (int i = 0; i < iterate; i++)
        {
            bool[] s = Enumerable.Repeat(0, vbMaxSize).Select(_ => RNG.NextBool()).ToArray();
            SuccinctBitVector sucVB = new SuccinctBitVector(s);
            Console.WriteLine($"start : {i}");
            int rank = 0;
            int count0 = 0;
            int count1 = 0;
            for (int k = 0; k < vbMaxSize; k++)
            {
                var kind = s[k];
                if (kind) rank++;

                var accessres = sucVB.Access(k);
                var rankres = sucVB.Rank(k);
                var selectres = sucVB.Select(kind ? count1 : count0, kind);
                if (kind != accessres) throw new Exception();
                if (rank != rankres) throw new Exception();
                if (k != selectres) throw new Exception();

                if (kind) count1++;
                else count0++;
            }
        }
    }
}
