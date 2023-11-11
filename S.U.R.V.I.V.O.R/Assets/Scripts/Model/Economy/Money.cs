using System;

namespace TheRevenantsAge
{
    public struct Money: IEquatable<Money>
    {
        public MoneyType MoneyType { get; }

        public uint Amount { get; }

        public Money(MoneyType moneyType, uint amount)
        {
            MoneyType = moneyType;
            Amount = amount;
        }

        public static Money operator -(Money first, Money second)
        {
            if (first.MoneyType != second.MoneyType) 
                throw new InvalidOperationException();
            return new Money(first.MoneyType, first.Amount - second.Amount);
        }
        
        public static Money operator +(Money first, Money second)
        {
            if (first.MoneyType != second.MoneyType) 
                throw new InvalidOperationException();
            return new Money(first.MoneyType, first.Amount + second.Amount);
        }
        
        public static Money operator -(Money first, uint second)
        {
            return new Money(first.MoneyType, first.Amount - second);
        }
        
        public static Money operator +(Money first, uint second)
        {
            return new Money(first.MoneyType, first.Amount + second);
        }

        public static bool operator ==(Money first, Money second)
        {
            return first.Equals(second);
        }
        
        public static bool operator !=(Money first, Money second)
        {
            return !first.Equals(second);
        }

        public bool Equals(Money other)
        {
            return MoneyType == other.MoneyType && Amount == other.Amount;
        }

        public override bool Equals(object obj)
        {
            return obj is Money other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int) MoneyType, Amount);
        }
    }
}