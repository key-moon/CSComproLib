using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// 簡潔ビットベクトル
// 空間計算量 : n + n/4 + n/8
// 時間計算量 :   Rank O(1)
// 　　　　　   Select O(log(n/256))
class SuccinctBitVector
{
    const int BITBLOCK_LENGTH = 32;
    const int LARGEBLOCK_LENGTH = 256;
    const int BLOCK_PER_LARGEBLOCK = LARGEBLOCK_LENGTH / BITBLOCK_LENGTH;
    
    public int Length;
    uint[] bits;
    byte[] count;
    int[] largeCount;

    public SuccinctBitVector(int length)
    {
        Length = length;
        
        bits = new uint[(length + BITBLOCK_LENGTH - 1) / BITBLOCK_LENGTH];
        count = new byte[bits.Length];
        largeCount = new int[(bits.Length + LARGEBLOCK_LENGTH - 1) / LARGEBLOCK_LENGTH];
    }
    public SuccinctBitVector(uint[] bits)
    {
        Length = bits.Length * BITBLOCK_LENGTH;
        this.bits = bits;
        count = new byte[bits.Length];
        largeCount = new int[(bits.Length + BLOCK_PER_LARGEBLOCK) / BLOCK_PER_LARGEBLOCK];
        byte sum = 0;
        for (int i = 0; i < bits.Length - 1; i++)
        {
            if ((i + 1) % BLOCK_PER_LARGEBLOCK == 0)
            {
                int ind = (i + 1) / BLOCK_PER_LARGEBLOCK;
                largeCount[ind] = largeCount[ind - 1] + sum + MyMath.PopCount(bits[i]);
                sum = 0;
            }
            else
            {
                sum += MyMath.PopCount(bits[i]);
                count[i + 1] = sum;
            }
        }
    }
    public SuccinctBitVector(bool[] bits) : this(BoolsToUInts(bits)) { }

    public bool this[int index]
    {
        get => Access(index);
    }
    public bool Access(int index) => (bits[index / BITBLOCK_LENGTH] & (1u << (index % BITBLOCK_LENGTH))) != 0;
    
    public int Rank(int index)
    {
        index++;
        int bitblockind = index / BITBLOCK_LENGTH;
        int res = largeCount[bitblockind / BLOCK_PER_LARGEBLOCK] + count[bitblockind] + MyMath.PopCount(bits[bitblockind] & (uint)((1UL << (index % BITBLOCK_LENGTH)) - 1));
        return res;
    }

    public int Select(int index, bool kind)
    {
        if (kind) return Select1(index);
        return Select0(index);
    }
    public int Select0(int index)
    {
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

        int i;
        int remain = index - (ok * LARGEBLOCK_LENGTH - largeCount[ok]);
        int offset = ok * BLOCK_PER_LARGEBLOCK;
        int max = Math.Min(BLOCK_PER_LARGEBLOCK, count.Length - offset);
        for (i = 0; i < max; i++)
        {
            if (remain < (i * BITBLOCK_LENGTH - count[offset + i]))
            {
                i--;
                break;
            }
        }
        res += i * BITBLOCK_LENGTH;

        remain = remain - (i * BITBLOCK_LENGTH - count[offset + i]);
        ok = 0;
        ng = BITBLOCK_LENGTH;
        uint bit = bits[offset + i];
        while (ng - ok > 1)
        {
            int mid = (ng + ok) / 2;
            if ((mid - MyMath.PopCount(bit & (uint)((1UL << mid) - 1))) <= remain) ok = mid;
            else ng = mid;
        }
        res += ok;
        return res;
    }
    public int Select1(int index)
    {
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

        int i;
        int remain = index - largeCount[ok];
        int offset = ok * BLOCK_PER_LARGEBLOCK;
        int max = Math.Min(BLOCK_PER_LARGEBLOCK, count.Length - offset);
        for (i = 0; i < max; i++)
        {
            if(remain < count[offset + i])
            {
                i--;
                break;
            }
        }
        res += i * BITBLOCK_LENGTH;

        remain = remain - count[offset + i];
        ok = 0;
        ng = BITBLOCK_LENGTH;
        uint bit = bits[offset + i];
        while (ng - ok > 1)
        {
            int mid = (ng + ok) / 2;
            if (MyMath.PopCount(bit & (uint)((1UL << mid) - 1)) <= remain) ok = mid;
            else ng = mid;
        }
        res += ok;
        return res;
    }

    public static uint[] BoolsToUInts(bool[] bits)
    {
        uint[] uintbits = new uint[(bits.Length + BITBLOCK_LENGTH) / BITBLOCK_LENGTH];
        for (int i = 0; i < uintbits.Length; i++)
        {
            int offset = i * BITBLOCK_LENGTH;
            int max = Math.Min(BITBLOCK_LENGTH, bits.Length - offset);
            for (int j = 0; j < max; j++)
            {
                if (bits[offset + j]) uintbits[i] |= (1u << j);
            }
        }
        return uintbits;
    }
}