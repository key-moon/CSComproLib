using System.Runtime.CompilerServices;

static partial class MyMath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(uint n)
    {
        n |= (n >> 1);
        n |= (n >> 2);
        n |= (n >> 4);
        n |= (n >> 8);
        n |= (n >> 16);
        return MultiplyDeBruijnBitPosition[(n * 0x07c4acddu) >> 27];
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(int n)
    {
        if (n < 0) return Log2((uint)(-n));
        return Log2((uint)n);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log10(uint n)
    {
        int tmp = (Log2(n) * 1233) >> 12;
        if (n < Pow10Int[tmp + 1]) return tmp - 1;
        return tmp;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log10(int n)
    {
        if (n < 0) return Log10((uint)(-n));
        return Log10((uint)n);
    }

    private static int _Log2_1(uint n)
    {
        int res = 0;
        if ((n >> 16) != 0)
        {
            res += 16;
            n >>= 16;
        }
        if ((n >> 8) != 0)
        {
            res += 8;
            n >>= 8;
        }
        res += LogTable256[n];
        return res;
    }
    private static int _Log2_2(uint n)
    {
        for (int i = 31; i >= 0; i--)
        {
            if ((n >> i) != 0) return i + 1;
        }
        return 0;
    }
}