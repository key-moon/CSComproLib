using System.Runtime.CompilerServices;

class PersistentStack<T>
{
    PersistentStack<T> previousStack;
    public T Top { get; private set; }
    public int Count { get; private set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PersistentStack() : this(null, default(T), 0) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private PersistentStack(PersistentStack<T> prev, T top, int count)
    {
        previousStack = prev;
        Top = top;
        Count = count;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PersistentStack<T> Push(T value)
    {
        var res = Copy();
        res.previousStack = this;
        res.Top = value;
        res.Count++;
        return res;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PersistentStack<T> Pop() => previousStack is null ? null : previousStack.Copy();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PersistentStack<T> Copy() => new PersistentStack<T>(previousStack, Top, Count);
}
