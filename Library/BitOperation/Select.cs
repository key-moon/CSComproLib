static partial class BitOperation
{
    public static byte Select(ulong n, ulong rank)
    {
        ulong a = (n & 0x55555555) + ((n >> 1) & 0x55555555);
        ulong b = (a & 0x33333333) + ((a >> 2) & 0x33333333);
        ulong c = (b & 0x0f0f0f0f) + ((b >> 4) & 0x0f0f0f0f);
        ulong d = (c & 0x00ff00ff) + ((c >> 8) & 0x00ff00ff);
        ulong t = (d & 0xff) + ((d >> 8) & 0xff);
        byte s = 0;
        if (rank >= t) {s += 32; rank -= t;}
        t = (d >> s) & 0xff;
        if (rank >= t) {s += 16; rank -= t;}
        t = (c >> s) & 0xf;
        if (rank >= t) {s += 8; rank -= t;}
        t = (b >> s) & 0x7;
        if (rank >= t) {s += 4; rank -= t;}
        t = (a >> s) & 0x3;
        if (rank >= t) {s += 2; rank -= t;}
        t = (n >> s) & 0x1;
        if (rank >= t) s++;
        return s;
    }
    public static byte Select(uint n, ulong rank)
    {
        ulong a = (n & 0x55555555) + ((n >> 1) & 0x55555555);
        ulong b = (a & 0x33333333) + ((a >> 2) & 0x33333333);
        ulong c = (b & 0x0f0f0f0f) + ((b >> 4) & 0x0f0f0f0f);
        ulong t = (c & 0xff) + ((c >> 8) & 0xff);
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