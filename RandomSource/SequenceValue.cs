using System;

namespace RandomSource
{
    public struct SequenceValue(long sequence, long value)
    {
        public long Sequence { get; } = sequence;
        public long Value { get; } = value;
    }
}