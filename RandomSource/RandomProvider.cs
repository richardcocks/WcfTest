using System;
using System.Collections;
using System.Collections.Generic;

namespace RandomSource;

public class RandomProvider(Random random, int fidelity) : IEnumerable<SequenceValue>
{
    private long _sequence = 0L;

    private readonly int _fidelityMask = (int)((1L << fidelity) - 1);

    public RandomProvider(int fidelity): this(new Random(0),fidelity)
    {
    }

    public IEnumerator<SequenceValue> GetEnumerator()
    {
        while (true)
        {
            yield return new SequenceValue(_sequence++,  random.Next() & _fidelityMask);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}