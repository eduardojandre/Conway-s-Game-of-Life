using System.Text.Json;
using BoardManager;

namespace BoardManagerUnitTests.BoardRuleEngineTests
{
    public class CalculateNextState
    {
        private IBoardRulesEngine _sut;
        [SetUp]
        public void Setup()
        {
            _sut = new BoardRulesEngine();
        }

        [TestCase( 
        "[" +
            "[false, false, false, false]," +
            "[false, true , true , false]," +
            "[false, true , true , false]," +
            "[false, false, false, false]" +
        "]")]
        [TestCase(
        "[" +
            "[false, false, false, false, false]," +
            "[false, false, true , false, false]," +
            "[false, true , false, true , false]," +
            "[false, false, true , false, false]," +
            "[false, false, false, false, false]" +
        "]")]
        public void ShouldCalculateNextStateGivenFinalState(string currentState)
        {
            AssertCalculateNextState(currentState, currentState);
        }

        [TestCase(
        "[" +
            "[false, false, false, false, false]," +
            "[false, false, true , false, false]," +
            "[false, false, true , false, false]," +
            "[false, false, true , false, false]," +
            "[false, false, false, false, false]" +
        "]",
        "[" +
            "[false, false, false, false, false]," +
            "[false, false, false, false, false]," +
            "[false, true , true , true , false]," +
            "[false, false, false, false, false]," +
            "[false, false, false, false, false]" +
        "]")]
        [TestCase(
        "[" +
            "[false, false, false, false, false, false]," +
            "[false, true , true , false, false, false]," +
            "[false, true , false, false, false, false]," +
            "[false, false, false, false, true , false]," +
            "[false, false, false, true , true , false]" +
            "[false, false, false, false, false, false]" +
        "]",
        "[" +
            "[false, false, false, false, false, false]," +
            "[false, true , true , false, false, false]," +
            "[false, true , true , false, false, false]," +
            "[false, false, false, true , true , false]," +
            "[false, false, false, true , true , false]" +
            "[false, false, false, false, false, false]" +
        "]")]
        public void ShouldCalculateNextStateGivenOscillatorsP2(string firstState, string secondState)
        {
            AssertCalculateNextState(firstState, secondState);
            AssertCalculateNextState(secondState, firstState);
        }
        private void AssertCalculateNextState(string currentStateStr, string expectedStr)
        {
            var currentState = JsonSerializer.Deserialize<bool[][]>(currentStateStr);
            var expected = JsonSerializer.Deserialize<bool[][]>(expectedStr);
            var actual = _sut.CalculateNextState(currentState);
            Assert.That(AreBoardStatesEquivalent(actual, expected), Is.True);
        }
        private bool AreBoardStatesEquivalent(bool[][] state1, bool[][] state2)
        {
            for (int i = 0; i < state1.Length; i++)
            {
                if (!Enumerable.SequenceEqual(state1[i], state2[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
