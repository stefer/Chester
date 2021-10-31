using Chess.Evaluations;
using Chess.Models;
using Chess.Search;
using FakeItEasy;
using NUnit.Framework;

namespace Chess.Tests.Search
{
    internal class AlphaBetaMinMaxSearchTests
    {
        [TestCase(Color.White, 0,  1, ExpectedResult =  1)]
        [TestCase(Color.White, 0, -1, ExpectedResult = -1)]
        [TestCase(Color.White, 1,  1, ExpectedResult =  1)]
        [TestCase(Color.White, 1, -1, ExpectedResult = -1)]
        [TestCase(Color.Black, 0,  1, ExpectedResult = -1)]
        [TestCase(Color.Black, 0, -1, ExpectedResult =  1)]
        [TestCase(Color.Black, 1,  1, ExpectedResult = -1)]
        [TestCase(Color.Black, 1, -1, ExpectedResult =  1)]
        public int Search_ReturnsScore(Color toMove, int depth, int score)
        {
            var evaluator = A.Fake<IEvaluator>();
            A.CallTo(() => evaluator.Evaluate(A<Board>._)).Returns(score);
            var sut = new AlphaBetaMinMaxSearch(new SearchOptions(depth), evaluator);

            var board = new Board();
            var eval = sut.Search(board, toMove);

            return eval.Value;
        }

        [Test]
        public void DepthZero_ReturnsHighestScore()
        {
            var evaluator = A.Fake<IEvaluator>();
            A.CallTo(() => evaluator.Evaluate(A<Board>._)).Returns(0);
            A.CallTo(() => evaluator.Evaluate(A<Board>.That.Matches(b => b.At("e4").IsWhite()))).Returns(10);
            A.CallTo(() => evaluator.Evaluate(A<Board>.That.Matches(b => b.At("d4").IsWhite()))).Returns(9);
            var sut = new AlphaBetaMinMaxSearch(new SearchOptions(0), evaluator);

            var board = new Board();
            var eval = sut.Search(board, Color.White);

            Assert.That(eval.Value, Is.EqualTo(10));
            Assert.That(eval.Move.ToStringLong(), Is.EqualTo("e2e4"));
        }

        [Test]
        public void DepthOne_ReturnsHighestScore()
        {
            var evaluator = A.Fake<IEvaluator>();
            A.CallTo(() => evaluator.Evaluate(A<Board>._)).Returns(0);
            A.CallTo(() => evaluator.Evaluate(A<Board>.That.Matches(b => b.At("e4").IsWhite()))).Returns(10);
            A.CallTo(() => evaluator.Evaluate(A<Board>.That.Matches(b => b.At("e4").IsWhite() && b.At("d5").IsBlack()))).Returns(-10);
            A.CallTo(() => evaluator.Evaluate(A<Board>.That.Matches(b => b.At("d4").IsWhite()))).Returns(8);
            var sut = new AlphaBetaMinMaxSearch(new SearchOptions(1), evaluator);

            var board = new Board();
            var eval = sut.Search(board, Color.White);

            Assert.That(eval.Value, Is.EqualTo(8));
            Assert.That(eval.Move.ToStringLong(), Is.EqualTo("d2d4"));
        }

    }
}
