using System;

namespace Funda.Core.Entities
{
    public readonly struct Makelaar
    {
        public int Id { get; }
        public string Name { get; }

        public Makelaar(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            if (obj is Makelaar other)
                return Id.Equals(other.Id) && Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
            return false;
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                return (Id * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }
    }
}