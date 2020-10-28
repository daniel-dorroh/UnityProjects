using System;
using System.Collections;
using System.Collections.Generic;

namespace Models
{
    public class SeedFactory<T>
        where T : ILife, new()
    {
        private Genes _parent1;
        private Genes _parent2;

        public SeedFactory()
        {
            _parent1 = Genes.Random();
            _parent2 = Genes.Random();
        }

        public SeedFactory(Genes parent1, Genes parent2)
        {
            _parent1 = parent1;
            _parent2 = parent2;
        }

        protected virtual Func<string, string> Mutate { get; } = null;

        public Seed<T> Create()
        {
            var genes = _parent1.Combine(_parent2, Mutate);
            return new Seed<T>(genes);
        }
    }
}
