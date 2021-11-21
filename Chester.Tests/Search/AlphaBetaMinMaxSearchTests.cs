using Chester.Models;
using Chester.Parsers;
using Chester.Search;
using Chester.Evaluations;
using FakeItEasy;
using NUnit.Framework;
using System.Collections.Generic;

namespace Chester.Tests.Search
{
    internal class AlphaBetaMinMaxSearchTests
    {
        [TestCase(Color.White, 0, 1, ExpectedResult = 1)]
        [TestCase(Color.White, 0, -1, ExpectedResult = -1)]
        [TestCase(Color.White, 1, 1, ExpectedResult = 1)]
        [TestCase(Color.White, 1, -1, ExpectedResult = -1)]
        [TestCase(Color.Black, 0, -1, ExpectedResult = -1)]
        [TestCase(Color.Black, 0, 1, ExpectedResult = 1)]
        [TestCase(Color.Black, 1, -1, ExpectedResult = -1)]
        [TestCase(Color.Black, 1, 1, ExpectedResult = 1)]
        public int Search_ReturnsScore(Color toMove, int depth, int score)
        {
            var evaluator = A.Fake<IEvaluator>();
            A.CallTo(() => evaluator.Evaluate(A<Board>._)).Returns(score);
            var sut = new AlphaBetaMinMaxSearch(new SearchOptions(depth), evaluator, new NullReporter());

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
            var sut = new AlphaBetaMinMaxSearch(new SearchOptions(0), evaluator, new NullReporter());

            var board = new Board();
            var eval = sut.Search(board, Color.White);

            Assert.That(eval.Value, Is.EqualTo(10));
            Assert.That(eval.Move.ToStringLong(), Is.EqualTo("e2e4"));
        }

        [Test]
        public void DepthZero_ReturnsHighestScoreBlack()
        {
            var evaluator = A.Fake<IEvaluator>();
            A.CallTo(() => evaluator.Evaluate(A<Board>._)).Returns(0);
            A.CallTo(() => evaluator.Evaluate(A<Board>.That.Matches(b => b.At("e4").IsWhite()))).Returns(10);
            A.CallTo(() => evaluator.Evaluate(A<Board>.That.Matches(b => b.At("d4").IsWhite()))).Returns(9);
            A.CallTo(() => evaluator.Evaluate(A<Board>.That.Matches(b => b.At("e5").IsBlack()))).Returns(-11);
            A.CallTo(() => evaluator.Evaluate(A<Board>.That.Matches(b => b.At("d6").IsBlack()))).Returns(-9);
            var sut = new AlphaBetaMinMaxSearch(new SearchOptions(0), evaluator, new NullReporter());

            var board = new Board();
            board.MakeMove(new Move(board.At("e2"), "e2", "e4")); ;
            var eval = sut.Search(board, Color.Black);

            Assert.That(eval.Value, Is.EqualTo(-11));
            Assert.That(eval.Move.ToStringLong(), Is.EqualTo("e7e5"));
        }


        [Test]
        public void DepthOne_ReturnsHighestScore()
        {
            var evaluator = A.Fake<IEvaluator>();
            A.CallTo(() => evaluator.Evaluate(A<Board>._)).Returns(0);
            A.CallTo(() => evaluator.Evaluate(A<Board>.That.Matches(b => b.At("e4").IsWhite()))).Returns(10);
            A.CallTo(() => evaluator.Evaluate(A<Board>.That.Matches(b => b.At("e4").IsWhite() && b.At("d5").IsBlack()))).Returns(-10);
            A.CallTo(() => evaluator.Evaluate(A<Board>.That.Matches(b => b.At("d4").IsWhite()))).Returns(8);
            var sut = new AlphaBetaMinMaxSearch(new SearchOptions(1), evaluator, new NullReporter());

            var board = new Board();
            var eval = sut.Search(board, Color.White);

            Assert.That(eval.Value, Is.EqualTo(8));
            Assert.That(eval.Move.ToStringLong(), Is.EqualTo("d2d4"));
        }

        [Test, Explicit]
        public void DepthThree_ReturnsHighestScore()
        {
            var evaluator = new Evaluator();
            var sut = new AlphaBetaMinMaxSearch(new SearchOptions(3), evaluator, new NullReporter());

            var board = new Board();
            var eval = sut.Search(board, Color.White);

            Assert.That(eval.Value, Is.EqualTo(40));
            Assert.That(eval.Move.ToStringLong(), Is.EqualTo("d2d4"));
        }

        [Test]
        public void DepthOne_ReturnsHighestScore_ForBlackQueenTake()
        {
            var evaluator = new Evaluator();
            var sut = new AlphaBetaMinMaxSearch(new SearchOptions(1), evaluator, new NullReporter());

            var board = new Board();
            Play(board, "e2e4 g8h6 d1g4");
            var eval = sut.Search(board, Color.Black);

            Assert.That(eval.Value, Is.EqualTo(-910));
            Assert.That(eval.Move.ToStringLong(), Is.EqualTo("h6g4"));
        }

        [Test]
        public void InvalidOperation_MovedBlack()
        {
            var evaluator = new Evaluator();
            var sut = new AlphaBetaMinMaxSearch(new SearchOptions(1), evaluator, new NullReporter());

            var board = new Board();
            Play(board, "e2e4 b8c6 g1f3 g8f6 b1c3 a8b8 f1b5 f6g4 e1g1 a7a6 b5a4 b7b5 a4b3 c6a5 b3d5 b5b4 c3e2 c7c6 d5b3 a5b3 a2b3 c8b7 h2h3 g4f6 d2d4 f6e4 d1d3 e4d6 c2c3 b4c3 b2c3 b8c8 f3e5 c8c7 c3c4 f7f6 e5g4 f6f5 g4e5 c6c5 d4d5 d6e4 c1b2 h8g8 b2a3 d7d6 e5f3 g7g6 b3b4 c5b4 a3b4 c7d7 b4a5 d8c8 f1b1");
            Assert.That(() => sut.Search(board, Color.Black), Throws.Nothing);
        }

        private static void Play(Board board, string moveText)
        {
            var reader = new MoveTextReader(moveText);
            var moves = reader.ReadAll();
            board.MakeMoves(moves);
        }
    }
    public class NullReporter : ISearchReporter
    {
        public void BestLine(int depth, int score, IEnumerable<Move> bestLine)
        {
        }

        public void CurrentMove(Move move, long moveNumber, int score)
        {
        }

        public void NodeVisited()
        {
        }

        public void Reset()
        {
        }

    }

}
