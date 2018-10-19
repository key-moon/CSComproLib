using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static partial class Math
{
    public static int PopCount (uint n)
    {
        n = (n & 0x55555555) + ((n >> 1) & 0x55555555);
        n = (n & 0x33333333) + ((n >> 2) & 0x33333333);
        n = (n & 0x0f0f0f0f) + ((n >> 4) & 0x0f0f0f0f);
        n = (n & 0x00ff00ff) + ((n >> 8) & 0x00ff00ff);
        return (int)((n & 0x0000ffff) + ((n >> 16) & 0x0000ffff));
    }
    public static int PopCount (int n)
    {
        if (n < 0) return PopCount((uint)(-n)) + 1;
        return PopCount((uint)n);
    }

    //another method
    private static int _PopCount_1(uint n)
    {
        return PopCountTable256[n & 255] + PopCountTable256[(n >> 8) & 255] + PopCountTable256[(n >> 16) & 255] + PopCountTable256[(n >> 24) & 255];
    }
}