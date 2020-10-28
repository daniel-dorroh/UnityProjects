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
            genes.Material.Should().Be(Convert.ToUInt32(genes.Sequence, 2));
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

        [Test]
        public void ExpressAsBool_OneGene_ReturnsTrue()
        {
            var genes = new Genes(1);
            genes.ExpressAsBool(31).Should().BeTrue();
        }

        [Test]
        public void ExpressAsBool_ZeroGene_ReturnsFalse()
        {
            var genes = new Genes(0);
            genes.ExpressAsBool(31).Should().BeFalse();
        }

        [Test]
        public void ExpressAsRgba_ReturnsColor()
        {
            var genes = new Genes(uint.MaxValue);
            var color1 = genes.ExpressAsRgba(0);
            var color2 = genes.ExpressAsRgba(7);
            color1.Should().BeEquivalentTo(color2);
        }
    }
}
