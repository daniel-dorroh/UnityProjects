using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RandomGenerator = UnityEngine.Random;

namespace Models
{
    public class Genes
    {
        private uint _material;
        private string _sequence;

        public Genes(uint material)
        {
            _material = material;
            _sequence = Convert.ToString(material, 2).PadLeft(32, '0');
        }

        public uint Material => _material;
        public string Sequence => _sequence;

        public static Genes Random()
        {
            var material = (uint)Math.Floor(RandomGenerator.value * uint.MaxValue);
            return new Genes(material);
        }

        public Genes Combine(Genes parent2, Func<string, string> mutate = null)
        {
            var sequence2 = parent2._sequence;
            var combination = new StringBuilder();

            for (int i = 0; i < 32; i += 2)
            {
                string gene;

                if (RandomGenerator.value > 0.5)
                {
                    gene = _sequence.Substring(i, 2);
                }
                else
                {
                    gene = sequence2.Substring(i, 2);
                }

                gene = mutate != null ? mutate(gene) : gene;
                combination.Append(gene);
            }

            var combinedMaterial = Convert.ToUInt32(combination.ToString(), 2);
            return new Genes(combinedMaterial);
        }

        public bool ExpressAsBool(int offset)
        {
            if (offset > 31)
            {
                offset = offset % 32;
            }

            return _sequence[offset] == '1';
        }

        public Color ExpressAsRgba(int offset)
        {
            var colors = new List<int>();
            var color = new List<char>();

            for (int i = offset; i < offset + 32 && colors.Count != 4; i++)
            {
                i = i % 32;
                color.Add(_sequence[i]);
                if (color.Count == 8)
                {
                    colors.Add(Convert.ToInt16(new string(color.ToArray()), 2));
                    color.Clear();
                }
            }

            return new Color(colors[0] / 255.0f, colors[1] / 255.0f, colors[2] / 255.0f, colors[3] / 255.0f);
        }
    }
}
