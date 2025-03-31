using BoardManager;

namespace BoardManagerUnitTests.BoardRuleEngineTests
{
    public class CalculateNextCellState
    {
        private IBoardRulesEngine _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new BoardRulesEngine();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void ShouldBeDeadGivenUnderpopulation(int neighborsAlive)
        {
            AssertCalculateNextCellState(true, neighborsAlive, false);
        }

        [TestCase(2)]
        [TestCase(3)]
        public void ShouldStayAliveGivenNeighborsAlive(int neighborsAlive)
        {
            AssertCalculateNextCellState(true, neighborsAlive, true);
        }

        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        public void ShouldBeDeadGivenOverpopulation(int neighborsAlive)
        {
            AssertCalculateNextCellState(true, neighborsAlive, false);
        }

        [Test]
        public void ShouldBecomeAliveGivenReproduction()
        {
            AssertCalculateNextCellState(true, 3, true);
        }
        
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        public void ShouldStayDeadGivenNeighborsAlive(int neighborsAlive)
        {
            AssertCalculateNextCellState(false, neighborsAlive, false);
        }

        private void AssertCalculateNextCellState(bool isAlive, int neighborsAlive, bool expected)
        {
            var actual = _sut.CalculateNextCellState(isAlive, neighborsAlive);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}