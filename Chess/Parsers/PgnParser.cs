using Chess.Models.Pgn;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Parsers
{
    /// <summary>
    /// https://www.thechessdrum.net/PGN_Reference.txt
    /// </summary>
    public class PgnParser { 
        public static IEnumerable<PgnGame> Parse(string input)
        {
            int position = 0;
            do
            {
                MoveToAttributes(input, ref position);
                var attributes = ParseAttributes(input, ref position);
                SkipWhiteSpace(input, ref position);
                var moves = ParseMoves(input, ref position);
                yield return new PgnGame(attributes, moves);
                SkipWhiteSpace(input, ref position);
            } while (position < input.Length);
        }

        private static void MoveToAttributes(string span, ref int position)
        {
            while (position < span.Length && span[position] != '[') position++;
        }

        private static MoveText ParseMoves(string span, ref int position)
        {
            var savedPos = position;
            MoveToAttributes(span, ref position);
            var reader = new MoveTextReader(span.AsMemory()[savedPos..position]);
            return reader.ReadAll();
        }

        private static void SkipWhiteSpace(string span, ref int position)
        {
            while (position < span.Length && char.IsWhiteSpace(span[position])) position++;
        }

        private static Dictionary<string ,string> ParseAttributes(string span, ref int position)
        {
            Expect('[', span, ref position, false);
            Dictionary<string, string> attributes = new();
            do
            {
                var (key, value) = ParseAttribute(span, ref position);
                attributes.Add(key, value);
            } while (position < span.Length && span[position] == '[');

            return attributes;
        }

        private static (string span,string) ParseAttribute(string span, ref int position)
        {
            Expect('[', span, ref position);
            var key = ParseKey(span, ref position);
            SkipWhiteSpace(span, ref position);
            var value = ParseString(span, ref position);
            SkipWhiteSpace(span, ref position);
            Expect(']', span, ref position);
            SkipWhiteSpace(span, ref position);
            return (key, value);
        }

        private static string ParseKey(string span, ref int position)
        {
            var endPos = position;
            while (endPos < span.Length && char.IsLetterOrDigit(span[endPos])) endPos++;
            var key = span[position..endPos].ToString();
            position = endPos;
            return key;
        }

        private static string ParseString(string span, ref int position)
        {
            Expect('"', span, ref position);
            var endPos = position;
            while (endPos < span.Length && span[endPos] != '"') endPos++;
            var key = span[position..endPos].ToString();
            position = endPos;
            Expect('"', span, ref position);
            return key;
        }

        private static void Expect(char expected, string span, ref int position, bool take = true)
        {
            if (position >= span.Length)
                throw new PgnParseError($"Expected {expected} but found end of stream at position {position}");

            if (span[position] != expected)
                throw new PgnParseError($"Expected {expected} but found {span[position]} at position {position}");

            if (take) position++;
        }
    }
}
