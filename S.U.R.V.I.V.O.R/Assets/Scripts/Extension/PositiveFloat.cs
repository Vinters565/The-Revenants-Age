using System;

namespace Extension
{
    public struct PositiveFloat 
    {
        private float _value;
        public PositiveFloat(float val) 
        {
            if (val < 0)
                throw new ArgumentException("Value needs to be positive");
            _value = val;
        }
        
        public static implicit operator float(PositiveFloat d) => d._value;
        public static explicit operator PositiveFloat(float d) => new(d);
    }
}