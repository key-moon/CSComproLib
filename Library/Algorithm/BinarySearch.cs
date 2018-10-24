using System.Runtime.CompilerServices;

static partial class Algorithm
{
    delegate bool MonotonicIncreaseIntFunc(int value);
    delegate bool MonotonicIncreaseLongFunc(long value);
    delegate bool MonotonicIncreaseDoubleFunc(double value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int BinarySearch(int ok, int ng, MonotonicIncreaseIntFunc func)
    {
        if(ng > ok)
        {
            while (ng - ok > 1)
            {
                int mid = (ng + ok) / 2;
                if (func(mid)) ok = mid;
                else ng = mid;
            }
        }
        else
        {
            while (ok - ng > 1)
            {
                int mid = (ng + ok) / 2;
                if (func(mid)) ok = mid;
                else ng = mid;
            }
        }
        return ok;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static long BinarySearch(long ok, long ng, MonotonicIncreaseLongFunc func)
    {
        if (ng > ok)
        {
            while (ng - ok > 1)
            {
                long mid = (ng + ok) / 2;
                if (func(mid)) ok = mid;
                else ng = mid;
            }
        }
        else
        {
            while (ok - ng > 1)
            {
                long mid = (ng + ok) / 2;
                if (func(mid)) ok = mid;
                else ng = mid;
            }
        }
        return ok;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static double BinarySearch(double ok, double ng, MonotonicIncreaseDoubleFunc func, double tolerance = 1e-5)
    {
        if (ng > ok)
        {
            while (ng - ok > tolerance)
            {
                double mid = (ng + ok) / 2;
                if (func(mid)) ok = mid;
                else ng = mid;
            }
        }
        else
        {
            while (ok - ng > tolerance)
            {
                double mid = (ng + ok) / 2;
                if (func(mid)) ok = mid;
                else ng = mid;
            }
        }
        return ok;
    }
}