using System;
static partial class RNG
{
    private static ulong _xorshift_x = (ulong)DateTime.Now.Ticks * 0x3141592c0ffee;
    public static bool NextBool() => (NextULong() & 1) == 1;
    public static sbyte NextSByte()
    {
        unchecked
        {
            return (sbyte)NextULong();
        }
    }
    public static sbyte NextSByte(sbyte MinValue, sbyte MaxValue) => (sbyte)(NextSByte() % (MaxValue - MinValue) + MinValue);
    public static byte NextByte()
    {
        unchecked
        {
            return (byte)NextULong();
        }
    }
    public static byte NextByte(byte MinValue, byte MaxValue) => (byte)(NextByte() % (MaxValue - MinValue) + MinValue);
    public static short NextShort()
    {
        unchecked
        {
            return (short)NextULong();
        }
    }
    public static short NextShort(short MinValue, short MaxValue) => (short)(NextShort() % (MaxValue - MinValue) + MinValue);
    public static ushort NextUShort()
    {
        unchecked
        {
            return (ushort)NextULong();
        }
    }
    public static ushort NextUShort(ushort MinValue, ushort MaxValue) => (ushort)(NextUShort() % (MaxValue - MinValue) + MinValue);
    public static int NextInt()
    {
        unchecked
        {
            return (int)NextULong();
        }
    }
    public static int NextInt(int MinValue, int MaxValue) => NextInt() % (MaxValue - MinValue) + MinValue;
    public static uint NextUInt()
    {
        unchecked
        {
            return (uint)NextULong();
        }
    }
    public static uint NextUInt(uint MinValue, uint MaxValue) => NextUInt() % (MaxValue - MinValue) + MinValue;
    public static long NextLong()
    {
        unchecked
        {
            return (long)NextULong();
        }
    }
    public static long NextLong(long MinValue, long MaxValue) => NextLong() % (MaxValue - MinValue) + MinValue;
    public static ulong NextULong()
    {
        _xorshift_x = _xorshift_x ^ (_xorshift_x << 7);
        _xorshift_x = _xorshift_x ^ (_xorshift_x >> 9);
        return _xorshift_x;
    }
    public static ulong NextULong(ulong MinValue, ulong MaxValue) => NextULong() % (MaxValue - MinValue) + MinValue;
}