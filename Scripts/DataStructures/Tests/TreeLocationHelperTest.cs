using DataStructures;
using NUnit.Framework;

namespace Tests
{
    public class TreeLocationHelperTest
    {
        [Test]
        public void ShouldComputeCorrectChildTreeLocation()
        {
            Assert.That(TreeLocationHelper.childTreeLocation(0, 1, 0, 2), Is.EqualTo(0b010000));
            Assert.That(TreeLocationHelper.childTreeLocation(0b010000, 3, 1, 2), Is.EqualTo(0b011100));
            Assert.That(TreeLocationHelper.childTreeLocation(0b011100, 2, 2, 2), Is.EqualTo(0b011110));
        }

        [Test]
        public void ShouldComputeCorrectLeftNeighbor()
        {
            Assert.That(TreeLocationHelper.leftNeighborLocation(0b000011, 2, 2), Is.EqualTo(0b10));
            Assert.That(TreeLocationHelper.leftNeighborLocation(0b010010, 2, 2), Is.EqualTo(0b000111));
            Assert.That(TreeLocationHelper.leftNeighborLocation(0b100010, 2, 2), Is.EqualTo(TreeLocationHelper.NO_NEIGHBOR_FOUND));
        }

        [Test]
        public void ShouldComputeCommonAncestors()
        {
            Assert.That(TreeLocationHelper.computeCommonAncestor(0b11001100, 0b11010101), Is.EqualTo(0b11000000));
        }
    }
}
