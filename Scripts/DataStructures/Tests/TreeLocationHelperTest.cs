using DataStructures;
using NUnit.Framework;

namespace Tests
{
    public class TreeLocationHelperTest
    {
        [Test]
        public void ShouldComputeCorrectChildTreeLocation()
        {
            Assert.That(TreeLocationHelper.childTreeLocation(0, 1), Is.EqualTo(0b01));
            Assert.That(TreeLocationHelper.childTreeLocation(TreeLocationHelper.childTreeLocation(0, 1), 3), Is.EqualTo(0b0111));
            Assert.That(TreeLocationHelper.childTreeLocation(0b0111, 2), Is.EqualTo(0b011110));
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
