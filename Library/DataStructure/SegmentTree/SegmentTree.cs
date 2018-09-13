using System;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>セグ木(一点更新区間取得)</summary>
/// <example>range minimum query : new SegmentTree<int>(n, int.MaxValue, Min);</example>
/// <typeparam name="T">要素の型</typeparam>
class SegmentTree<T>
{
    /// <summary>区間2つをマージする関数を提供</summary>
    /// <returns>マージ結果</returns>
    public delegate T Merge(T a, T b);
    /// <summary></summary>
    Merge merge;
    /// <summary>単位元(初期化時に撒く)</summary>
    T IdentityElement;
    /// <summary>ノード</summary>
    public T[] nodes;
    /// <summary>最下位ノードの個数(2^nにしておく)</summary>
    int leafNodeCount;

    /// <summary>コンストラクタ</summary>
    /// <param name="length">長さ</param>
    /// <param name="identityElement">単位元(addだったら0,maxだったら-inf…)</param>
    /// <param name="merge">マージするための関数</param>
    public SegmentTree(int length, T identityElement, Merge merge)
    {
        length--;
        leafNodeCount = 1;
        this.merge = merge;
        while (length != 0)
        {
            length >>= 1;
            leafNodeCount <<= 1;
        }
        IdentityElement = identityElement;
        nodes = Enumerable.Repeat(IdentityElement, leafNodeCount << 1).ToArray();
    }

    /// <summary>Updateにインデクサでアクセス</summary>
    public T this[int index]
    {
        set { Update(index, value); }
    }

    /// <summary>indexにvalueを代入(0-indexed)</summary>
    public void Update(int index, T value)
    {
        AssertIndex(index);
        int leafInd = GetIndOfLeafNode(index);
        nodes[leafInd] = value;
        while (leafInd > 1)
        {
            leafInd >>= 1;
            MergeChilds(leafInd);
        }
    }

    /// <summary>区間クエリ</summary>
    public T Query(int s, int e)
    {
        if (s > e) return Query(e, s);
        AssertIndex(s);
        AssertIndex(e);

        int xor = (s ^ e);
        int sectionLength = 1;
        while (xor != 0)
        {
            sectionLength <<= 1;
            xor >>= 1;
        }

        int sectionBegin = s & (int.MaxValue - (sectionLength - 1));
        int sectionEnd = s & (sectionLength - 1);
        int sectionMiddle;
        if (sectionBegin == s) sectionMiddle = sectionBegin;
        else if (sectionEnd == e) sectionMiddle = sectionEnd;
        else sectionMiddle = sectionBegin + (sectionLength >> 1);

        int leafS = GetIndOfLeafNode(s);
        int leafE = GetIndOfLeafNode(e);

        int sDiff = sectionMiddle - s;
        int eDiff = e - sectionMiddle + 1;

        T res = IdentityElement;
        for (int i = 1; i <= sectionLength; i <<= 1)
        {
            if ((sDiff & i) == i)
            {
                res = merge(res, nodes[leafS]);
                leafS++;
            }
            if ((eDiff & i) == i)
            {
                res = merge(res, nodes[leafE]);
                leafE--;
            }
            leafS >>= 1;
            leafE >>= 1;
        }
        return res;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>子ノード2つをマージ</summary>
    /// <param name="index">マージするノードの親ノード</param>
    private void MergeChilds(int index)
    {
        int child = index << 1;
        nodes[index] = merge(nodes[child], nodes[child + 1]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>葉ノード(最下位ノード)のnodes内indexを取得</summary>
    private int GetIndOfLeafNode(int index) => index + leafNodeCount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>indexが範囲内かを調べる</summary>
    private void AssertIndex(int index) { if (index >= leafNodeCount) throw new IndexOutOfRangeException(); }
}