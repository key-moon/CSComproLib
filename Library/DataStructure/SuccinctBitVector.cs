using System;
using System.Runtime.CompilerServices;

// 簡潔ビットベクトル
// 空間計算量 : n + n/4 + n/8
// 時間計算量 :   Rank O(1)
// 　　　　　   Select O(log(n/256))
class SuccinctBitVector
{
    const int BITBLOCK_LENGTH = 64;
    const int LARGEBLOCK_LENGTH = 256;
    const int BLOCK_PER_LARGEBLOCK = LARGEBLOCK_LENGTH / BITBLOCK_LENGTH;
    
    public int Length;
    ulong[] bits;
    byte[] count;
    int[] largeCount;
    int count0;
    int count1;

    public SuccinctBitVector(ulong[] bits) : this(bits, bits.Length * BITBLOCK_LENGTH) { }
    public SuccinctBitVector(bool[] bits) : this(BoolsToUInts(bits), bits.Length) { }
    private SuccinctBitVector(ulong[] bits, int length)
    {
        Length = length;
        this.bits = bits;
        count = new byte[bits.Length];
        largeCount = new int[(bits.Length + BLOCK_PER_LARGEBLOCK - 1) / BLOCK_PER_LARGEBLOCK];
        byte sum = 0;
        for (int i = 0; i < bits.Length - 1; i++)
        {
            var popcnt = BitOperation.PopCount(bits[i]);
            if ((i + 1) % BLOCK_PER_LARGEBLOCK == 0)
            {
                int ind = (i + 1) / BLOCK_PER_LARGEBLOCK;
                largeCount[ind] = largeCount[ind - 1] + sum + popcnt;
                sum = 0;
            }
            else
            {
                sum += popcnt;
                count[i + 1] = sum;
            }
            count0 += BITBLOCK_LENGTH - popcnt;
            count1 += popcnt;
        }
        var lastpopcnt = BitOperation.PopCount(bits[bits.Length - 1]);
        count0 += BITBLOCK_LENGTH - lastpopcnt;
        count1 += lastpopcnt;
    }

