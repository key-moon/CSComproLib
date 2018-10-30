using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

public static partial class Verify
{
    public static void SucVB64_Measure(int iterate, int vbSize, int vbIterate, bool doAccess = true, bool doRank = true, bool doSelect = true)
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
            var sucVB = new SuccinctBitVector64(s);

            Debug.WriteLine($"start : {i}");

            if (doAccess)
            {
                start = DateTime.Now;
                for (int k = 0; k < vbIterate; k++) sucVB.Access(k);
                end = DateTime.Now;
                spans[0] += end - start;
            }
            if (doRank)
            {
                start = DateTime.Now;
                for (int k = 0; k < vbIterate; k++) sucVB.Rank(k);
                end = DateTime.Now;
                spans[1] += end - start;
            }
            if (doSelect)
            {
                start = DateTime.Now;
                for (int k = 0; k < count0; k++) sucVB.Select0(k);
                for (int k = 0; k < count1; k++) sucVB.Select1(k);
                end = DateTime.Now;
                spans[2] += end - start;
            }
        }
        Console.WriteLine($"要素{vbSize}個\n{vbIterate}回合計({iterate}回平均)");
        Console.WriteLine($"access : {(spans[0].TotalMilliseconds / (iterate)):0.000}ms");
        Console.WriteLine($"  rank : {(spans[1].TotalMilliseconds / (iterate)):0.000}ms");
        Console.WriteLine($"select : {(spans[2].TotalMilliseconds / (iterate)):0.000}ms");
    }
    public static void SucVB64_Verify(int iterate, int vbMaxSize, bool doAccess = true, bool doRank = true, bool doSelect = true)
    {
        for (int i = 0; i < iterate; i++)
        {
            bool[] s = Enumerable.Repeat(0, vbMaxSize).Select(_ => RNG.NextBool()).ToArray();
            SuccinctBitVector64 sucVB = new SuccinctBitVector64(s);
            Debug.WriteLine($"start : {i}");
            int rank = 0;
            int count0 = 0;
            int count1 = 0;
            for (int k = 0; k < vbMaxSize; k++)
            {
                var kind = s[k];
                if (kind) rank++;
                if (doAccess)
                {
                    var accessres = sucVB.Access(k);
                    if (kind != accessres) throw new Exception();
                }
                if (doRank)
                {
                    var rankres = sucVB.Rank(k);
                    if (rank != rankres) throw new Exception();
                }
                if (doSelect)
                {
                    var selectres = sucVB.Select(kind ? count1 : count0, kind);
                    if (k != selectres) throw new Exception();
                }
                if (kind) count1++;
                else count0++;
            }
        }
    }
    public static void SucVB_Measure(int iterate, int vbSize, int vbIterate, bool doAccess = true, bool doRank = true, bool doSelect = true)
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

            Debug.WriteLine($"start : {i}");

            if (doAccess)
            {
                start = DateTime.Now;
                for (int k = 0; k < vbIterate; k++) sucVB.Access(k);
                end = DateTime.Now;
                spans[0] += end - start;
            }
            if (doRank)
            {
                start = DateTime.Now;
                for (int k = 0; k < vbIterate; k++) sucVB.Rank(k);
                end = DateTime.Now;
                spans[1] += end - start;
            }
            if (doSelect)
            {
                start = DateTime.Now;
                for (int k = 0; k < count0; k++) sucVB.Select0(k);
                for (int k = 0; k < count1; k++) sucVB.Select1(k);
                end = DateTime.Now;
                spans[2] += end - start;
            }
        }
        Console.WriteLine($"要素{vbSize}個\n{vbIterate}回合計({iterate}回平均)");
        if (doAccess) Console.WriteLine($"access : {(spans[0].TotalMilliseconds / (iterate)):0.000}ms");
        if (doRank) Console.WriteLine($"  rank : {(spans[1].TotalMilliseconds / (iterate)):0.000}ms");
        if (doSelect) Console.WriteLine($"select : {(spans[2].TotalMilliseconds / (iterate)):0.000}ms");
    }
    public static void SucVB_Verify(int iterate, int vbMaxSize, bool doAccess = true, bool doRank = true, bool doSelect = true)
    {
        for (int i = 0; i < iterate; i++)
        {
            bool[] s = Enumerable.Repeat(0, vbMaxSize).Select(_ => RNG.NextBool()).ToArray();
            SuccinctBitVector sucVB = new SuccinctBitVector(s);
            Debug.WriteLine($"start : {i}");
            int rank = 0;
            int count0 = 0;
            int count1 = 0;
            for (int k = 0; k < vbMaxSize; k++)
            {
                var kind = s[k];
                if (kind) rank++;
                if (doAccess)
                {
                    var accessres = sucVB.Access(k);
                    if (kind != accessres) throw new Exception();
                }
                if (doRank)
                {
                    var rankres = sucVB.Rank(k);
                    if (rank != rankres) throw new Exception();
                }
                if (doSelect)
                {
                    var selectres = sucVB.Select(kind ? count1 : count0, kind);
                    if (k != selectres) throw new Exception();
                }
                if (kind) count1++;
                else count0++;
            }
        }
    }
}
