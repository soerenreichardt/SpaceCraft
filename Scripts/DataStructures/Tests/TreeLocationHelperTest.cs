using DataStructures;
using NUnit.Framework;

namespace Tests
{
    public class TreeLocationHelperTest
    {
        [Test]
        public void ShouldComputeCorrectChildTreeLocation()
        {
            Assert.That(TreeLocationHelper.childTreeLocation(0, 1, 0), Is.EqualTo(0b01));
            Assert.That(TreeLocationHelper.childTreeLocation(0b01, 3, 1), Is.EqualTo(0b1101));
            Assert.That(TreeLocationHelper.childTreeLocation(0b1101, 2, 2), Is.EqualTo(0b101101));
        }

        [Test]
        public void ShouldComputeCorrectLeftNeighbor()
        {
            Assert.That(TreeLocationHelper.leftNeighborLocation(0b110000, 2, 2), Is.EqualTo(0b100000));
            Assert.That(TreeLocationHelper.leftNeighborLocation(0b100001, 2, 2), Is.EqualTo(0b110100));
            Assert.That(TreeLocationHelper.leftNeighborLocation(0b100010, 2, 2), Is.EqualTo(TreeLocationHelper.NO_NEIGHBOR_FOUND));
        }

        [Test]
        public void ShouldComputeQuadrantForLevel()
        {
            var treeLocation = 0b011100;
            Assert.That(TreeLocationHelper.quadrantForLevel(treeLocation, 0), Is.EqualTo(0b0));
            Assert.That(TreeLocationHelper.quadrantForLevel(treeLocation, 1), Is.EqualTo(0b11));
            Assert.That(TreeLocationHelper.quadrantForLevel(treeLocation, 2), Is.EqualTo(0b01));
        }
        
        [Test]
        public void ShouldComputeCommonPathLength()
        {
            var commonPathLength = TreeLocationHelper.commonPathLength(0b0011001100, 0b11011100, 3);
            Assert.That(commonPathLength, Is.EqualTo(2));
            
            var commonPathLength2 = TreeLocationHelper.commonPathLength(0b11011100, 0b11010100, 3);
            Assert.That(commonPathLength2, Is.EqualTo(1));
        }
    }
}
