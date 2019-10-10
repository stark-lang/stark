using System;

namespace StarkPlatform.Reflection.Metadata
{
    public readonly struct Rune : IEquatable<Rune>
    { 
        private readonly int _value;

        public static readonly Rune MinValue = new Rune(0);

        public static readonly Rune MaxValue = new Rune(0x10FFFF);

        public Rune(int value)
        {
            _value = value;
        }

        public bool Equals(Rune other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is Rune other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public static bool operator ==(Rune left, Rune right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Rune left, Rune right)
        {
            return !left.Equals(right);
        }

        public static implicit operator int(Rune value)
        {
            return value._value;
        }

        public static implicit operator Rune(int value)
        {
            return new Rune(value);
        }

        public override string ToString()
        {
            return char.ConvertFromUtf32(_value);
        }
    }
}