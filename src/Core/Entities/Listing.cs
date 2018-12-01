using System;

namespace Funda.Core.Entities
{
    public readonly struct Listing
    {
        public Guid Id { get; }
        public Makelaar Makelaar { get; }
     
        public Listing(Guid id, Makelaar makelaar)
        {
            Id = id;
            Makelaar = makelaar;
        }
    }
}