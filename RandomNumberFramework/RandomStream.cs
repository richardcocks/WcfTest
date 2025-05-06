using System;
using System.IO;

namespace RandomNumberFramework
{
    public class RandomStream : Stream
    {
        public RandomStream(Random random)
        {
            this._random = random;
        }
    
        private int _sequence;
        private readonly Random _random;
        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        // ReSharper disable once ValueParameterNotUsed
        public override long Position { get => _sequence; set => throw new NotSupportedException(); }

        public override void Flush()
        {}
    
        public override int Read(byte[] buffer, int offset, int count)
        {
            byte[] internalBuffer = new byte[count];
            _random.NextBytes(internalBuffer);
            internalBuffer.CopyTo(buffer, offset);
            _sequence+=count;
            return count;
        }
        
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}