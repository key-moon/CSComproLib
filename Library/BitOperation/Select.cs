
using System.Runtime.CompilerServices;

static partial class BitOperation
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Select(ulong n, ulong rank)
    {
        unchecked
        {
            ulong a = (n & 0x5555555555555555ul) + ((n >> 1) & 0x5555555555555555ul);
            ulong b = (a & 0x3333333333333333ul) + ((a >> 2) & 0x3333333333333333ul);
            ulong c = (b & 0x0f0f0f0f0f0f0f0ful) + ((b >> 4) & 0x0f0f0f0f0f0f0f0ful);
            ulong d = (c & 0x00ff00ff00ff00fful) + ((c >> 8) & 0x00ff00ff00ff00fful);

            ulong t = (d & 0xff) + ((d >> 16) & 0xff);
            byte s = 0;
            if (rank >= t) { s += 32; rank -= t; }
            t = (d >> s) & 0xff;
            if (rank >= t) { s += 16; rank -= t; }
            t = (c >> s) & 0xf;
            if (rank >= t) { s += 8; rank -= t; }
            t = (b >> s) & 0x7;
            if (rank >= t) { s += 4; rank -= t; }
            t = (a >> s) & 0x3;
            if (rank >= t) { s += 2; rank -= t; }
            t = (n >> s) & 0x1;
            if (rank >= t) s++;
            return s;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Select(uint n, ulong rank)
    {
        unchecked
        {
            uint a = (n & 0x55555555u) + ((n >> 1) & 0x55555555u);
            uint b = (a & 0x33333333u) + ((a >> 2) & 0x33333333u);
            uint c = (b & 0x0f0f0f0fu) + ((b >> 4) & 0x0f0f0f0fu);
            uint t = (c & 0xffu) + ((c >> 8) & 0xffu);
            byte s = 0;
            if (rank >= t) { s += 16; rank -= t; }
            t = (c >> s) & 0xf;
            if (rank >= t) { s += 8; rank -= t; }
            t = (b >> s) & 0x7;
            if (rank >= t) { s += 4; rank -= t; }
            t = (a >> s) & 0x3;
            if (rank >= t) { s += 2; rank -= t; }
            t = (n >> s) & 0x1;
            if (rank >= t) s++;
            return s;
        }
    }
}