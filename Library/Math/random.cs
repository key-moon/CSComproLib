using System;
static partial class MyMath
{
    private static ulong _xorshift_x = 123456789, _xorshift_y = 362436069, _xorshift_z = (ulong)DateTime.Now.Ticks * 0xdeadface, _xorshift_w = (ulong)DateTime.Now.Ticks * 0x3141592c0ffee;
    static ulong Xor128()
    {
        ulong t = (_xorshift_x ^ (_xorshift_x << 11));
        _xorshift_x = _xorshift_y; _xorshift_y = _xorshift_z; _xorshift_z = _xorshift_w;
        return (_xorshift_w = (_xorshift_w ^ (_xorshift_w >> 19)) ^ (t ^ (t >> 8)));
    }
}