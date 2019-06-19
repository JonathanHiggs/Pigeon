using System;

using Pigeon.Annotations;

namespace Pigeon.Sandbox.Contracts
{
    [Serializable]
    [Topic]
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
