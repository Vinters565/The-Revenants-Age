using System;

namespace Extension
{
    public class Range
    {
        public int Start { get; }
        public int End { get; }

        public Range(int point): this(point, point)
        {
        }

        public Range(int startPoint, int endPoint)
        {
            if (startPoint < 0 || endPoint < 0)
                throw new ArgumentException();
            if (startPoint > endPoint)
                throw new ArgumentException();
            this.Start = startPoint;
            this.End = endPoint;
        }
    }
}