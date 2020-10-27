using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Genes
    {
        private int _material;
        private string _sequence;

        private static readonly Random RandomGenerator = new Random();

        public Genes(int material)
        {
            _material = material;
            _sequence = Convert.ToString(material, 2).PadLeft(32, '0');
        }

        public int Material => _material;
        public string Sequence => _sequence;

        public static Genes Random()
        {
            var material = RandomGenerator.Next(int.MaxValue);
            return new Genes(material);
        }

        public Genes Combine(Genes parent2)
        {
            var sequence2 = parent2._sequence;
            var combination = new StringBuilder();

            for (int i = 0; i < 32; i += 2)
            {
                string gene;

                if (RandomGenerator.NextDouble() > 0.5)
                {
                    gene = _sequence.Substring(i, 2);
                }
                else
                {
                    gene = sequence2.Substring(i, 2);
                }

                combination.Append(gene);
            }

            var combinedMaterial = Convert.ToInt32(combination.ToString(), 2);
            return new Genes(combinedMaterial);
        }
    }
}
