using System;
using System.Runtime.CompilerServices;

// 簡潔ビットベクトル
// 空間計算量 : n + n/4 + n/8
// 時間計算量 : Access O(1)
//                Rank O(1)
// 　　　　　   Select O(logN) …log_2(N/256)+8+log_2(32)
// 　　実測値 : 要素1100000個
// 　　　　　   100000回合計(500回平均)
// 　　　　　   Access : 0.064ms
// 　　　　　     Rank : 0.649ms
// 　　　　　   Select : 6.974ms
class SuccinctBitVector
{
    const int BITBLOCK_LENGTH = 32;
    const int BITBLOCK_LENGTH_BITS = 5;

    const int LARGEBLOCK_LENGTH = 256;
    const int LARGEBLOCK_LENGTH_BITS = 8;

    const int BLOCK_PER_LARGEBLOCK = 8; //LARGEBLOCK_LENGTH / BITBLOCK_LENGTH
    const int BLOCK_PER_LARGEBLOCK_BITS = 3;

    public int Length;
    uint[] bits;
    byte[] count;
    int[] largeCount;
    int count0;
    int count1;

    public SuccinctBitVector(uint[] bits) : this(bits, bits.Length * BITBLOCK_LENGTH) { }
    public SuccinctBitVector(bool[] bits) : this(BoolsToUInts(bits), bits.Length) { }
    private SuccinctBitVector(uint[] bits, int length)
    {
        Length = length;
        this.bits = bits;
        count = new byte[bits.Length];
        largeCount = new int[(bits.Length + BLOCK_PER_LARGEBLOCK - 1) >> BLOCK_PER_LARGEBLOCK_BITS];
        byte sum = 0;
        for (int i = 0; i < bits.Length - 1; i++)
        {
            var popcnt = BitOperation.PopCount(bits[i]);
            if (((i + 1) & (BLOCK_PER_LARGEBLOCK - 1)) == 0)
            {
                int ind = (i + 1) >> BLOCK_PER_LARGEBLOCK_BITS;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return Access(index);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Access(int index)
    {
        unchecked
        {
            return (bits[index >> BITBLOCK_LENGTH_BITS] & (1U << index)) != 0;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Rank(int index)
    {
        unchecked
        {
            index++;
            int bitblockind = index >> BITBLOCK_LENGTH_BITS;
            int res = largeCount[bitblockind >> BLOCK_PER_LARGEBLOCK_BITS] + count[bitblockind] + BitOperation.PopCount(bits[bitblockind] & ((1U << index) - 1));
            return res;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Select(int index, bool kind)
    {
        if (kind) return Select1(index);
        return Select0(index);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Select0(int index)
    {
        unchecked
        {
            if (index >= count0) return Length;

            int res = 0;
            int ok = 0;
            int ng = largeCount.Length;
            while (ng - ok > 1)
            {
                int mid = (ng + ok) >> 1;
                if ((mid * LARGEBLOCK_LENGTH - largeCount[mid]) <= index) ok = mid;
                else ng = mid;
            }
            res += ok * LARGEBLOCK_LENGTH;

            int bitind;
            int remain = index - (ok * LARGEBLOCK_LENGTH - largeCount[ok]);
            int offset = ok * BLOCK_PER_LARGEBLOCK;
            for (bitind = Math.Min(BLOCK_PER_LARGEBLOCK, count.Length - offset) - 1; bitind >= 1; bitind--)
            {
                if ((bitind * BITBLOCK_LENGTH - count[offset + bitind]) <= remain) break;
            }
            res += bitind * BITBLOCK_LENGTH;

            var bit = bits[offset + bitind];
            remain = remain - (bitind * BITBLOCK_LENGTH - count[offset + bitind]);
            res += BitOperation.Select(~bit, (ulong)remain);
            return res;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Select1(int index)
    {
        unchecked
        {
            if (index >= count1) return Length;

            int res = 0;
            int ok = 0;
            int ng = largeCount.Length;
            while (ng - ok > 1)
            {
                int mid = (ng + ok) >> 1;
                if (largeCount[mid] <= index) ok = mid;
                else ng = mid;
            }
            res += ok * LARGEBLOCK_LENGTH;

            int bitind;
            int remain = index - largeCount[ok];
            int offset = ok * BLOCK_PER_LARGEBLOCK;
            int max = Math.Min(BLOCK_PER_LARGEBLOCK, count.Length - offset);
            for (bitind = max - 1; bitind >= 1; bitind--)
            {
                if (count[offset + bitind] <= remain) break;
            }
            res += bitind * BITBLOCK_LENGTH;

            var bit = bits[offset + bitind];
            remain = remain - count[offset + bitind];
            res += BitOperation.Select(bit, (ulong)remain);
            return res;
        }
    }

    public static uint[] BoolsToUInts(bool[] bits)
    {
        var ulongbits = new uint[(bits.Length + BITBLOCK_LENGTH) >> BITBLOCK_LENGTH_BITS];
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

// 簡潔ビットベクトル(64bit)
// 空間計算量 : n + n/8 + n/8
// 時間計算量 : Access O(1)
//                Rank O(1)
// 　　　　　   Select O(logN) …log_2(N/256)+4+log_2(64)
// 　　実測値 : 要素1048576個
//              100000回合計(500回平均)
//              要素1100000個
// 　　　　　   100000回合計(500回平均)
// 　　　　　   Access : 0.687ms
// 　　　　　     Rank : 1.426ms
// 　　　　　   Select : 7.671ms              
class SuccinctBitVector64
{
    const int BITBLOCK_LENGTH = 64;
    const int BITBLOCK_LENGTH_BITS = 6;

    const int LARGEBLOCK_LENGTH = 256;
    const int LARGEBLOCK_LENGTH_BITS = 8;

    const int BLOCK_PER_LARGEBLOCK = 4; //LARGEBLOCK_LENGTH / BITBLOCK_LENGTH
    const int BLOCK_PER_LARGEBLOCK_BITS = 2;
    
    public int Length;
    ulong[] bits;
    byte[] count;
    int[] largeCount;
    int count0;
    int count1;

    public SuccinctBitVector64(ulong[] bits) : this(bits, bits.Length * BITBLOCK_LENGTH) { }
    public SuccinctBitVector64(bool[] bits) : this(BoolsToUInts(bits), bits.Length) { }
    private SuccinctBitVector64(ulong[] bits, int length)
    {
        Length = length;
        this.bits = bits;
        count = new byte[bits.Length];
        largeCount = new int[(bits.Length + BLOCK_PER_LARGEBLOCK - 1) >> BLOCK_PER_LARGEBLOCK_BITS];
        byte sum = 0;
        for (int i = 0; i < bits.Length - 1; i++)
        {
            var popcnt = BitOperation.PopCount(bits[i]);
            if (((i + 1) & (BLOCK_PER_LARGEBLOCK - 1)) == 0)
            {
                int ind = (i + 1) >> BLOCK_PER_LARGEBLOCK_BITS;
                largeCount[ind] = (largeCount[ind - 1] + sum) + popcnt;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return Access(index);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Access(int index)
    {
        unchecked
        {
            return (bits[index >> BITBLOCK_LENGTH_BITS] & (1UL << index)) != 0;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Rank(int index)
    {
        unchecked
        {
            index++;
            int bitblockind = index >> BITBLOCK_LENGTH_BITS;
            int res = largeCount[bitblockind >> BLOCK_PER_LARGEBLOCK_BITS] + count[bitblockind] + BitOperation.PopCount(bits[bitblockind] & ((1UL << index) - 1));
            return res;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Select(int index, bool kind)
    {
        if (kind) return Select1(index);
        return Select0(index);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Select0(int index)
    {
        unchecked
        {
            if (index >= count0) return Length;

            int res = 0;
            int ok = 0;
            int ng = largeCount.Length;
            while (ng - ok > 1)
            {
                int mid = (ng + ok) >> 1;
                if ((mid * LARGEBLOCK_LENGTH - largeCount[mid]) <= index) ok = mid;
                else ng = mid;
            }
            res += ok * LARGEBLOCK_LENGTH;

            int bitind;
            int remain = index - (ok * LARGEBLOCK_LENGTH - largeCount[ok]);
            int offset = ok * BLOCK_PER_LARGEBLOCK;
            int max = Math.Min(BLOCK_PER_LARGEBLOCK, count.Length - offset);
            for (bitind = max - 1; bitind >= 1; bitind--)
            {
                if ((bitind * BITBLOCK_LENGTH - count[offset + bitind]) <= remain) break;
            }
            res += bitind * BITBLOCK_LENGTH;
        
            var bit = bits[offset + bitind];
            remain = remain - (bitind * BITBLOCK_LENGTH - count[offset + bitind]);
            res += BitOperation.Select(~bit, (ulong)remain);
            return res;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Select1(int index)
    {
        unchecked
        {
            if (index >= count1) return Length;

            int res = 0;
            int ok = 0;
            int ng = largeCount.Length;
            while (ng - ok > 1)
            {
                int mid = (ng + ok) >> 1;
                if (largeCount[mid] <= index) ok = mid;
                else ng = mid;
            }
            res += ok * LARGEBLOCK_LENGTH;

            int bitind;
            int remain = index - largeCount[ok];
            int offset = ok * BLOCK_PER_LARGEBLOCK;
            for (bitind = Math.Min(BLOCK_PER_LARGEBLOCK, count.Length - offset) - 1; bitind >= 1; bitind--)
            {
                if (count[offset + bitind] <= remain) break;
            }
            res += bitind * BITBLOCK_LENGTH;

            var bit = bits[offset + bitind];
            remain = remain - count[offset + bitind];
            res += BitOperation.Select(bit, (ulong)remain);
            return res;
        }
    }

    public static ulong[] BoolsToUInts(bool[] bits)
    {
        var ulongbits = new ulong[(bits.Length + BITBLOCK_LENGTH) >> BITBLOCK_LENGTH_BITS];
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