    public bool this[int index]
    {
        [MethodImpl(MethodImplOptions.Unmanaged & MethodImplOptions.AggressiveInlining)]
        get
        {
            return Access(index);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Access(int index) => (bits[index / BITBLOCK_LENGTH] & (1UL << (index % BITBLOCK_LENGTH))) != 0;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Rank(int index)
    {
        index++;
        int bitblockind = index / BITBLOCK_LENGTH;
        int res = largeCount[bitblockind / BLOCK_PER_LARGEBLOCK] + count[bitblockind] + BitOperation.PopCount(bits[bitblockind] & ((1UL << (index % BITBLOCK_LENGTH)) - 1));
        return res;
    }

    public int Select(int index, bool kind)
    {
        if (kind) return Select1(index);
        return Select0(index);
    }
    public int Select0(int index)
    {
        if (index >= count0) return Length;

        int res = 0;
        int ok = 0;
        int ng = largeCount.Length;
        while (ng - ok > 1)
        {
            int mid = (ng + ok) / 2;
            if ((mid * LARGEBLOCK_LENGTH - largeCount[mid]) <= index) ok = mid;
            else ng = mid;
        }
        res += ok * LARGEBLOCK_LENGTH;
        
        int bitind = 0;
        int remain = index - (ok * LARGEBLOCK_LENGTH - largeCount[ok]);
        int offset = ok * BLOCK_PER_LARGEBLOCK;
        int max = Math.Min(BLOCK_PER_LARGEBLOCK, count.Length - offset);
        for (int i = max - 1; i >= 0; i--)
        {
            if ((i * BITBLOCK_LENGTH - count[offset + i]) <= remain)
            {
                bitind = i;
                break;
            }
        }
        res += bitind * BITBLOCK_LENGTH;
        
        var bit = bits[offset + bitind];
        remain = remain - (bitind * BITBLOCK_LENGTH - count[offset + bitind]);
        res += BitOperation.Select(~bit, (ulong)remain);
        return res;
    }
    public int Select1(int index)
    {
        if (index >= count1) return Length;

        int res = 0;
        int ok = 0;
        int ng = largeCount.Length;
        while (ng - ok > 1)
        {
            int mid = (ng + ok) / 2;
            if (largeCount[mid] <= index) ok = mid;
            else ng = mid;
        }
        res += ok * LARGEBLOCK_LENGTH;
        
        int bitind = 0;
        int remain = index - largeCount[ok];
        int offset = ok * BLOCK_PER_LARGEBLOCK;
        int max = Math.Min(BLOCK_PER_LARGEBLOCK, count.Length - offset);
        for (int i = max - 1; i >= 0; i--)
        {
            if (count[offset + i] <= remain)
            {
                bitind = i;
                break;
            }
        }
        res += bitind * BITBLOCK_LENGTH;
        
        var bit = bits[offset + bitind];
        remain = remain - count[offset + bitind];
        res += BitOperation.Select(bit, (ulong)remain);
        return res;
    }

    public static ulong[] BoolsToUInts(bool[] bits)
    {
        var ulongbits = new ulong[(bits.Length + BITBLOCK_LENGTH) / BITBLOCK_LENGTH];
        for (int i = 0; i < ulongbits.Length; i++)
        {
            int offset = i * BITBLOCK_LENGTH;
            int max = Math.Min(BITBLOCK_LENGTH, bits.Length - offset);
            for (int j = 0; j < max; j++)
            {
                if (bits[offset + j]) ulongbits[i] |= (1UL << j);
            }
        }
        return ulongbits;
    }
}

class SuccinctBitVector32
{
    const int BITBLOCK_LENGTH = 32;
    const int LARGEBLOCK_LENGTH = 256;
    const int BLOCK_PER_LARGEBLOCK = LARGEBLOCK_LENGTH / BITBLOCK_LENGTH;

    public int Length;
    uint[] bits;
    byte[] count;
    int[] largeCount;
    int count0;
    int count1;

    public SuccinctBitVector32(uint[] bits) : this(bits, bits.Length * BITBLOCK_LENGTH) { }
    public SuccinctBitVector32(bool[] bits) : this(BoolsToUInts(bits), bits.Length) { }
    private SuccinctBitVector32(uint[] bits, int length)
    {
        Length = length;
        this.bits = bits;
        count = new byte[bits.Length];
        largeCount = new int[(bits.Length + BLOCK_PER_LARGEBLOCK - 1) / BLOCK_PER_LARGEBLOCK];
        byte sum = 0;
        for (int i = 0; i < bits.Length - 1; i++)
        {
            var popcnt = BitOperation.PopCount(bits[i]);
            if ((i + 1) % BLOCK_PER_LARGEBLOCK == 0)
            {
                int ind = (i + 1) / BLOCK_PER_LARGEBLOCK;
                largeCount[ind] = largeCount[ind - 1] + sum + popcnt;
                sum = 0;
            }
            else
            {
                sum += popcnt;
                count[i + 1] = sum;
            }
            count0 += BITBLOCK_LENGTH - popcnt;
            count1 += popcnt;
        }
        var lastpopcnt = BitOperation.PopCount(bits[bits.Length - 1]);
        count0 += BITBLOCK_LENGTH - lastpopcnt;
        count1 += lastpopcnt;
    }

    public bool this[int index]
    {
        [MethodImpl(MethodImplOptions.Unmanaged & MethodImplOptions.AggressiveInlining)]
        get
        {
            return Access(index);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Access(int index) => (bits[index / BITBLOCK_LENGTH] & (1U << (index % BITBLOCK_LENGTH))) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Rank(int index)
    {
        index++;
        int bitblockind = index / BITBLOCK_LENGTH;
        int res = largeCount[bitblockind / BLOCK_PER_LARGEBLOCK] + count[bitblockind] + BitOperation.PopCount(bits[bitblockind] & ((1U << (index % BITBLOCK_LENGTH)) - 1));
        return res;
    }

    public int Select(int index, bool kind)
    {
        if (kind) return Select1(index);
        return Select0(index);
    }
    public int Select0(int index)
    {
        if (index >= count0) return Length;

        int res = 0;
        int ok = 0;
        int ng = largeCount.Length;
        while (ng - ok > 1)
        {
            int mid = (ng + ok) / 2;
            if ((mid * LARGEBLOCK_LENGTH - largeCount[mid]) <= index) ok = mid;
            else ng = mid;
        }
        res += ok * LARGEBLOCK_LENGTH;

        int bitind = 0;
        int remain = index - (ok * LARGEBLOCK_LENGTH - largeCount[ok]);
        int offset = ok * BLOCK_PER_LARGEBLOCK;
        int max = Math.Min(BLOCK_PER_LARGEBLOCK, count.Length - offset);
        for (int i = max - 1; i >= 0; i--)
        {
            if ((i * BITBLOCK_LENGTH - count[offset + i]) <= remain)
            {
                bitind = i;
                break;
            }
        }
        res += bitind * BITBLOCK_LENGTH;

        var bit = bits[offset + bitind];
        remain = remain - (bitind * BITBLOCK_LENGTH - count[offset + bitind]);
        res += BitOperation.Select(~bit, (ulong)remain);
        return res;
    }
    public int Select1(int index)
    {
        if (index >= count1) return Length;

        int res = 0;
        int ok = 0;
        int ng = largeCount.Length;
        while (ng - ok > 1)
        {
            int mid = (ng + ok) / 2;
            if (largeCount[mid] <= index) ok = mid;
            else ng = mid;
        }
        res += ok * LARGEBLOCK_LENGTH;

        int bitind = 0;
        int remain = index - largeCount[ok];
        int offset = ok * BLOCK_PER_LARGEBLOCK;
        int max = Math.Min(BLOCK_PER_LARGEBLOCK, count.Length - offset);
        for (int i = max - 1; i >= 0; i--)
        {
            if (count[offset + i] <= remain)
            {
                bitind = i;
                break;
            }
        }
        res += bitind * BITBLOCK_LENGTH;

        var bit = bits[offset + bitind];
        remain = remain - count[offset + bitind];
        res += BitOperation.Select(bit, (ulong)remain);
        return res;
    }

    public static uint[] BoolsToUInts(bool[] bits)
    {
        var ulongbits = new uint[(bits.Length + BITBLOCK_LENGTH) / BITBLOCK_LENGTH];
        for (int i = 0; i < ulongbits.Length; i++)
        {
            int offset = i * BITBLOCK_LENGTH;
            int max = Math.Min(BITBLOCK_LENGTH, bits.Length - offset);
            for (int j = 0; j < max; j++)
            {
                if (bits[offset + j]) ulongbits[i] |= (1U << j);
            }
        }
        return ulongbits;
    }
}