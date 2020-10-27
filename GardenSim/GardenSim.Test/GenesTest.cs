using System;
using FluentAssertions;
using Models;
using NUnit.Framework;

namespace GardenSim.Test
{
    public class GenesTest
    {
        [Test]
        public void Constructor_PadsSequence()
        {
            var genes = new Genes(3);
            genes.Sequence.Should().Be("00000000000000000000000000000011");
        }

        [Test]
        public void Random_CreatesSelfConsistentGenes()
        {
            var genes = Genes.Random();
            genes.Material.Should().Be(Convert.ToInt32(genes.Sequence, 2));
        }

        [Test]
        public void Combine_CreatesNewGenes()
        {
            var parent1 = Genes.Random();
            var parent2 = Genes.Random();
            var combined = parent1.Combine(parent2);
            for (int i = 0; i < 32; i += 2)
            {
                var gene = combined.Sequence.Substring(i, 2);
                var isParentGene = gene == parent1.Sequence.Substring(i, 2)
                                   || gene == parent2.Sequence.Substring(i, 2);
                isParentGene.Should().BeTrue();
            }
        }
    }
}
