using System.Runtime.CompilerServices;

static partial class BitOperation
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte PopCount(ulong n)
    {
        n = (n & 0x5555555555555555) + ((n >>  1) & 0x5555555555555555);
        n = (n & 0x3333333333333333) + ((n >>  2) & 0x3333333333333333);
        n = (n & 0x0f0f0f0f0f0f0f0f) + ((n >>  4) & 0x0f0f0f0f0f0f0f0f);
        n = (n & 0x00ff00ff00ff00ff) + ((n >>  8) & 0x00ff00ff00ff00ff);
        n = (n & 0x0000ffff0000ffff) + ((n >> 16) & 0x0000ffff0000ffff);
        return (byte)((n & 0x00000000ffffffff) + ((n >> 32) & 0x00000000ffffffff));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte PopCount(long n)
    {
        if (n < 0) return (byte)(PopCount((ulong)(-n)) + 1);
        return PopCount((ulong)n);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte PopCount (uint n)
    {
        n = (n & 0x55555555) + ((n >> 1) & 0x55555555);
        n = (n & 0x33333333) + ((n >> 2) & 0x33333333);
        n = (n & 0x0f0f0f0f) + ((n >> 4) & 0x0f0f0f0f);
        n = (n & 0x00ff00ff) + ((n >> 8) & 0x00ff00ff);
        return (byte)((n & 0x0000ffff) + ((n >> 16) & 0x0000ffff));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte PopCount (int n)
    {
        if (n < 0) return (byte)(PopCount((uint)(-n)) + 1);
        return PopCount((uint)n);
    }

    //another method
    private static int _PopCount_1(uint n)
    {
        return PopCountTable256[n & 255] + PopCountTable256[(n >> 8) & 255] + PopCountTable256[(n >> 16) & 255] + PopCountTable256[(n >> 24) & 255];
    }
    public static readonly int[] PopCountTable256 =
    {
        0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4,
        1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
        1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
        2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
        1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
        2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
        2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
        3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
        1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
        2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
        2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
        3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
        2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
        3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
        3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
        4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 7, 7, 8
    };
}