using System;

namespace Pigeon.Sandbox.Contracts
{
    [Serializable]
    public class Observation
    {
        public readonly string Name;
        public readonly double Price;

        public Observation(string name, double price)
        {
            Name = name;
            Price = price;
        }

        public override string ToString()
        {
            return $"{Name}: {Price:C4}";
        }
    }
}
